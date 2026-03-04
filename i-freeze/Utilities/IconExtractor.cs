using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Utilities
{
    public class IconExtractor
    {
        //This flag instructs the SHGetFileInfo function to retrieve the handle of the icon that represents the file.
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0; // This flag requests a large icon.
        private const uint SHGFI_SMALLICON = 0x1; // This flag requests a small icon.
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80; // This flag specifies that the file is a standard file with no special attributes.


        // This structure holds information about a file, including an icon handle 
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        //This method retrieves the icon associated with the specified file. It
        /*
            1-Initializes an SHFILEINFO structure to hold the icon and other file information.
            2-Sets the flags variable to request either a large or small icon based on the largeIcon parameter.
            3-Calls the SHGetFileInfo function to retrieve the icon handle.
            4-Converts the handle into an Icon object and clones it to create a managed copy.
            5-Calls DestroyIcon to free the resources associated with the original icon handle.
            6-Returns the managed Icon object to the caller.
         */

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public static Icon GetFileIcon(string filePath, bool largeIcon)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | (largeIcon ? SHGFI_LARGEICON : SHGFI_SMALLICON);

            SHGetFileInfo(filePath, FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);

            Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
            DestroyIcon(shinfo.hIcon);

            return icon;
        }

        [DllImport("user32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);
    }
}
