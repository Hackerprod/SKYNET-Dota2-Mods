using System.Diagnostics;
using System.IO;
using System.Text;

[DebuggerDisplay("Name = {Name}")]
public class VpkNode
{
    private const uint Terminator = 65535u;

    public const uint DirectoryArchiveIndex = 32767u;

    private readonly VpkNode parent;

    public VpkNode Parent => parent;

    public string FilePath
    {
        get
        {
            if (parent != null && parent.Parent != null)
            {
                string name = Name;
                string name2 = parent.Name;
                string name3 = parent.Parent.Name;
                return $"{name2}/{name}.{name3}";
            }
            return null;
        }
    }

    public string Name
    {
        get;
        private set;
    }

    public uint Crc
    {
        get;
        private set;
    }

    public short PreloadBytes
    {
        get;
        private set;
    }

    public short ArchiveIndex
    {
        get;
        private set;
    }

    public uint EntryOffset
    {
        get;
        private set;
    }

    public uint EntryLength
    {
        get;
        private set;
    }

    public byte[] PreloadData
    {
        get;
        private set;
    }

    public VpkNode[] Children
    {
        get;
        internal set;
    }

    public VpkNode()
    {
    }

    public VpkNode(VpkNode parent)
    {
        this.parent = parent;
    }

    internal void Load(BinaryReader reader)
    {
        StringBuilder stringBuilder = new StringBuilder();
        char c;
        do
        {
            c = reader.ReadChar();
            stringBuilder.Append(c);
        }
        while (c != 0);
        Name = stringBuilder.ToString().TrimEnd(default(char));
    }

    internal void LoadFileInfo(BinaryReader reader)
    {
        Load(reader);
        if (!string.IsNullOrEmpty(Name))
        {
            Crc = reader.ReadUInt32();
            PreloadBytes = reader.ReadInt16();
            ArchiveIndex = reader.ReadInt16();
            EntryOffset = reader.ReadUInt32();
            EntryLength = reader.ReadUInt32();
            ushort num = reader.ReadUInt16();
            if (num != 65535)
            {
                throw new InvalidDataException("Error: VPK entry did not end with correct terminator");
            }
            if (PreloadBytes > 0)
            {
                PreloadData = reader.ReadBytes(PreloadBytes);
            }
        }
    }
}