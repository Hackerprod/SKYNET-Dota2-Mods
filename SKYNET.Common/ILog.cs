using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SKYNET.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class ILog
{
    private static Mutex mutexFile;
    public static string fileName = "[SKYNET] Dota2 Mods.log";
    static Process currentProcess;
    public string Message { get; set; } = "";
    public bool IsDebugEnabled = true;

    public static void Save(object ex)
    {
        string ErrorMessage = "";

        if (fileName == "")
            fileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName) + ".log";

        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ex.ToString());
            ErrorMessage = string.Format($"{(object)stringBuilder.ToString()}:");
            AppendFile(ErrorMessage, fileName);
        }
        catch { }
    }
    public static void Save(Exception ex)
    {
        string ErrorMessage = "";

        if (fileName == "")
            fileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName) + ".log";

        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ex.Message);
            if (ex.InnerException != null)
                stringBuilder.Append("\r\n").Append((object)ex.InnerException);
            if (ex.StackTrace != null)
                stringBuilder.Append("\r\n").Append(ex.StackTrace);
            ErrorMessage = string.Format($"{(object)stringBuilder.ToString()}:");
            AppendFile(ErrorMessage, fileName);
        }
        catch { }
    }
    public static void Save(string strs, Exception ex)
    {
        string ErrorMessage = "";

        if (fileName == "")
            fileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName) + ".log";

        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ex.Message);
            if (ex.InnerException != null)
                stringBuilder.Append("\r\n").Append((object)ex.InnerException);
            if (ex.StackTrace != null)
                stringBuilder.Append("\r\n").Append(ex.StackTrace);
            ErrorMessage = string.Format($"{(object)stringBuilder.ToString()}:");

            AppendFile(strs + Environment.NewLine + ErrorMessage, fileName);
        }
        catch { }
    }
    public static void AppendFile(string s, string filename)
    {
        string path = Path.Combine(Paths.LogsDirectory, filename);
        StreamWriter streamWriter = null;
        try
        {
            mutexFile = LogMutex.mutexFile;
            mutexFile.WaitOne();
            FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
            streamWriter = new StreamWriter(stream);
            streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
            streamWriter.WriteLine(Conversions.ToString(DateAndTime.Now) + ":" + s);
            streamWriter.Close();
        }
        catch (Exception ex)
        {
            Exception ex2 = ex;
            streamWriter?.Close();
        }
        finally
        {
            mutexFile.ReleaseMutex();
        }
    }
    public class LogMutex
    {
        public static Mutex mutexFile = new Mutex(false, "LogMutex");
    }

    private static string GetDataDirectory()
    {
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).Directory?.FullName + @"\Data";
        }
        finally { currentProcess = null; }
    }


    Assembly assembly;
    private string HeadMessage;

    public ILog(string Head)
    {
        this.HeadMessage = Head;
    }

    public ILog()
    {
    }


    public static string GetExePatch()
    {
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).FullName;
        }
        finally { currentProcess = null; }
    }
    public bool IsWarnEnabled()
    { return true; }

    public void MoreInfo(object v, Exception ex8 = null)
    {
        //throw new NotImplementedException();
    }

}
