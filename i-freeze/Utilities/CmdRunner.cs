using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Utilities
{
    public class CmdRunner
    {
        string path;



        public CmdRunner(string path)
        {
            this.path = path;
        }
        public void cmdprocess()
        {


            string cwd = Directory.GetCurrentDirectory();


            var p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = cwd;
            p.Start();
            p.StandardOutput.ReadToEnd();
        }
    }
}
