using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public sealed class RecyclableMemoryStream : MemoryStream
{
    private struct BlockAndOffset
    {
        public int Block;

        public int Offset;

        public BlockAndOffset(int block, int offset)
        {
            Block = block;
            Offset = offset;
        }
    }

    private const long MaxStreamLength = 2147483647L;

    private static readonly byte[] emptyArray = new byte[0];

    private readonly List<byte[]> blocks = new List<byte[]>(1);

    private readonly byte[] byteBuffer = new byte[1];

    private readonly Guid id;

    private readonly RecyclableMemoryStreamManager memoryManager;

    private readonly string tag;

    private List<byte[]> dirtyBuffers;

    private long disposedState;

    private byte[] largeBuffer;

    private int length;

    private int position;

    internal Guid Id
    {
        get
        {
            CheckDisposed();
            return id;
        }
    }

    internal string Tag
    {
        get
        {
            CheckDisposed();
            return tag;
        }
    }

    internal RecyclableMemoryStreamManager MemoryManager
    {
        get
        {
            CheckDisposed();
            return memoryManager;
        }
    }

    internal string AllocationStack
    {
        get;
    }

    internal string DisposeStack
    {
        get;
        private set;
    }

    public override int Capacity
    {
        get
        {
            CheckDisposed();
            if (largeBuffer != null)
            {
                return largeBuffer.Length;
            }
            long val = (long)blocks.Count * (long)memoryManager.BlockSize;
            return (int)Math.Min(2147483647L, val);
        }
        set
        {
            CheckDisposed();
            EnsureCapacity(value);
        }
    }

    public override long Length
    {
        get
        {
            CheckDisposed();
            return length;
        }
    }

    public override long Position
    {
        get
        {
            CheckDisposed();
            return position;
        }
        set
        {
            CheckDisposed();
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", "value must be non-negative");
            }
            if (value > 2147483647)
            {
                throw new ArgumentOutOfRangeException("value", "value cannot be more than " + 2147483647L);
            }
            position = (int)value;
        }
    }

    public override bool CanRead => !Disposed;

    public override bool CanSeek => !Disposed;

    public override bool CanTimeout => false;

    public override bool CanWrite => !Disposed;

    private bool Disposed => Interlocked.Read(ref disposedState) != 0;

    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager)
        : this(memoryManager, null, 0, null)
    {
    }

    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, string tag)
        : this(memoryManager, tag, 0, null)
    {
    }

    public RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, string tag, int requestedSize)
        : this(memoryManager, tag, requestedSize, null)
    {
    }

    internal RecyclableMemoryStream(RecyclableMemoryStreamManager memoryManager, string tag, int requestedSize, byte[] initialLargeBuffer)
        : base(emptyArray)
    {
        this.memoryManager = memoryManager;
        id = Guid.NewGuid();
        this.tag = tag;
        if (requestedSize < memoryManager.BlockSize)
        {
            requestedSize = memoryManager.BlockSize;
        }
        if (initialLargeBuffer == null)
        {
            EnsureCapacity(requestedSize);
        }
        else
        {
            largeBuffer = initialLargeBuffer;
        }
        if (this.memoryManager.GenerateCallStacks)
        {
            AllocationStack = Environment.StackTrace;
        }
        RecyclableMemoryStreamManager.Events.Writer.MemoryStreamCreated(id, this.tag, requestedSize);
        this.memoryManager.ReportStreamCreated();
    }

    ~RecyclableMemoryStream()
    {
        Dispose(disposing: false);
    }

    protected override void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref disposedState, 1L, 0L) != 0L)
        {
            string disposeStack = null;
            if (memoryManager.GenerateCallStacks)
            {
                disposeStack = Environment.StackTrace;
            }
            RecyclableMemoryStreamManager.Events.Writer.MemoryStreamDoubleDispose(id, tag, AllocationStack, DisposeStack, disposeStack);
        }
        else
        {
            RecyclableMemoryStreamManager.Events.Writer.MemoryStreamDisposed(id, tag);
            if (memoryManager.GenerateCallStacks)
            {
                DisposeStack = Environment.StackTrace;
            }
            if (disposing)
            {
                memoryManager.ReportStreamDisposed();
                GC.SuppressFinalize(this);
            }
            else
            {
                RecyclableMemoryStreamManager.Events.Writer.MemoryStreamFinalized(id, tag, AllocationStack);
                if (AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    base.Dispose(disposing);
                    return;
                }
                memoryManager.ReportStreamFinalized();
            }
            memoryManager.ReportStreamLength(length);
            if (largeBuffer != null)
            {
                memoryManager.ReturnLargeBuffer(largeBuffer, tag);
            }
            if (dirtyBuffers != null)
            {
                foreach (byte[] dirtyBuffer in dirtyBuffers)
                {
                    memoryManager.ReturnLargeBuffer(dirtyBuffer, tag);
                }
            }
            memoryManager.ReturnBlocks(blocks, tag);
            blocks.Clear();
            base.Dispose(disposing);
        }
    }

    public override void Close()
    {
        Dispose(disposing: true);
    }

    public override byte[] GetBuffer()
    {
        CheckDisposed();
        if (largeBuffer != null)
        {
            return largeBuffer;
        }
        if (blocks.Count == 1)
        {
            return blocks[0];
        }
        byte[] buffer = memoryManager.GetLargeBuffer(Capacity, tag);
        InternalRead(buffer, 0, length, 0);
        largeBuffer = buffer;
        if (blocks.Count > 0 && memoryManager.AggressiveBufferReturn)
        {
            memoryManager.ReturnBlocks(blocks, tag);
            blocks.Clear();
        }
        return largeBuffer;
    }

    [Obsolete("This method has degraded performance vs. GetBuffer and should be avoided.")]
    public override byte[] ToArray()
    {
        CheckDisposed();
        byte[] array = new byte[Length];
        InternalRead(array, 0, length, 0);
        string stack = memoryManager.GenerateCallStacks ? Environment.StackTrace : null;
        RecyclableMemoryStreamManager.Events.Writer.MemoryStreamToArray(id, tag, stack, 0);
        memoryManager.ReportStreamToArray();
        return array;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return SafeRead(buffer, offset, count, ref position);
    }

    public int SafeRead(byte[] buffer, int offset, int count, ref int streamPosition)
    {
        CheckDisposed();
        if (buffer == null)
        {
            throw new ArgumentNullException("buffer");
        }
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException("offset", "offset cannot be negative");
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException("count", "count cannot be negative");
        }
        if (offset + count > buffer.Length)
        {
            throw new ArgumentException("buffer length must be at least offset + count");
        }
        int num = InternalRead(buffer, offset, count, streamPosition);
        streamPosition += num;
        return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        CheckDisposed();
        if (buffer == null)
        {
            modCommon.WriteLine("Error in RecyclableMemoryStream: buffer");
        }
        if (offset < 0)
        {
            modCommon.WriteLine("Error in RecyclableMemoryStream: offset " +  offset + "Offset must be in the range of 0 - buffer.Length-1");
        }
        if (count < 0)
        {
            modCommon.WriteLine("Error in RecyclableMemoryStream: count " + count + "count must be non-negative");
        }
        if (count + offset > buffer.Length)
        {
            modCommon.WriteLine("Error in RecyclableMemoryStream: count must be greater than buffer.Length - offset");
        }
        int blockSize = memoryManager.BlockSize;
        long num = (long)position + (long)count;
        if (num > 2147483647)
        {
            modCommon.WriteLine("Error in RecyclableMemoryStream: Maximum capacity exceeded");
        }
        if ((num + blockSize - 1) / blockSize * blockSize > 2147483647)
        {
            modCommon.WriteLine("Error in RecyclableMemoryStream: Maximum capacity exceeded");
        }
        EnsureCapacity((int)num);
        if (largeBuffer == null)
        {
            int num2 = count;
            int num3 = 0;
            BlockAndOffset blockAndRelativeOffset = GetBlockAndRelativeOffset(position);
            while (num2 > 0)
            {
                byte[] dst = blocks[blockAndRelativeOffset.Block];
                int num4 = Math.Min(blockSize - blockAndRelativeOffset.Offset, num2);
                Buffer.BlockCopy(buffer, offset + num3, dst, blockAndRelativeOffset.Offset, num4);
                num2 -= num4;
                num3 += num4;
                blockAndRelativeOffset.Block++;
                blockAndRelativeOffset.Offset = 0;
            }
        }
        else
        {
            Buffer.BlockCopy(buffer, offset, largeBuffer, position, count);
        }
        position = (int)num;
        length = Math.Max(position, length);
    }

    public override string ToString()
    {
        return $"Id = {Id}, Tag = {Tag}, Length = {Length:N0} bytes";
    }

    public override void WriteByte(byte value)
    {
        CheckDisposed();
        byteBuffer[0] = value;
        Write(byteBuffer, 0, 1);
    }

    public override int ReadByte()
    {
        return SafeReadByte(ref position);
    }

    public int SafeReadByte(ref int streamPosition)
    {
        CheckDisposed();
        if (streamPosition == length)
        {
            return -1;
        }
        byte result;
        if (largeBuffer == null)
        {
            BlockAndOffset blockAndRelativeOffset = GetBlockAndRelativeOffset(streamPosition);
            result = blocks[blockAndRelativeOffset.Block][blockAndRelativeOffset.Offset];
        }
        else
        {
            result = largeBuffer[streamPosition];
        }
        streamPosition++;
        return result;
    }

    public override void SetLength(long value)
    {
        CheckDisposed();
        if (value < 0 || value > 2147483647)
        {
            throw new ArgumentOutOfRangeException("value", "value must be non-negative and at most " + 2147483647L);
        }
        EnsureCapacity((int)value);
        length = (int)value;
        if (position > value)
        {
            position = (int)value;
        }
    }

    public override long Seek(long offset, SeekOrigin loc)
    {
        CheckDisposed();
        if (offset > 2147483647)
        {
            throw new ArgumentOutOfRangeException("offset", "offset cannot be larger than " + 2147483647L);
        }
        int num;
        switch (loc)
        {
            case SeekOrigin.Begin:
                num = (int)offset;
                break;
            case SeekOrigin.Current:
                num = (int)offset + position;
                break;
            case SeekOrigin.End:
                num = (int)offset + length;
                break;
            default:
                throw new ArgumentException("Invalid seek origin", "loc");
        }
        if (num < 0)
        {
            throw new IOException("Seek before beginning");
        }
        position = num;
        return position;
    }

    public override void WriteTo(Stream stream)
    {
        CheckDisposed();
        if (stream == null)
        {
            throw new ArgumentNullException("stream");
        }
        if (largeBuffer == null)
        {
            int num = 0;
            int num2 = length;
            while (num2 > 0)
            {
                int num3 = Math.Min(blocks[num].Length, num2);
                stream.Write(blocks[num], 0, num3);
                num2 -= num3;
                num++;
            }
        }
        else
        {
            stream.Write(largeBuffer, 0, length);
        }
    }

    private void CheckDisposed()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException($"The stream with Id {id} and Tag {tag} is disposed.");
        }
    }

    private int InternalRead(byte[] buffer, int offset, int count, int fromPosition)
    {
        if (length - fromPosition <= 0)
        {
            return 0;
        }
        int num3;
        if (largeBuffer == null)
        {
            BlockAndOffset blockAndRelativeOffset = GetBlockAndRelativeOffset(fromPosition);
            int num = 0;
            int num2 = Math.Min(count, length - fromPosition);
            while (num2 > 0)
            {
                num3 = Math.Min(blocks[blockAndRelativeOffset.Block].Length - blockAndRelativeOffset.Offset, num2);
                Buffer.BlockCopy(blocks[blockAndRelativeOffset.Block], blockAndRelativeOffset.Offset, buffer, num + offset, num3);
                num += num3;
                num2 -= num3;
                blockAndRelativeOffset.Block++;
                blockAndRelativeOffset.Offset = 0;
            }
            return num;
        }
        num3 = Math.Min(count, length - fromPosition);
        Buffer.BlockCopy(largeBuffer, fromPosition, buffer, offset, num3);
        return num3;
    }

    private BlockAndOffset GetBlockAndRelativeOffset(int offset)
    {
        int blockSize = memoryManager.BlockSize;
        return new BlockAndOffset(offset / blockSize, offset % blockSize);
    }

    private void EnsureCapacity(int newCapacity)
    {
        if (newCapacity > memoryManager.MaximumStreamCapacity && memoryManager.MaximumStreamCapacity > 0)
        {
            RecyclableMemoryStreamManager.Events.Writer.MemoryStreamOverCapacity(newCapacity, memoryManager.MaximumStreamCapacity, tag, AllocationStack);
            throw new InvalidOperationException("Requested capacity is too large: " + newCapacity + ". Limit is " + memoryManager.MaximumStreamCapacity);
        }
        if (largeBuffer != null)
        {
            if (newCapacity > largeBuffer.Length)
            {
                byte[] buffer = memoryManager.GetLargeBuffer(newCapacity, tag);
                InternalRead(buffer, 0, length, 0);
                ReleaseLargeBuffer();
                largeBuffer = buffer;
            }
        }
        else
        {
            while (Capacity < newCapacity)
            {
                blocks.Add(memoryManager.GetBlock());
            }
        }
    }

    private void ReleaseLargeBuffer()
    {
        if (memoryManager.AggressiveBufferReturn)
        {
            memoryManager.ReturnLargeBuffer(largeBuffer, tag);
        }
        else
        {
            if (dirtyBuffers == null)
            {
                dirtyBuffers = new List<byte[]>(1);
            }
            dirtyBuffers.Add(largeBuffer);
        }
        largeBuffer = null;
    }
}
