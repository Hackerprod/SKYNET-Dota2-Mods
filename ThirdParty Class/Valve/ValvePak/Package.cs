using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class Package : IDisposable
{
    public const int MAGIC = 1437209140;

    public const char DirectorySeparatorChar = '/';

    private BinaryReader Reader;

    private bool IsDirVPK;

    private uint HeaderSize;

    public string FileName
    {
        get;
        private set;
    }

    public uint Version
    {
        get;
        private set;
    }

    public uint TreeSize
    {
        get;
        private set;
    }

    public uint FileDataSectionSize
    {
        get;
        private set;
    }

    public uint ArchiveMD5SectionSize
    {
        get;
        private set;
    }

    public uint OtherMD5SectionSize
    {
        get;
        private set;
    }

    public uint SignatureSectionSize
    {
        get;
        private set;
    }

    public byte[] TreeChecksum
    {
        get;
        private set;
    }

    public byte[] ArchiveMD5EntriesChecksum
    {
        get;
        private set;
    }

    public byte[] WholeFileChecksum
    {
        get;
        private set;
    }

    public byte[] PublicKey
    {
        get;
        private set;
    }

    public byte[] Signature
    {
        get;
        private set;
    }

    public Dictionary<string, List<PackageEntry>> Entries
    {
        get;
        private set;
    }

    public List<ArchiveMD5SectionEntry> ArchiveMD5Entries
    {
        get;
        private set;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && Reader != null)
        {
            Reader.Dispose();
            Reader = null;
        }
    }

    public void SetFileName(string fileName)
    {
        if (fileName.EndsWith(".vpk", StringComparison.Ordinal))
        {
            fileName = fileName.Substring(0, fileName.Length - 4);
        }
        if (fileName.EndsWith("_dir", StringComparison.Ordinal))
        {
            IsDirVPK = true;
            fileName = fileName.Substring(0, fileName.Length - 4);
        }
        FileName = fileName;
    }

    public void Read(string filename)
    {
        SetFileName(filename);
        FileStream input = new FileStream(string.Format("{0}{1}.vpk", FileName, IsDirVPK ? "_dir" : string.Empty), FileMode.Open, FileAccess.Read, FileShare.Read);
        Read(input);
    }

    public void Read(Stream input)
    {
        if (FileName == null)
        {
            throw new InvalidOperationException("If you call Read() directly with a stream, you must call SetFileName() first.");
        }
        Reader = new BinaryReader(input);
        if (Reader.ReadUInt32() != 1437209140)
        {
            throw new InvalidDataException("Given file is not a VPK.");
        }
        Version = Reader.ReadUInt32();
        TreeSize = Reader.ReadUInt32();
        if (Version != 1)
        {
            if (Version != 2)
            {
                throw new InvalidDataException($"Bad VPK version. ({Version})");
            }
            FileDataSectionSize = Reader.ReadUInt32();
            ArchiveMD5SectionSize = Reader.ReadUInt32();
            OtherMD5SectionSize = Reader.ReadUInt32();
            SignatureSectionSize = Reader.ReadUInt32();
        }
        HeaderSize = (uint)input.Position;
        ReadEntries();
        if (Version == 2)
        {
            input.Position += FileDataSectionSize;
            ReadArchiveMD5Section();
            ReadOtherMD5Section();
            ReadSignatureSection();
        }
    }

    public PackageEntry FindEntry(string filePath)
    {
        filePath = filePath?.Replace('\\', '/');
        return FindEntry(Path.GetDirectoryName(filePath), filePath);
    }

    public PackageEntry FindEntry(string directory, string fileName)
    {
        fileName = fileName?.Replace('\\', '/');
        return FindEntry(directory, Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName)?.TrimStart('.'));
    }

    public PackageEntry FindEntry(string directory, string fileName, string extension)
    {
        if (extension == null)
        {
            extension = string.Empty;
        }
        if (!Entries.ContainsKey(extension))
        {
            return null;
        }
        directory = directory?.Replace('\\', '/').Trim('/');
        if (directory == string.Empty)
        {
            directory = null;
        }
        return Entries[extension].FirstOrDefault(delegate (PackageEntry x)
        {
            if (x.DirectoryName == directory)
            {
                return x.FileName == fileName;
            }
            return false;
        });
    }

    public void ReadEntry(PackageEntry entry, out byte[] output, bool validateCrc = true)
    {
        output = new byte[entry.SmallData.Length + entry.Length];
        if (entry.SmallData.Length != 0)
        {
            entry.SmallData.CopyTo(output, 0);
        }
        if (entry.Length != 0)
        {
            Stream stream = null;
            try
            {
                uint num = entry.Offset;
                if (entry.ArchiveIndex != 32767)
                {
                    if (!IsDirVPK)
                    {
                        throw new InvalidOperationException("Given VPK is not a _dir, but entry is referencing an external archive.");
                    }
                    stream = new FileStream($"{FileName}_{entry.ArchiveIndex:D3}.vpk", FileMode.Open, FileAccess.Read);
                }
                else
                {
                    stream = Reader.BaseStream;
                    num += HeaderSize + TreeSize;
                }
                stream.Seek(num, SeekOrigin.Begin);
                stream.Read(output, entry.SmallData.Length, (int)entry.Length);
            }
            finally
            {
                if (entry.ArchiveIndex != 32767)
                {
                    stream?.Close();
                }
            }
        }
        if (validateCrc && entry.CRC32 != Crc32.Compute(output))
        {
            throw new InvalidDataException("CRC32 mismatch for read data.");
        }
    }

    private void ReadEntries()
    {
        Dictionary<string, List<PackageEntry>> dictionary = new Dictionary<string, List<PackageEntry>>();
        while (true)
        {
            string text = Reader.ReadNullTermString(Encoding.UTF8);
            if (text == string.Empty)
            {
                break;
            }
            if (text == " ")
            {
                text = string.Empty;
            }
            List<PackageEntry> list = new List<PackageEntry>();
            while (true)
            {
                string text2 = Reader.ReadNullTermString(Encoding.UTF8);
                if (text2 == string.Empty)
                {
                    break;
                }
                if (text2 == " ")
                {
                    text2 = null;
                }
                while (true)
                {
                    string text3 = Reader.ReadNullTermString(Encoding.UTF8);
                    if (text3 == string.Empty)
                    {
                        break;
                    }
                    PackageEntry packageEntry = new PackageEntry
                    {
                        FileName = text3,
                        DirectoryName = text2,
                        TypeName = text,
                        CRC32 = Reader.ReadUInt32(),
                        SmallData = new byte[Reader.ReadUInt16()],
                        ArchiveIndex = Reader.ReadUInt16(),
                        Offset = Reader.ReadUInt32(),
                        Length = Reader.ReadUInt32()
                    };
                    if (Reader.ReadUInt16() != 65535)
                    {
                        throw new FormatException("Invalid terminator.");
                    }
                    if (packageEntry.SmallData.Length != 0)
                    {
                        Reader.Read(packageEntry.SmallData, 0, packageEntry.SmallData.Length);
                    }
                    list.Add(packageEntry);
                }
            }
            dictionary.Add(text, list);
        }
        Entries = dictionary;
    }

    public void VerifyHashes()
    {
        if (Version != 2)
        {
            throw new InvalidDataException("Only version 2 is supported.");
        }
        using (MD5 mD = MD5.Create())
        {
            Reader.BaseStream.Position = 0L;
            byte[] array = mD.ComputeHash(Reader.ReadBytes((int)(HeaderSize + TreeSize + FileDataSectionSize + ArchiveMD5SectionSize + 32)));
            if (!array.SequenceEqual(WholeFileChecksum))
            {
                throw new InvalidDataException($"Package checksum mismatch ({BitConverter.ToString(array)} != expected {BitConverter.ToString(WholeFileChecksum)})");
            }
            Reader.BaseStream.Position = HeaderSize;
            array = mD.ComputeHash(Reader.ReadBytes((int)TreeSize));
            if (!array.SequenceEqual(TreeChecksum))
            {
                throw new InvalidDataException($"File tree checksum mismatch ({BitConverter.ToString(array)} != expected {BitConverter.ToString(TreeChecksum)})");
            }
            Reader.BaseStream.Position = HeaderSize + TreeSize + FileDataSectionSize;
            array = mD.ComputeHash(Reader.ReadBytes((int)ArchiveMD5SectionSize));
            if (!array.SequenceEqual(ArchiveMD5EntriesChecksum))
            {
                throw new InvalidDataException($"Archive MD5 entries checksum mismatch ({BitConverter.ToString(array)} != expected {BitConverter.ToString(ArchiveMD5EntriesChecksum)})");
            }
        }
        if (PublicKey == null || Signature == null || IsSignatureValid())
        {
            return;
        }
        throw new InvalidDataException("VPK signature is not valid.");
    }

    public bool IsSignatureValid()
    {
        Reader.BaseStream.Position = 0L;
        AsnKeyParser asnKeyParser = new AsnKeyParser(PublicKey);
        RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
        rSACryptoServiceProvider.ImportParameters(asnKeyParser.ParseRSAPublicKey());
        RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
        rSAPKCS1SignatureDeformatter.SetHashAlgorithm("SHA256");
        byte[] rgbHash = new SHA256Managed().ComputeHash(Reader.ReadBytes((int)(HeaderSize + TreeSize + FileDataSectionSize + ArchiveMD5SectionSize + OtherMD5SectionSize)));
        return rSAPKCS1SignatureDeformatter.VerifySignature(rgbHash, Signature);
    }

    private void ReadArchiveMD5Section()
    {
        ArchiveMD5Entries = new List<ArchiveMD5SectionEntry>();
        if (ArchiveMD5SectionSize != 0)
        {
            uint num = ArchiveMD5SectionSize / 28u;
            for (int i = 0; i < num; i++)
            {
                ArchiveMD5Entries.Add(new ArchiveMD5SectionEntry
                {
                    ArchiveIndex = Reader.ReadUInt32(),
                    Offset = Reader.ReadUInt32(),
                    Length = Reader.ReadUInt32(),
                    Checksum = Reader.ReadBytes(16)
                });
            }
        }
    }

    private void ReadOtherMD5Section()
    {
        if (OtherMD5SectionSize != 48)
        {
            throw new InvalidDataException($"Encountered OtherMD5Section with size of {OtherMD5SectionSize} (should be 48)");
        }
        TreeChecksum = Reader.ReadBytes(16);
        ArchiveMD5EntriesChecksum = Reader.ReadBytes(16);
        WholeFileChecksum = Reader.ReadBytes(16);
    }

    private void ReadSignatureSection()
    {
        if (SignatureSectionSize != 0)
        {
            int count = Reader.ReadInt32();
            PublicKey = Reader.ReadBytes(count);
            int count2 = Reader.ReadInt32();
            Signature = Reader.ReadBytes(count2);
        }
    }
}

