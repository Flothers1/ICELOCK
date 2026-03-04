using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Utilities
{
    public class Reusable
    {

        public static void OpenUrl(string url)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = url;
            processInfo.UseShellExecute = true;
            Process.Start(processInfo);
        }

    }
}
