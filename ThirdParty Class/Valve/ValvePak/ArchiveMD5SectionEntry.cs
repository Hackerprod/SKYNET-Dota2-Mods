using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ArchiveMD5SectionEntry
{
    public uint ArchiveIndex
    {
        get;
        set;
    }

    public uint Offset
    {
        get;
        set;
    }

    public uint Length
    {
        get;
        set;
    }

    public byte[] Checksum
    {
        get;
        set;
    }
}
