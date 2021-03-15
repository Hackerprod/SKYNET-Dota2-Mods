using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class FileManager
{
    public static void renameFile(string filename, string FileNameWithoutExtension)
    {
        if (File.Exists(filename))
        {
            string newFN = filename.Replace(GetFileNameWithoutExtension(filename), FileNameWithoutExtension);
            try { File.Copy(filename, newFN); } catch { return; }
        }
        try
        {
            new FileInfo(filename).Attributes = FileAttributes.Normal;
            File.Delete(filename);
        }
        catch { }
    }
    public static void deleteFile(string filename)
    {
        if (File.Exists(filename))
        {
            try
            {
                new FileInfo(filename).Attributes = FileAttributes.Normal;
                File.Delete(filename);
            }
            catch { }
        }
    }
    public static void DeleteDirectory(string fullPath)
    {
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath)
            {
                Attributes = FileAttributes.Normal
            };
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories);
            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
            {
                if (fileSystemInfo is FileInfo)
                {
                    fileSystemInfo.Attributes = FileAttributes.Normal;
                }
            }
            Thread.Sleep(100);
            directoryInfo.Delete(recursive: true);
        }

    }
    public static bool FileExist(string filename)
    {
        if (File.Exists(filename))
            return true;
        return false;
    }
    public static bool DirectoryExist(string Directoryname)
    {
        if (Directory.Exists(Directoryname))
            return true;
        return false;
    }
    public static string GetFileName(string FilePath)
    {
        return Path.GetFileName(FilePath);
    }
    public static string GetFileNameWithoutExtension(string FilePath)
    {
        return Path.GetFileNameWithoutExtension(FilePath);
    }
    public static string GetExtension(string FilePath)
    {
        return Path.GetExtension(FilePath);
    }
    public static string GetFileLength(string FilePath)
    {
        FileInfo info = new FileInfo(FilePath);
        return info.Length.ToString();
    }
    public static void FileCopy(string source, string target)
    {
        if (File.Exists(source))
        {
            try { File.Copy(source, target); } catch { }
        }
    }
    public static void FileMove(string source, string target)
    {
        if (File.Exists(target))
        {
            try
            {
                new FileInfo(target).Attributes = FileAttributes.Normal;
                File.Delete(target);
            }
            catch
            {
                modCommon.Show("No se ha podido eliminar el mod en el directorio:" + Environment.NewLine + target);
            }
        }
        if (File.Exists(source))
        {
            try
            {
                File.Copy(source, target, true);
            }
            catch
            {
                modCommon.Show("No se ha podido copiar el mod al directorio:" + Environment.NewLine + target);
            }
        }
        try
        {
            new FileInfo(source).Attributes = FileAttributes.Normal;
            File.Delete(source);
        }
        catch { }
    }
    internal static void HideDirectory(string Path)
    {
        try
        {
            DirectoryInfo info = new DirectoryInfo(Path);
            info.Attributes = FileAttributes.Hidden;
        }
        catch { }
    }

    internal static void HideFile(string file)
    {
        try
        {
            FileInfo inf = new FileInfo(file);
            inf.Attributes = FileAttributes.Hidden;
        }
        catch { }
    }

    public static bool IsFolderUserAccessible(string path)
    {
        try
        {
            AuthorizationRuleCollection accessRules = new DirectoryInfo(path).GetAccessControl().GetAccessRules(true, true, typeof(NTAccount));
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            SecurityIdentifier right = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            foreach (AuthorizationRule item in accessRules)
            {
                FileSystemAccessRule fileSystemAccessRule = item as FileSystemAccessRule;
                if (fileSystemAccessRule != null && (fileSystemAccessRule.FileSystemRights & FileSystemRights.WriteData) > (FileSystemRights)0)
                {
                    try
                    {
                        string value = item.IdentityReference.Translate(typeof(NTAccount)).Value;
                        IdentityReference left = item.IdentityReference.Translate(typeof(SecurityIdentifier));
                        if (value.Equals(current.Name) || left == right)
                        {
                            return true;
                        }
                    }
                    catch (IdentityNotMappedException)
                    {
                    }
                }
            }
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public static Encoding GetEncoding(string filename)
    {
        // Read the BOM
        var bom = new byte[4];
        using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            file.Read(bom, 0, 4);
        }

        // Analyze the BOM
        if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
        return Encoding.ASCII;
    }

    public static void MakeFolderUserAccessible(string directoryName)
    {
        ModifyFolderUsersAndPermissions(directoryName, WindowsIdentity.GetCurrent().Name, FileSystemRights.FullControl, AccessControlType.Allow);
    }
    public static void MakeFolderAllUsersAccessible(string directoryName)
    {
        try
        {
            var path = string.Format("WinNT://{0},computer", Environment.MachineName);
            using (var userget = new DirectoryEntry(path))
            {
                var userNames = from DirectoryEntry dirchild in userget.Children
                                where dirchild.SchemaClassName == "User"
                                select dirchild.Name;
                foreach (var UserName in userNames)
                {
                    ModifyFolderUsersAndPermissions(directoryName, UserName, FileSystemRights.FullControl,
                        AccessControlType.Allow);
                }
            }
        }
        catch { }
    }

    private static void ModifyFolderUsersAndPermissions(string directoryName, string userAccount, FileSystemRights userRights, AccessControlType accessType)
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
            DirectorySecurity accessControl = directoryInfo.GetAccessControl();
            accessControl.AddAccessRule(new FileSystemAccessRule(userAccount, userRights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, accessType));
            directoryInfo.SetAccessControl(accessControl);
        }
        catch { }
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr ILCreateFromPathW(string pszPath);

    [DllImport("shell32.dll")]
    private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cild, IntPtr apidl, int dwFlags);
    [DllImport("shell32.dll")]
    private static extern void ILFree(IntPtr pidl);
    public static void OpenFolderAndSelectFile(string filePath)
    {
        if (filePath == null)
            return;

        IntPtr pidl = ILCreateFromPathW(filePath);
        SHOpenFolderAndSelectItems(pidl, 0, IntPtr.Zero, 0);
        ILFree(pidl);
    }

}

