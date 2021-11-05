using System;
using System.IO;
using System.Linq;

public static class VPKUtil
{
    public static Stream GetInputStream(string vpkDirFileName, VpkNode node)
    {
        if (node.EntryLength == 0 && node.PreloadBytes > 0)
        {
            return new MemoryStream(node.PreloadData);
        }
        if (node.PreloadBytes != 0)
        {
            throw new NotSupportedException("Unable to get entry data: Both EntryLength and PreloadBytes specified.");
        }
        string text = new string(Enumerable.Repeat('0', 3 - node.ArchiveIndex.ToString().Length).ToArray());
        string path = vpkDirFileName.Replace("_dir.vpk", "_" + text + node.ArchiveIndex + ".vpk");
        FileStream fileStream = new FileStream(path, FileMode.Open);
        fileStream.Seek(node.EntryOffset, SeekOrigin.Begin);
        return fileStream;
    }
}
