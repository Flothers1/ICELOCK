using Infrastructure;
using Infrastructure.DataContext;
using Infrastructure.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace i_freeze.Utilities
{
    /// <summary>
    /// High-performance, async-first process execution utility with proper resource management
    /// </summary>
    public class ExeRunner : IDisposable
    {
        private readonly string _path;
        private readonly string _args;
        private Process _currentProcess;
        private bool _disposed = false;

        public ExeRunner(string path, string args = null)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _args = args;
        }

        /// <summary>
        /// Executes process asynchronously with cancellation support
        /// </summary>
        public async Task<ProcessResult> RunProcessAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var process = CreateProcess();
            
                process.Start();
                _currentProcess = process;

                await process.WaitForExitAsync(cancellationToken);
                return new ProcessResult(process.ExitCode, process.ExitCode == 0);
            }
            catch (OperationCanceledException)
            {
                _currentProcess?.Kill();
                return ProcessResult.Cancelled;
            }
        
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ExeRunner RunProcessAsync");
                return ProcessResult.Failed;
            }
         
            finally
            {
                _currentProcess = null;
            }
        }

        /// <summary>
        /// Executes process with Arabic code page support
        /// </summary>
        public async Task<ProcessResult> RunProcessForArabicAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Set console code page to 1256 (Windows Arabic)
                using (var codePageProcess = CreateCodePageProcess())
                {
                    codePageProcess.Start();
                    await codePageProcess.WaitForExitAsync(cancellationToken);
                }

                // Run main process
                return await RunProcessAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ExeRunner RunProcessForArabicAsync");
                return ProcessResult.Failed;
            }
        }

        /// <summary>
        /// Executes process as administrator with UAC prompt
        /// </summary>
        public async Task<ProcessResult> RunProcessAsAdminAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await UpdateAdminStatusAsync(false);

                using var process = CreateAdminProcess();
                process.Start();
                _currentProcess = process;

                await process.WaitForExitAsync(cancellationToken);
                await UpdateAdminStatusAsync(true);

                return new ProcessResult(process.ExitCode, process.ExitCode == 0);
            }
            catch (OperationCanceledException)
            {
                _currentProcess?.Kill();
                return ProcessResult.Cancelled;
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ExeRunner RunProcessAsAdminAsync");
                return ProcessResult.Failed;
            }
            finally
            {
                _currentProcess = null;
            }
        }

        /// <summary>
        /// Executes process and returns output
        /// </summary>
        public async Task<ProcessOutputResult> RunProcessWithOutputAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var process = CreateProcessWithOutput();
                process.Start();
                _currentProcess = process;

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync(cancellationToken);

                var output = await outputTask;
                var error = await errorTask;

                return new ProcessOutputResult(process.ExitCode, output, error, process.ExitCode == 0);
            }
            catch (OperationCanceledException)
            {
                _currentProcess?.Kill();
                return ProcessOutputResult.Cancelled;
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ExeRunner RunProcessWithOutputAsync");
                return ProcessOutputResult.Failed;
            }
            finally
            {
                _currentProcess = null;
            }
        }

        /// <summary>
        /// Special method for software update with admin privileges
        /// </summary>
        public async Task<int> RunProcessAsAdminForSoftUpdate(CancellationToken cancellationToken = default)
        {
            try
            {
                using var process = CreateAdminProcess();
                process.Start();
                _currentProcess = process;

                await process.WaitForExitAsync(cancellationToken);
                return process.ExitCode == 1 ? 0 : 1;
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ExeRunner RunProcessAsAdminForSoftUpdate");
                return 1;
            }
            finally
            {
                _currentProcess = null;
            }
        }

        #region Backward Compatibility (Deprecated - Use Async versions)
        [Obsolete("Use RunProcessAsync instead")]
        public async void runprocess()
        {
            await RunProcessAsync();
        }

        [Obsolete("Use RunProcessForArabicAsync instead")]
        public async void RunProcessForArabic()
        {
            await RunProcessForArabicAsync();
        }

        [Obsolete("Use RunProcessAsAdminAsync instead")]
        public void RunProcessAsAdmin()
        {
            _ = Task.Run(async () => await RunProcessAsAdminAsync());
        }

        [Obsolete("Use RunProcessWithOutputAsync instead")]
        public string runprocess_output()
        {
            var result = RunProcessWithOutputAsync().GetAwaiter().GetResult();
            return result.Output;
        }

        [Obsolete("Use RunProcessWithOutputAsync instead")]
        public async void RunToEnd()
        {
            await RunProcessWithOutputAsync();
        }

        [Obsolete("Use RunProcessWithOutputAsync instead")]
        public async void RunToEnd2()
        {
            await RunProcessWithOutputAsync();
        }
        #endregion

        #region Private Helper Methods
        private Process CreateProcess()
        {
            var workingDir = @"C:\Users\Public\Ice Lock\";
            var exePath = Path.IsPathRooted(_path) ? _path : Path.Combine(workingDir, _path);

            // Diagnostic / early failure
            if (!File.Exists(exePath))
                throw new FileNotFoundException($"Executable not found: {exePath}");

            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _path,
                    Arguments = _args ?? string.Empty,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                //    WorkingDirectory = Directory.GetCurrentDirectory()
                    WorkingDirectory = workingDir
                },
                EnableRaisingEvents = true
            };
        }

        private Process CreateProcessWithOutput()
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _path,
                    Arguments = _args ?? string.Empty,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Directory.GetCurrentDirectory()
                }
            };
        }

        private Process CreateAdminProcess()
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _path,
                    Arguments = _args ?? string.Empty,
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = Directory.GetCurrentDirectory()
                }
            };
        }

        private Process CreateCodePageProcess()
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c chcp 1256",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Directory.GetCurrentDirectory()
                }
            };
        }

        private async Task UpdateAdminStatusAsync(bool isAdmin)
        {
            try
            {
                using (var context = new ToggleButtonActionContext())
                {
                    var adminToggle = context.ToggleButtonAction.Find("IsAdmin");
                    if (adminToggle != null)
                    {
                        adminToggle.Action = isAdmin ? 1 : 0;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ExeRunner UpdateAdminStatusAsync");
            }
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    _currentProcess?.Kill();
                    _currentProcess?.Dispose();
                }
                catch
                {
                    // Ignore disposal errors
                }
                _disposed = true;
            }
        }

        ~ExeRunner()
        {
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Result of process execution
    /// </summary>
    public class ProcessResult
    {
        public int ExitCode { get; }
        public bool Success { get; }
        public bool WasCancelled { get; }

        public ProcessResult(int exitCode, bool success, bool wasCancelled = false)
        {
            ExitCode = exitCode;
            Success = success;
            WasCancelled = wasCancelled;
        }

        public static ProcessResult Cancelled => new ProcessResult(-1, false, true);
        public static ProcessResult Failed => new ProcessResult(-1, false);
    }

    /// <summary>
    /// Result of process execution with output capture
    /// </summary>
    public class ProcessOutputResult : ProcessResult
    {
        public string Output { get; }
        public string Error { get; }

        public ProcessOutputResult(int exitCode, string output, string error, bool success, bool wasCancelled = false)
            : base(exitCode, success, wasCancelled)
        {
            Output = output ?? string.Empty;
            Error = error ?? string.Empty;
        }

        public static new ProcessOutputResult Cancelled => new ProcessOutputResult(-1, string.Empty, string.Empty, false, true);
        public static new ProcessOutputResult Failed => new ProcessOutputResult(-1, string.Empty, string.Empty, false);
    }
}