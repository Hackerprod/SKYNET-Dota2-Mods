using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helpers
{
    public static class Paths
    {
        private static string RootPath { get { return GetRootPath(); } }
        public static string DataDirectory { get { return Path.Combine(RootPath, "Data"); } }
        public static string DBDirectory { get { return Path.Combine(DataDirectory, "DB"); } }
        public static string LogsDirectory { get { return Path.Combine(DataDirectory, "Logs"); } }
        public static string Assemblies { get { return Path.Combine(DataDirectory, "Assemblies"); } }
        public static string VPKGeneratorLocation { get { return Path.Combine(DataDirectory, "VPKGenerator"); } }
        public static string TempDirectory { get { return Path.Combine(DataDirectory, "TEMP"); } }

        public static string GetRootPath()
        {
            Process currentProcess;
            try
            {
                currentProcess = Process.GetCurrentProcess();
                return new FileInfo(currentProcess.MainModule.FileName).Directory?.FullName;
            }
            finally { currentProcess = null; }
        }

    }
}
