using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GitPatchExtractor
{
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int SHOpenFolderAndSelectItems(
          IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags
          );
    }

    internal class OpenFolderAndSelectItems
    {
        private static IntPtr FilePathToIDL(IShellLinkW shellLink, string filePath)
        {
            IntPtr idl;
            Marshal.ThrowExceptionForHR(
              shellLink.SetPath(filePath)
              );
            Marshal.ThrowExceptionForHR(
              shellLink.GetIDList(out idl)
              );
            return idl;
        }
        public OpenFolderAndSelectItems(string parentDirectory, ICollection<string> filenames)
        {
            IShellLinkW shellLink = (IShellLinkW)Activator.CreateInstance(
             Type.GetTypeFromCLSID(new Guid("00021401-0000-0000-C000-000000000046"), true)
             );
            var parentIdl = FilePathToIDL(shellLink, parentDirectory);
            var idls = filenames.Select(
              filename => FilePathToIDL(shellLink, Path.Combine(parentDirectory, filename))
              ).ToArray();
            try
            {
                NativeMethods.SHOpenFolderAndSelectItems(parentIdl, (uint)idls.Length, idls, 0);
            }
            finally
            {
                foreach (IntPtr idl in idls)
                {
                    Marshal.FreeCoTaskMem(idl);
                }
                Marshal.FreeCoTaskMem(parentIdl);
                Marshal.ReleaseComObject(shellLink);
            }
        }
    }
}
