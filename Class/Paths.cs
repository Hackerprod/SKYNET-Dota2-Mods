using System;
using System.IO;

public static class Paths
{
    public const string AppInfoKv = "appinfo";

    public const string AppNameKv = "common/name";

    public const string AppTypeKv = "common/type";

    public const string AppDepotsKv = "depots";

    public const string AppListOfDlcKv = "extended/listofdlc";

    public const string AppDepotsManifestIdKv = "manifests/public";

    public const string DepotFromAppKv = "depotfromapp";

    public const string AppInstallDirKv = "config/installdir";

    public const string SharedInstallKv = "sharedinstall";

    public const string AppDepotNameKv = "name";

    public static string CurrentDirectory
    {
        get;
    }

    public static string ServerBasePath
    {
        get;
        private set;
    }

    public static string ServerRawBasePath
    {
        get;
        private set;
    }

    public static string ContentBasePath
    {
        get;
        private set;
    }

    public static string ContentRawBasePath
    {
        get;
        private set;
    }

    public static string AppCachePath
    {
        get;
        private set;
    }

    public static string AppCachePatchesPath
    {
        get;
        private set;
    }

    public static string ServerAchivementSchemasPath
    {
        get;
        private set;
    }

    public static string ClientConfigurationsPath
    {
        get;
        private set;
    }

    public static string WwwBasePath
    {
        get;
        private set;
    }

    public static string CloudBasePath
    {
        get;
        private set;
    }

    public static string RichPresencesBasePath
    {
        get;
        private set;
    }

    static Paths()
    {
        CurrentDirectory = Directory.GetCurrentDirectory();
        ServerRawBasePath = string.Empty;
        ContentRawBasePath = string.Empty;
        try
        {
            SetBasePath(CurrentDirectory);
            string path;
            string pathFromFile = GetPathFromFile(Path.Combine(CurrentDirectory, "__basepath.txt"), out path);
            ServerRawBasePath = path;
            if (pathFromFile != null)
            {
                EnsureDirectory(pathFromFile);
                SetBasePath(pathFromFile);
            }
        }
        catch
        {
        }
        try
        {
            string path2;
            string pathFromFile2 = GetPathFromFile(Path.Combine(CurrentDirectory, "__contentpath.txt"), out path2);
            ContentRawBasePath = path2;
            if (pathFromFile2 != null)
            {
                EnsureDirectory(pathFromFile2);
                SetContentBasePath(pathFromFile2);
            }
        }
        catch
        {
        }
        try
        {
            EnsureDirectory(RichPresencesBasePath);
        }
        catch
        {
        }
    }

    private static string GetPathFromFile(string file, out string path)
    {
        path = string.Empty;
        if (File.Exists(file))
        {
            path = File.ReadAllText(file);
            if (!Path.IsPathRooted(path))
            {
                return Path.GetFullPath(Path.Combine(CurrentDirectory, path));
            }
            return path;
        }
        return null;
    }

    public static void SetBasePath(string basePath)
    {
        ServerBasePath = basePath;
        WwwBasePath = Path.Combine(ServerBasePath, "data", "www");
        CloudBasePath = Path.Combine(WwwBasePath, "cloud");
        ContentBasePath = Path.Combine(WwwBasePath, "content");
        AppCachePath = Path.Combine(ServerBasePath, "data", "appcache");
        AppCachePatchesPath = Path.Combine(ServerBasePath, "data", "appcache", "patches");
        ServerAchivementSchemasPath = Path.Combine(ServerBasePath, "data", "appcache", "stats");
        ClientConfigurationsPath = Path.Combine(WwwBasePath, "client-config");
        RichPresencesBasePath = Path.Combine(ServerBasePath, "data", "rich_presence");
    }

    public static void SetContentBasePath(string basePath)
    {
        ContentBasePath = Path.Combine(basePath);
    }

    public static void SaveBasePath(string path)
    {
        try
        {
            string path2 = Path.Combine(CurrentDirectory, "__basepath.txt");
            if (string.IsNullOrEmpty(path))
            {
                if (File.Exists(path2))
                {
                    File.Delete(path2);
                }
            }
            else
            {
                File.WriteAllText(path2, path);
            }
        }
        catch
        {
        }
    }

    public static void SaveContentBasePath(string path)
    {
        try
        {
            string path2 = Path.Combine(CurrentDirectory, "__contentpath.txt");
            if (string.IsNullOrEmpty(path))
            {
                if (File.Exists(path2))
                {
                    File.Delete(path2);
                }
            }
            else
            {
                File.WriteAllText(path2, path);
            }
        }
        catch
        {
        }
    }

    public static bool EnsureDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return false;
        }
        Directory.CreateDirectory(path);
        return true;
    }
    public static bool EnsureSettingsFile(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        File.WriteAllText(path, "[SkynetDota2Mods]");
        return true;
    }
    public static void CleanDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (FileInfo item in directoryInfo.EnumerateFiles())
            {
                try { item.Delete(); } catch { }
            }
            foreach (DirectoryInfo item2 in directoryInfo.EnumerateDirectories())
            {
                try { item2.Delete(recursive: true); } catch { }
            }
        }
    }

    public static string GetEncryptedDepotPath(uint depotId)
    {
        return Path.Combine(WwwBasePath, "content-steam", depotId.ToString());
    }

    public static string GetDepotChunksPath(uint depotId)
    {
        return Path.Combine(WwwBasePath, "content-steam", depotId.ToString(), "chunks");
    }

    public static string GetDecryptedDepotName(uint depotId, ulong manifestId)
    {
        return $"{depotId}_{manifestId}.manifest";
    }

    public static string GetContentAppBasePath(uint appId)
    {
        return Path.Combine(ContentBasePath, appId.ToString());
    }

    public static string GetContentAppPath(uint appId)
    {
        return Path.Combine(ContentBasePath, appId.ToString(), "content");
    }

    public static string GetContentAppBackupPath(uint appId)
    {
        return Path.Combine(ContentBasePath, appId.ToString(), "content_backup");
    }

    public static string GetDepotsAppPath(uint appId)
    {
        return Path.Combine(ContentBasePath, appId.ToString(), "depots");
    }

    public static string GetAppAchivementSchemaName(uint appId)
    {
        return $"UserGameStatsSchema_{appId}.bin";
    }

    public static string GetServerAchivementSchemaFilePath(uint appId)
    {
        return Path.Combine(ServerAchivementSchemasPath, GetAppAchivementSchemaName(appId));
    }

    public static string GetItemsDefinitions(uint appId)
    {
        return Path.Combine(ServerBasePath, "appcache", "items", $"{appId}_defmeta.json");
    }

    public static string GetRichPresencePath(uint appId)
    {
        return Path.Combine(RichPresencesBasePath, appId.ToString());
    }

    public static string GetRichPresenceFile(uint appId, string language)
    {
        return Path.Combine(RichPresencesBasePath, appId.ToString(), language + ".vdf");
    }

    internal static string GetCurrentDirectoryName(string dir)
    {
        string[] directories = dir.Replace(@"\", "/").Split('/');
        return directories[directories.Length - 1];
    }

    internal static string GetParent(string path)
    {
        path = path.Replace(@"\", "/");
        string CustomPath = "";
        string result = "";
        for (int i = 0; i < path.Length; i++)
        {

            if (i != path.Length - 1)
                CustomPath += path[i].ToString();
        }

        string[] folders = CustomPath.Split('/');
        for (int i = 0; i < folders.Length; i++)
        {
            if (i != folders.Length - 1)
            {
                result += folders[i] + "/";
            }
        }
        return result;
    }
}

