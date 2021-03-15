using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading;

public sealed class RecyclableMemoryStreamManager
{
    [EventSource(Name = "Microsoft-IO-RecyclableMemoryStream", Guid = "{B80CD4E4-890E-468D-9CBA-90EB7C82DFC7}")]
    public sealed class Events : EventSource
    {
        public enum MemoryStreamBufferType
        {
            Small,
            Large
        }

        public enum MemoryStreamDiscardReason
        {
            TooLarge,
            EnoughFree
        }

        public static Events Writer = new Events();

        [Event(1, Level = EventLevel.Verbose)]
        public void MemoryStreamCreated(Guid guid, string tag, int requestedSize)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(1, guid, tag ?? string.Empty, requestedSize);
            }
        }

        [Event(2, Level = EventLevel.Verbose)]
        public void MemoryStreamDisposed(Guid guid, string tag)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(2, guid, tag ?? string.Empty);
            }
        }

        [Event(3, Level = EventLevel.Critical)]
        public void MemoryStreamDoubleDispose(Guid guid, string tag, string allocationStack, string disposeStack1, string disposeStack2)
        {
            if (IsEnabled())
            {
                WriteEvent(3, guid, tag ?? string.Empty, allocationStack ?? string.Empty, disposeStack1 ?? string.Empty, disposeStack2 ?? string.Empty);
            }
        }

        [Event(4, Level = EventLevel.Error)]
        public void MemoryStreamFinalized(Guid guid, string tag, string allocationStack)
        {
            if (IsEnabled())
            {
                WriteEvent(4, guid, tag ?? string.Empty, allocationStack ?? string.Empty);
            }
        }

        [Event(5, Level = EventLevel.Verbose)]
        public void MemoryStreamToArray(Guid guid, string tag, string stack, int size)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(5, guid, tag ?? string.Empty, stack ?? string.Empty, size);
            }
        }

        [Event(6, Level = EventLevel.Informational)]
        public void MemoryStreamManagerInitialized(int blockSize, int largeBufferMultiple, int maximumBufferSize)
        {
            if (IsEnabled())
            {
                WriteEvent(6, blockSize, largeBufferMultiple, maximumBufferSize);
            }
        }

        [Event(7, Level = EventLevel.Verbose)]
        public void MemoryStreamNewBlockCreated(long smallPoolInUseBytes)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(7, smallPoolInUseBytes);
            }
        }

        [Event(8, Level = EventLevel.Verbose)]
        public void MemoryStreamNewLargeBufferCreated(int requiredSize, long largePoolInUseBytes)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(8, requiredSize, largePoolInUseBytes);
            }
        }

        [Event(9, Level = EventLevel.Verbose)]
        public void MemoryStreamNonPooledLargeBufferCreated(int requiredSize, string tag, string allocationStack)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(9, requiredSize, tag ?? string.Empty, allocationStack ?? string.Empty);
            }
        }

        [Event(10, Level = EventLevel.Warning)]
        public void MemoryStreamDiscardBuffer(MemoryStreamBufferType bufferType, string tag, MemoryStreamDiscardReason reason)
        {
            if (IsEnabled())
            {
                WriteEvent(10, bufferType, tag ?? string.Empty, reason);
            }
        }

        [Event(11, Level = EventLevel.Error)]
        public void MemoryStreamOverCapacity(int requestedCapacity, long maxCapacity, string tag, string allocationStack)
        {
            if (IsEnabled())
            {
                WriteEvent(11, requestedCapacity, maxCapacity, tag ?? string.Empty, allocationStack ?? string.Empty);
            }
        }
    }

    public delegate void EventHandler();

    public delegate void LargeBufferDiscardedEventHandler(Events.MemoryStreamDiscardReason reason);

    public delegate void StreamLengthReportHandler(long bytes);

    public delegate void UsageReportEventHandler(long smallPoolInUseBytes, long smallPoolFreeBytes, long largePoolInUseBytes, long largePoolFreeBytes);

    public const int DefaultBlockSize = 131072;

    public const int DefaultLargeBufferMultiple = 1048576;

    public const int DefaultMaximumBufferSize = 134217728;

    private readonly int blockSize;

    private readonly long[] largeBufferFreeSize;

    private readonly long[] largeBufferInUseSize;

    private readonly int largeBufferMultiple;

    private readonly ConcurrentStack<byte[]>[] largePools;

    private readonly int maximumBufferSize;

    private readonly ConcurrentStack<byte[]> smallPool;

    private long smallPoolFreeSize;

    private long smallPoolInUseSize;

    public int BlockSize => blockSize;

    public int LargeBufferMultiple => largeBufferMultiple;

    public int MaximumBufferSize => maximumBufferSize;

    public long SmallPoolFreeSize => smallPoolFreeSize;

    public long SmallPoolInUseSize => smallPoolInUseSize;

    public long LargePoolFreeSize => largeBufferFreeSize.Sum();

    public long LargePoolInUseSize => largeBufferInUseSize.Sum();

    public long SmallBlocksFree => smallPool.Count;

    public long LargeBuffersFree
    {
        get
        {
            long num = 0L;
            ConcurrentStack<byte[]>[] array = largePools;
            foreach (ConcurrentStack<byte[]> concurrentStack in array)
            {
                num += concurrentStack.Count;
            }
            return num;
        }
    }

    public long MaximumFreeSmallPoolBytes
    {
        get;
        set;
    }

    public long MaximumFreeLargePoolBytes
    {
        get;
        set;
    }

    public long MaximumStreamCapacity
    {
        get;
        set;
    }

    public bool GenerateCallStacks
    {
        get;
        set;
    }

    public bool AggressiveBufferReturn
    {
        get;
        set;
    }

    public event EventHandler BlockCreated;

    public event EventHandler BlockDiscarded;

    public event EventHandler LargeBufferCreated;

    public event EventHandler StreamCreated;

    public event EventHandler StreamDisposed;

    public event EventHandler StreamFinalized;

    public event StreamLengthReportHandler StreamLength;

    public event EventHandler StreamConvertedToArray;

    public event LargeBufferDiscardedEventHandler LargeBufferDiscarded;

    public event UsageReportEventHandler UsageReport;

    public RecyclableMemoryStreamManager()
        : this(131072, 1048576, 134217728)
    {
    }

    public RecyclableMemoryStreamManager(int blockSize, int largeBufferMultiple, int maximumBufferSize)
    {
        if (blockSize <= 0)
        {
            throw new ArgumentOutOfRangeException("blockSize", blockSize, "blockSize must be a positive number");
        }
        if (largeBufferMultiple <= 0)
        {
            throw new ArgumentOutOfRangeException("largeBufferMultiple", "largeBufferMultiple must be a positive number");
        }
        if (maximumBufferSize < blockSize)
        {
            throw new ArgumentOutOfRangeException("maximumBufferSize", "maximumBufferSize must be at least blockSize");
        }
        this.blockSize = blockSize;
        this.largeBufferMultiple = largeBufferMultiple;
        this.maximumBufferSize = maximumBufferSize;
        if (!IsLargeBufferMultiple(maximumBufferSize))
        {
            throw new ArgumentException("maximumBufferSize is not a multiple of largeBufferMultiple", "maximumBufferSize");
        }
        smallPool = new ConcurrentStack<byte[]>();
        int num = maximumBufferSize / largeBufferMultiple;
        largeBufferInUseSize = new long[num + 1];
        largeBufferFreeSize = new long[num];
        largePools = new ConcurrentStack<byte[]>[num];
        for (int i = 0; i < largePools.Length; i++)
        {
            largePools[i] = new ConcurrentStack<byte[]>();
        }
        Events.Writer.MemoryStreamManagerInitialized(blockSize, largeBufferMultiple, maximumBufferSize);
    }

    internal byte[] GetBlock()
    {
        if (!smallPool.TryPop(out byte[] result))
        {
            result = new byte[BlockSize];
            Events.Writer.MemoryStreamNewBlockCreated(smallPoolInUseSize);
            ReportBlockCreated();
        }
        else
        {
            Interlocked.Add(ref smallPoolFreeSize, -BlockSize);
        }
        Interlocked.Add(ref smallPoolInUseSize, BlockSize);
        return result;
    }

    internal byte[] GetLargeBuffer(int requiredSize, string tag)
    {
        requiredSize = RoundToLargeBufferMultiple(requiredSize);
        int num = requiredSize / largeBufferMultiple - 1;
        byte[] result;
        if (num < largePools.Length)
        {
            if (!largePools[num].TryPop(out result))
            {
                result = new byte[requiredSize];
                Events.Writer.MemoryStreamNewLargeBufferCreated(requiredSize, LargePoolInUseSize);
                ReportLargeBufferCreated();
            }
            else
            {
                Interlocked.Add(ref largeBufferFreeSize[num], -result.Length);
            }
        }
        else
        {
            num = largeBufferInUseSize.Length - 1;
            result = new byte[requiredSize];
            string allocationStack = null;
            if (GenerateCallStacks)
            {
                allocationStack = Environment.StackTrace;
            }
            Events.Writer.MemoryStreamNonPooledLargeBufferCreated(requiredSize, tag, allocationStack);
            ReportLargeBufferCreated();
        }
        Interlocked.Add(ref largeBufferInUseSize[num], result.Length);
        return result;
    }

    private int RoundToLargeBufferMultiple(int requiredSize)
    {
        return (requiredSize + LargeBufferMultiple - 1) / LargeBufferMultiple * LargeBufferMultiple;
    }

    private bool IsLargeBufferMultiple(int value)
    {
        if (value != 0)
        {
            return value % LargeBufferMultiple == 0;
        }
        return false;
    }

    internal void ReturnLargeBuffer(byte[] buffer, string tag)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException("buffer");
        }
        if (!IsLargeBufferMultiple(buffer.Length))
        {
            throw new ArgumentException("buffer did not originate from this memory manager. The size is not a multiple of " + LargeBufferMultiple);
        }
        int num = buffer.Length / largeBufferMultiple - 1;
        if (num < largePools.Length)
        {
            if ((largePools[num].Count + 1) * buffer.Length <= MaximumFreeLargePoolBytes || MaximumFreeLargePoolBytes == 0L)
            {
                largePools[num].Push(buffer);
                Interlocked.Add(ref largeBufferFreeSize[num], buffer.Length);
            }
            else
            {
                Events.Writer.MemoryStreamDiscardBuffer(Events.MemoryStreamBufferType.Large, tag, Events.MemoryStreamDiscardReason.EnoughFree);
                ReportLargeBufferDiscarded(Events.MemoryStreamDiscardReason.EnoughFree);
            }
        }
        else
        {
            num = largeBufferInUseSize.Length - 1;
            Events.Writer.MemoryStreamDiscardBuffer(Events.MemoryStreamBufferType.Large, tag, Events.MemoryStreamDiscardReason.TooLarge);
            ReportLargeBufferDiscarded(Events.MemoryStreamDiscardReason.TooLarge);
        }
        Interlocked.Add(ref largeBufferInUseSize[num], -buffer.Length);
        ReportUsageReport(smallPoolInUseSize, smallPoolFreeSize, LargePoolInUseSize, LargePoolFreeSize);
    }

    internal void ReturnBlocks(ICollection<byte[]> blocks, string tag)
    {
        if (blocks == null)
        {
            throw new ArgumentNullException("blocks");
        }
        int num = blocks.Count * BlockSize;
        Interlocked.Add(ref smallPoolInUseSize, -num);
        foreach (byte[] block in blocks)
        {
            if (block == null || block.Length != BlockSize)
            {
                throw new ArgumentException("blocks contains buffers that are not BlockSize in length");
            }
        }
        foreach (byte[] block2 in blocks)
        {
            if (MaximumFreeSmallPoolBytes != 0L && SmallPoolFreeSize >= MaximumFreeSmallPoolBytes)
            {
                Events.Writer.MemoryStreamDiscardBuffer(Events.MemoryStreamBufferType.Small, tag, Events.MemoryStreamDiscardReason.EnoughFree);
                ReportBlockDiscarded();
                break;
            }
            Interlocked.Add(ref smallPoolFreeSize, BlockSize);
            smallPool.Push(block2);
        }
        ReportUsageReport(smallPoolInUseSize, smallPoolFreeSize, LargePoolInUseSize, LargePoolFreeSize);
    }

    internal void ReportBlockCreated()
    {
        this.BlockCreated?.Invoke();
    }

    internal void ReportBlockDiscarded()
    {
        this.BlockDiscarded?.Invoke();
    }

    internal void ReportLargeBufferCreated()
    {
        this.LargeBufferCreated?.Invoke();
    }

    internal void ReportLargeBufferDiscarded(Events.MemoryStreamDiscardReason reason)
    {
        this.LargeBufferDiscarded?.Invoke(reason);
    }

    internal void ReportStreamCreated()
    {
        this.StreamCreated?.Invoke();
    }

    internal void ReportStreamDisposed()
    {
        this.StreamDisposed?.Invoke();
    }

    internal void ReportStreamFinalized()
    {
        this.StreamFinalized?.Invoke();
    }

    internal void ReportStreamLength(long bytes)
    {
        this.StreamLength?.Invoke(bytes);
    }

    internal void ReportStreamToArray()
    {
        this.StreamConvertedToArray?.Invoke();
    }

    internal void ReportUsageReport(long smallPoolInUseBytes, long smallPoolFreeBytes, long largePoolInUseBytes, long largePoolFreeBytes)
    {
        this.UsageReport?.Invoke(smallPoolInUseBytes, smallPoolFreeBytes, largePoolInUseBytes, largePoolFreeBytes);
    }

    public MemoryStream GetStream()
    {
        return new RecyclableMemoryStream(this);
    }

    public MemoryStream GetStream(string tag)
    {
        return new RecyclableMemoryStream(this, tag);
    }

    public MemoryStream GetStream(string tag, int requiredSize)
    {
        return new RecyclableMemoryStream(this, tag, requiredSize);
    }

    public MemoryStream GetStream(string tag, int requiredSize, bool asContiguousBuffer)
    {
        if (!asContiguousBuffer || requiredSize <= BlockSize)
        {
            return GetStream(tag, requiredSize);
        }
        return new RecyclableMemoryStream(this, tag, requiredSize, GetLargeBuffer(requiredSize, tag));
    }

    public MemoryStream GetStream(string tag, byte[] buffer, int offset, int count)
    {
        RecyclableMemoryStream recyclableMemoryStream = new RecyclableMemoryStream(this, tag, count);
        recyclableMemoryStream.Write(buffer, offset, count);
        recyclableMemoryStream.Position = 0L;
        return recyclableMemoryStream;
    }
}

