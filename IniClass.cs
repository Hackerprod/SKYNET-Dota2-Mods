using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SkynetDota2Mods
{
    public class IniClass
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "GetPrivateProfileStringA", ExactSpelling = true, SetLastError = true)]
        private static extern int GetPrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDefault, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpReturnedString, int nSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "WritePrivateProfileStringA", ExactSpelling = true, SetLastError = true)]
        private static extern int WritePrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpString, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "GetPrivateProfileSectionA", ExactSpelling = true, SetLastError = true)]
        private static extern int GetPrivateProfileSection([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpAppName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpReturnedString, int nSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "WritePrivateProfileSectionA", ExactSpelling = true, SetLastError = true)]
        private static extern int WritePrivateProfileSection([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpAllKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);

        public static string GetIniSectionKey(string sSection, string sKey, string sIniFileName)
        {
            string lpReturnedString = new string('\0', 2048);
            string lpDefault = "";
            int privateProfileString = GetPrivateProfileString(ref sSection, ref sKey, ref lpDefault, ref lpReturnedString, Strings.Len(lpReturnedString), ref sIniFileName);
            return Strings.Left(lpReturnedString, privateProfileString).Trim('\0');
        }

        public static bool SetIniSectionKey(string sSection, string sKey, string sValue, string sIniFileName)
        {
            int num = WritePrivateProfileString(ref sSection, ref sKey, ref sValue, ref sIniFileName);
            return Conversions.ToBoolean(Interaction.IIf(num == 0, false, true));
        }

        public static int GetIniSectionKeyCount(string sSection, string sIniFileName)
        {
            string lpReturnedString = new string('\0', 1024);
            string[] array = null;
            string lpKeyName = null;
            string lpDefault = "";
            int privateProfileString = GetPrivateProfileString(ref sSection, ref lpKeyName, ref lpDefault, ref lpReturnedString, 1024, ref sIniFileName);
            array = lpReturnedString.Substring(0, privateProfileString).Split(new char[1]
            {
            '\0'
            }, StringSplitOptions.RemoveEmptyEntries);
            return array.Length;
        }

        public static string[] GetIniSectionAllKey(string sSection, string sIniFileName)
        {
            string lpReturnedString = new string('\0', 1024);
            string[] array = null;
            string lpKeyName = null;
            string lpDefault = "";
            int privateProfileString = GetPrivateProfileString(ref sSection, ref lpKeyName, ref lpDefault, ref lpReturnedString, 1024, ref sIniFileName);
            return lpReturnedString.Substring(0, privateProfileString).Split(new char[1]
            {
            '\0'
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void SetIniSectionAllKey(string sSection, string sAllKey, string sIniFileName)
        {
            sAllKey += Conversions.ToString(Interaction.IIf(sAllKey.EndsWith("\0"), "", '\0'));
            WritePrivateProfileSection(ref sSection, ref sAllKey, ref sIniFileName);
        }

        public static int GetIniSectionCount(string sIniFileName)
        {
            string lpReturnedString = new string('\0', 1024);
            string lpApplicationName = null;
            string lpKeyName = null;
            string lpDefault = "";
            int privateProfileString = GetPrivateProfileString(ref lpApplicationName, ref lpKeyName, ref lpDefault, ref lpReturnedString, 1024, ref sIniFileName);
            string text = Strings.Left(lpReturnedString, privateProfileString).Trim('\0');
            return text.Split('\0').Length;
        }

        public static string[] GetIniAllSection(string sIniFileName)
        {
            string lpReturnedString = new string('\0', 1024);
            string lpApplicationName = null;
            string lpKeyName = null;
            string lpDefault = "";
            int privateProfileString = GetPrivateProfileString(ref lpApplicationName, ref lpKeyName, ref lpDefault, ref lpReturnedString, 1024, ref sIniFileName);
            string text = Strings.Left(lpReturnedString, privateProfileString).TrimEnd('\0');
            return text.Split('\0');
        }

        public static void DeleteSection(string sSection, string sIniFileName)
        {
            string lpKeyName = null;
            string lpString = null;
            int num = WritePrivateProfileString(ref sSection, ref lpKeyName, ref lpString, ref sIniFileName);
        }

        public static void DeleteSectionKey(string sSection, string sKey, string sIniFileName)
        {
            string lpString = null;
            int num = WritePrivateProfileString(ref sSection, ref sKey, ref lpString, ref sIniFileName);
        }
    }
}
