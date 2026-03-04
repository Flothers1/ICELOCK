
using System.Diagnostics;



namespace IceLockWorker
{
    class ExeRunner
    {
        string path;
        string args;

        public ExeRunner(string path)
        {
            this.path = path;
        }
        public ExeRunner(string path, string args)
        {
            this.path = path;
            this.args = args;
        }
        /// <summary>
        /// Safely quote an argument for the command line.
        /// Replaces internal double-quotes with escaped form and wraps with quotes.
        /// </summary>
        private static string QuoteArgument(string a)
        {
            if (a == null) return "\"\"";
            return "\"" + a.Replace("\"", "\\\"") + "\"";
        }

        private static string BuildArgs(params string[] pieces)
        {
            return string.Join(" ", pieces.Select(QuoteArgument));
        }

        /// <summary>
        /// The RunProcess method starts a new process using the specified executable path and arguments, 
        /// with administrative privileges. It waits for the process to exit before returning the process 
        /// object. If any exceptions occur during the process startup, they are logged, and an error 
        /// message is returned through the out parameter.
        /// </summary>
        /// <param name="error">An output parameter that captures any error message if an exception occurs.</param>
        /// <returns>A Process object representing the started process, or null if an error occurs.</returns>
        public Process RunProcess(out string error)
        {
            error = string.Empty;

            try
            {
                var p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = args;
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                p.WaitForExit();
                return p;
            }
            catch (Exception ex)
            {
                IFreezeService.Log($"ExeRunner RunProcess Exception {ex.Message}.");
                return null;
            }
        }

        /// <summary>
        /// The runprocess function starts a new process using the specified executable path and arguments. 
        /// It sets up the process to run with administrative privileges, hides the window during execution, 
        /// and uses the current directory as the working directory. If any exceptions occur during the 
        /// process startup, they are logged for troubleshooting purposes.
        /// </summary>
        public async void runprocess()
        {
            try
            {
                string cwd = Directory.GetCurrentDirectory();
                var p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = args;
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.WorkingDirectory = cwd;
                p.Start();
            }
            catch (Exception ex)
            {
                await IFreezeService.Log($"ExeRunner RunProcess Exception {ex.Message}.");
            }

        }
        /// <summary>
        /// Synchronously run the exe (this.path) with three parameters: filePath, id and code.
        /// Waits for the process to exit. Returns the Process or null on error; sets out error.
        /// </summary>
        public Process RunUploadFile(string filePath, string id, string code, out string error)
        {
            error = string.Empty;
            try
            {
                string cwd = Directory.GetCurrentDirectory();
                var p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = BuildArgs(filePath, id, code);
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.UseShellExecute = true; // required for runas
                p.StartInfo.Verb = "runas";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.WorkingDirectory = cwd;
                p.Start();
                p.WaitForExit();
                return p;
            }
            catch (Exception ex)
            {
                try { IFreezeService.Log($"ExeRunner RunUploadFile Exception {ex.Message}."); } catch { }
                error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Async version: runs the exe with three arguments and awaits its exit.
        /// Returns the Process (after exit) or null on error.
        /// </summary>
        public async Task<Process> RunUploadFileAsync(string filePath, string id, string code)
        {
            try
            {
                string cwd = Directory.GetCurrentDirectory();
                var p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = BuildArgs(filePath, id, code);
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.UseShellExecute = true; // required for runas
                p.StartInfo.Verb = "runas";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.WorkingDirectory = cwd;
                p.Start();
#if NET5_0_OR_GREATER || NETCOREAPP3_1
                // WaitForExitAsync exists in .NET Core 3.1+ / .NET 5+
                await p.WaitForExitAsync();
#else
                // Fallback for older frameworks: block on Task.Run
                await Task.Run(() => p.WaitForExit());
#endif
                return p;
            }
            catch (Exception ex)
            {
                // original code sometimes awaited this; keep async logging
                try { await IFreezeService.Log($"ExeRunner RunUploadFileAsync Exception: {ex.Message}"); } catch { }
                return null;
            }
        }

        public async Task RunPiFreezeAsync()
        {
            try
            {
                string cwd = Directory.GetCurrentDirectory();

                var p = new Process();
                p.StartInfo.FileName = path;
                p.StartInfo.Arguments = args;
                p.StartInfo.RedirectStandardError = false;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas"; // Admin
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.WorkingDirectory = cwd;

                p.Start();
            }
            catch (Exception ex)
            {
                await IFreezeService.Log($"ExeRunner RunPiFreezeAsync Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// The RunCommandAsAdmin function executes a specified command in the command prompt with elevated 
        /// (administrator) privileges. It uses the "runas" verb to prompt the user for permission to run 
        /// the command as an administrator. The command is passed as an argument to cmd.exe, and the 
        /// function waits for the command execution to complete before returning.
        /// </summary>
        /// <param name="command">The command to be executed with administrative privileges.</param>
        public void RunCommandAsAdmin(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C " + command,
                Verb = "runas", // This will prompt for admin privileges
                UseShellExecute = true,
            };

            Process.Start(psi)?.WaitForExit();
        }


    }
}
