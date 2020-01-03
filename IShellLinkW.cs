using System;
using System.Runtime.InteropServices;
using System.Text;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214F9-0000-0000-C000-000000000046")]
public interface IShellLinkW
{
    [PreserveSig]
    int GetPath(StringBuilder pszFile, int cch, [In, Out] ref WIN32_FIND_DATAW pfd, uint fFlags);

    [PreserveSig]
    int GetIDList([Out] out IntPtr ppidl);

    [PreserveSig]
    int SetIDList([In] ref IntPtr pidl);

    [PreserveSig]
    int GetDescription(StringBuilder pszName, int cch);

    [PreserveSig]
    int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    [PreserveSig]
    int GetWorkingDirectory(StringBuilder pszDir, int cch);

    [PreserveSig]
    int SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

    [PreserveSig]
    int GetArguments(StringBuilder pszArgs, int cch);

    [PreserveSig]
    int SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

    [PreserveSig]
    int GetHotkey([Out] out ushort pwHotkey);

    [PreserveSig]
    int SetHotkey(ushort wHotkey);

    [PreserveSig]
    int GetShowCmd([Out] out int piShowCmd);

    [PreserveSig]
    int SetShowCmd(int iShowCmd);

    [PreserveSig]
    int GetIconLocation(StringBuilder pszIconPath, int cch, [Out] out int piIcon);

    [PreserveSig]
    int SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

    [PreserveSig]
    int SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

    [PreserveSig]
    int Resolve(IntPtr hwnd, uint fFlags);

    [PreserveSig]
    int SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}

[Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), BestFitMapping(false)]
public struct WIN32_FIND_DATAW
{
    public uint dwFileAttributes;
    public FILETIME ftCreationTime;
    public FILETIME ftLastAccessTime;
    public FILETIME ftLastWriteTime;
    public uint nFileSizeHigh;
    public uint nFileSizeLow;
    public uint dwReserved0;
    public uint dwReserved1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string cFileName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
    public string cAlternateFileName;
}

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct FILETIME
{
    public uint dwLowDateTime;
    public uint dwHighDateTime;
}