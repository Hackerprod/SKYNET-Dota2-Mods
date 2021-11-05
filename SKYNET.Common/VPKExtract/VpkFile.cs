using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class VpkFile : IDisposable
{
    private const uint Magic = 1437209140u;

    private readonly string filename;

    private Stream fileStream;

    private uint treeLength;

    private List<VpkNode> nodes;

    public uint Version
    {
        get;
        private set;
    }

    public uint DataOffset
    {
        get
        {
            switch (Version)
            {
                case 1u:
                    return 12u;
                case 2u:
                    return 28u;
                default:
                    throw new InvalidOperationException("Called DataOffset on a VpkFile with unknown version");
            }
        }
    }

    public VpkFile(string filename)
    {
        this.filename = filename;
    }

    public void Open()
    {
        fileStream = File.OpenRead(filename);
        fileStream.Seek(0L, SeekOrigin.Begin);
        using (BinaryReader reader = new BinaryReader(fileStream, Encoding.UTF8, leaveOpen: true))
        {
            Load(reader);
        }
    }

    private void Load(BinaryReader reader)
    {
        uint num = reader.ReadUInt32();
        if (num != 1437209140)
        {
            throw new InvalidDataException("Incorrect magic");
        }
        Version = reader.ReadUInt32();
        if (Version < 1 || Version > 2)
        {
            throw new InvalidDataException("Unknown version");
        }
        switch (Version)
        {
            case 1u:
                LoadVersion1Header(reader);
                break;
            case 2u:
                LoadVersion2Header(reader);
                break;
            default:
                throw new InvalidOperationException("I got lost.");
        }
        nodes = LoadRootNodes(reader);
    }

    private static List<VpkNode> LoadRootNodes(BinaryReader reader)
    {
        List<VpkNode> list = new List<VpkNode>();
        VpkNode vpkNode = null;
        while (vpkNode == null || !string.IsNullOrEmpty(vpkNode.Name))
        {
            vpkNode = new VpkNode();
            vpkNode.Load(reader);
            if (!string.IsNullOrEmpty(vpkNode.Name))
            {
                list.Add(vpkNode);
                vpkNode.Children = LoadNodeChildren(reader, vpkNode);
            }
        }
        return list;
    }

    private static VpkNode[] LoadNodeChildren(BinaryReader reader, VpkNode parent)
    {
        List<VpkNode> list = new List<VpkNode>();
        VpkNode vpkNode = null;
        while (vpkNode == null || !string.IsNullOrEmpty(vpkNode.Name))
        {
            vpkNode = new VpkNode(parent);
            vpkNode.Load(reader);
            if (!string.IsNullOrEmpty(vpkNode.Name))
            {
                list.Add(vpkNode);
                vpkNode.Children = LoadNodeFileChildren(reader, vpkNode);
            }
        }
        return list.ToArray();
    }

    private static VpkNode[] LoadNodeFileChildren(BinaryReader reader, VpkNode parent)
    {
        List<VpkNode> list = new List<VpkNode>();
        VpkNode vpkNode = null;
        while (vpkNode == null || !string.IsNullOrEmpty(vpkNode.Name))
        {
            vpkNode = new VpkNode(parent);
            vpkNode.LoadFileInfo(reader);
            if (!string.IsNullOrEmpty(vpkNode.Name))
            {
                list.Add(vpkNode);
            }
        }
        return list.ToArray();
    }

    private void LoadVersion1Header(BinaryReader reader)
    {
        treeLength = reader.ReadUInt32();
    }

    private void LoadVersion2Header(BinaryReader reader)
    {
        treeLength = reader.ReadUInt32();
        int num = reader.ReadInt32();
        uint num2 = reader.ReadUInt32();
        int num3 = reader.ReadInt32();
        int num4 = reader.ReadInt32();
    }

    public void Close()
    {
        if (fileStream != null)
        {
            fileStream.Close();
            fileStream.Dispose();
        }
    }

    public VpkNode GetFile(string name)
    {
        IEnumerable<VpkNode> source = from node in nodes
                                      from dir in node.Children
                                      from fileEntry in dir.Children
                                      where fileEntry.FilePath == name
                                      select fileEntry;
        return source.SingleOrDefault();
    }

    public VpkNode[] GetAllFilesInDirectoryAndSubdirectories(string name)
    {
        IEnumerable<VpkNode[]> source = from node in nodes
                                        from dir in node.Children
                                        where dir.Name == name || dir.Name.StartsWith(name + "/")
                                        select dir.Children;
        return source.SelectMany((VpkNode[] x) => x).ToArray();
    }

    public void Dispose()
    {
        Close();
    }
}