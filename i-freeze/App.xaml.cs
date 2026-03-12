using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Utilities;
using i_freeze.View;
using i_freeze.ViewModel;
using IceLockWorker;
using Infrastructure;
using Infrastructure.DataContext;
using Infrastructure.Model;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using MessageBoxWindowConfirmPostpone = i_freeze.View.MessageBoxWindowConfirmPostpone;


namespace i_freeze
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string ShowMainEventName = "Global\\iFreeze_ShowMainWindow_Event_v1";

        private EventWaitHandle _showMainEvent;
        private bool _isShowMainEventOwner;

        private static Mutex _singleInstanceMutex;
        private static bool _isNewInstance;

        private const string FixedExePath = @"C:\Users\Public\Ice Lock\IceLock.exe";
        // Marker file so we create shortcuts only once per user
        private string ShortcutsMarkerPath()
            => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "i-Freeze", "shortcuts_created.flag");
        // Static ctor: ensure single instance and signal existing instance if present
        static App()
        {
            try
            {
                const string mutexName = "i-Freeze-SingleInstance-Mutex";
                _singleInstanceMutex = new Mutex(true, mutexName, out _isNewInstance);
                if (!_isNewInstance)
                {
                    // Another instance already running: try to signal it to show main window
                    try
                    {
                        var ev = EventWaitHandle.OpenExisting(ShowMainEventName);
                        ev.Set();
                        ev.Close();
                    }
                    catch
                    {
                        // Fallback: attempt to restore the other process main window (best-effort)
                        try
                        {
                            var current = Process.GetCurrentProcess();
                            var existing = Process.GetProcessesByName(current.ProcessName)
                                .FirstOrDefault(p => p.Id != current.Id);
                            if(existing != null)
                            {
                                NativeMethods.ShowWindow(existing.MainWindowHandle, WindowShowStyle.Restore);
                                NativeMethods.SetForegroundWindow(existing.MainWindowHandle);
                            }
                        }
                        catch { /* ignore */ }
                    }
                    // Exit this new instance immediately
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred during static initialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.IO.File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"), ex.Message);
                DeviceManagement.SendLogs(ex.Message, "app class static constructor").Wait();
            }
      

            //if (!_isNewInstance)
            //{
            //    // Another instance exists, activate it and exit
            //    var current = Process.GetCurrentProcess();
            //    var existing = Process.GetProcessesByName(current.ProcessName)
            //        .FirstOrDefault(p => p.Id != current.Id);

            //    if (existing != null)
            //    {
            //        NativeMethods.ShowWindow(existing.MainWindowHandle, WindowShowStyle.Restore);
            //        NativeMethods.SetForegroundWindow(existing.MainWindowHandle);
            //    }
            //    Environment.Exit(0);
            //}
        }

        public App()
        {
            // Subscribe to SessionEnding event
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            // Check if the reason for session ending is a system restart
            if (e.Reason == SessionEndReasons.SystemShutdown)
            {
                ShutdownFunction();
            }
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create/open the named EventWaitHandle; if createdNew==true this instance will listen for signals
            try
            {
                bool createdNew;
                _showMainEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowMainEventName, out createdNew);
                _isShowMainEventOwner = createdNew;

                if (_isShowMainEventOwner)
                {
                    // Start background listener that waits for the event and shows the main window when signalled
                    Task.Run(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                // Wait indefinitely until signaled
                                _showMainEvent.WaitOne();

                                // Marshal to UI thread and show/activate the main window
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        if (Application.Current.MainWindow == null)
                                        {
                                            Application.Current.MainWindow = new MainWindow();
                                        }

                                        var mw = Application.Current.MainWindow;

                                        // If window was hidden to tray, make sure it's visible
                                        if (mw.Visibility != Visibility.Visible)
                                        {
                                            mw.Show();
                                        }

                                        // Restore from minimized state
                                        if (mw.WindowState == WindowState.Minimized)
                                            mw.WindowState = WindowState.Normal;

                                        // Bring to front & activate
                                        mw.Activate();

                                        try
                                        {
                                            var helper = new System.Windows.Interop.WindowInteropHelper(mw);
                                            NativeMethods.ShowWindow(helper.Handle, WindowShowStyle.Restore);
                                            NativeMethods.SetForegroundWindow(helper.Handle);
                                        }
                                        catch { /* ignore if fails */ }
                                    }
                                    catch (Exception ex)
                                    {
                                        // best-effort logging; don't crash the listener
                                        _ = DeviceManagement.SendLogs(ex.Message, "App.ShowMainWindowListener");
                                    }
                                });
                            }
                        }
                        catch (ThreadAbortException) { /* listener aborted */ }
                        catch (Exception ex)
                        {
                            _ = DeviceManagement.SendLogs(ex.Message, "App.ShowMainWindowListenerTask");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // if we can't create/open the event, log but continue startup
                await DeviceManagement.SendLogs(ex.Message, "App.OnStartup.EventInit");
            }

            // --- Existing startup behavior (kept) ---
            try
            {
                using (var context = new i_Freeze_WindowContext())
                {
                    // Simplified: create and show main window (your previous code had a flag, you can restore it if needed)
                    MainWindow = new MainWindow();
                    MainWindow.Show();
                    Task.Run(() =>
                    {
                        try
                        {
                            EnsureShortcutsCreatedOnce();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Create shortcuts error: " + ex.Message);
                        }
                    });
                    var serviceControl = new WindowsServiceControl();
                    await serviceControl.RunDLP_PM();
                    Task.Run(async() =>
                    {
                        try
                        {
                            var serviceControl = new WindowsServiceControl();
                            await serviceControl.RunDLP_PM();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("RunDLP_PM error: " + ex.Message);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred during startup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.IO.File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"), ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "app class OnStartup method");
            }

            //// Create shortcut in start menu (keeps your behavior)
            //try
            //{
            //    await CreateShortcutInStartMenu();
            //}
            //catch (Exception ex)
            //{
            //    await DeviceManagement.SendLogs(ex.Message, "App.OnStartup.CreateShortcut");
            //}

            // You can start other background tasks here if needed (update checks, isolate loop, etc.)
            // e.g. Task.Run(() => IsolateCheckTimeLoop());
        }
        private void ShutdownFunction()
        {
            //try
            //{
                // Create a new context instance for shutdown function to avoid accessing a disposed one
            //    using (var context = new i_Freeze_WindowContext())
            //    {
            //        var flag = context.i_Freeze_Window.FirstOrDefault();
            //        if (flag != null)
            //        {
            //            flag.Flag = 0;
            //            context.SaveChanges();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error occurred during shutdown: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup event and mutex and unsubscribe events
            try
            {
                try
                {
                    if (_showMainEvent != null)
                    {
                        try
                        {
                            // signal listener so it can wake (optional)
                            _showMainEvent.Set();
                        }
                        catch { /* ignore */ }

                        try
                        {
                            _showMainEvent.Close();
                        }
                        catch { /* ignore */ }

                        _showMainEvent = null;
                    }
                }
                catch { /* ignore */ }

                try
                {
                    _singleInstanceMutex?.ReleaseMutex();
                    _singleInstanceMutex?.Dispose();
                    _singleInstanceMutex = null;
                }
                catch { /* ignore */ }

                try
                {
                    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
                }
                catch { /* ignore */ }
            }
            finally
            {
                base.OnExit(e);
            }
        }

        //protected override void OnExit(ExitEventArgs e)
        //{
        //    base.OnExit(e);

        //    //try
        //    //{
        //    //    using (var context = new i_Freeze_WindowContext())
        //    //    {
        //    //        var flag = context.i_Freeze_Window.FirstOrDefault();
        //    //        if (flag != null)
        //    //        {
        //    //            flag.Flag = 0;
        //    //            context.SaveChanges();
        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    // Log error but don't show UI during shutdown
        //    //    System.IO.File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"),
        //    //        $"Shutdown error: {ex.Message}");
        //    //}
        //    //finally
        //    //{
        //    //    _singleInstanceMutex?.ReleaseMutex();
        //    //    _singleInstanceMutex?.Dispose();
        //    //    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
        //    //}
        //}

        private static readonly HttpClient httpClient = new HttpClient();

      
        private async Task UpdateDeviceActionAsync(Action<DeviceActions> updateAction)
        {
            try
            {
                using (var deviceContext = new DeviceContext())
                {
                    var deviceActions = deviceContext.DeviceActions.FirstOrDefault();
                    if (deviceActions != null)
                    {
                        updateAction(deviceActions);
                        await deviceContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // await DeviceManagement.SendLogs(ex.Message, "DeviceViewModel UpdateDeviceActionAsync");
            }
        }

        // Add these methods inside your App class

        /// <summary>
        /// Create a Windows shortcut (.lnk) using WSH.
        /// </summary>
        private void CreateShortcutFile(string shortcutPath, string targetPath, string workingDirectory = null,
            string iconLocation = null, string arguments = null, string description = null)
            {
            try
            {
                // Ensure the directory exists
                var dir = Path.GetDirectoryName(shortcutPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                if (!string.IsNullOrWhiteSpace(workingDirectory))
                    shortcut.WorkingDirectory = workingDirectory;
                if (!string.IsNullOrWhiteSpace(iconLocation))
                    shortcut.IconLocation = iconLocation;
                if (!string.IsNullOrWhiteSpace(arguments))
                    shortcut.Arguments = arguments;
                if (!string.IsNullOrWhiteSpace(description))
                    shortcut.Description = description;

                // Save the .lnk file
                shortcut.Save();
            }
            catch (Exception ex)
            {
                // Log but do not throw — shortcut creation is best-effort
                Debug.WriteLine("CreateShortcutFile failed: " + ex.Message);
                _ = DeviceManagement.SendLogs(ex.Message, "CreateShortcutFile");
            }
        }

        /// <summary>
        /// Create per-user Start Menu Programs shortcut (visible in user's Start menu).
        /// </summary>
        private void CreateStartMenuShortcut()
        {
            try
            {
                var exePath = FixedExePath;
                if (!System.IO.File.Exists(exePath))
                {
                    Debug.WriteLine("CreateStartMenuShortcut: exe not found: " + exePath);
                    return;
                }

                var programs = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                var programsFolder = Path.Combine(programs, "Programs");
                var shortcutPath = Path.Combine(programsFolder, "IceLock.lnk");

                CreateShortcutFile(
                    shortcutPath,
                    targetPath: exePath,
                    workingDirectory: Path.GetDirectoryName(exePath),
                    iconLocation: exePath, // uses exe icon
                    arguments: "",
                    description: "IceLock"
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CreateStartMenuShortcut failed: " + ex.Message);
                _ = DeviceManagement.SendLogs(ex.Message, "CreateStartMenuShortcut");
            }
        }

        /// <summary>
        /// Create per-user Desktop shortcut.
        /// </summary>
        private void CreateDesktopShortcut()
        {
            try
            {
                var exePath = FixedExePath;
                if (!System.IO.File.Exists(exePath))
                {
                    Debug.WriteLine("CreateDesktopShortcut: exe not found: " + exePath);
                    return;
                }

                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var shortcutPath = Path.Combine(desktop, "IceLock.lnk");

                CreateShortcutFile(
                    shortcutPath,
                    targetPath: exePath,
                    workingDirectory: Path.GetDirectoryName(exePath),
                    iconLocation: exePath,
                    arguments: "",
                    description: "IceLock"
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CreateDesktopShortcut failed: " + ex.Message);
                _ = DeviceManagement.SendLogs(ex.Message, "CreateDesktopShortcut");
            }
        }

        /// <summary>
        /// Create a per-user Startup shortcut (so app launches at user sign-in).
        /// </summary>
        private void CreateStartupShortcut()
        {
            try
            {
                var exePath = FixedExePath;
                if (!System.IO.File.Exists(exePath))
                {
                    Debug.WriteLine("CreateStartupShortcut: exe not found: " + exePath);
                    return;
                }

                var startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                var shortcutPath = Path.Combine(startup, "IceLock.lnk");

                CreateShortcutFile(
                    shortcutPath,
                    targetPath: exePath,
                    workingDirectory: Path.GetDirectoryName(exePath),
                    iconLocation: exePath,
                    arguments: "",
                    description: "IceLock Auto Start"
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CreateStartupShortcut failed: " + ex.Message);
                _ = DeviceManagement.SendLogs(ex.Message, "CreateStartupShortcut");
            }
        }


        /// <summary>
        /// Ensure shortcuts are created only once (marker file).
        /// </summary>
        private void EnsureShortcutsCreatedOnce()
        {
            try
            {
                var marker = ShortcutsMarkerPath();
                var dir = Path.GetDirectoryName(marker);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (System.IO.File.Exists(marker)) return;

                CreateStartMenuShortcut();
                CreateDesktopShortcut();
                CreateStartupShortcut();

                // write marker (contains timestamp)
                System.IO.File.WriteAllText(marker, DateTime.UtcNow.ToString("o"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EnsureShortcutsCreatedOnce failed: " + ex.Message);
                _ = DeviceManagement.SendLogs(ex.Message, "EnsureShortcutsCreatedOnce");
            }
        }
        //private async Task CreateShortcutInStartMenu()
        //{
        //    string startMenuPath = Path.Combine(
        //        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
        //        "Programs"
        //    );

        //    if (!Directory.Exists(startMenuPath))
        //        Directory.CreateDirectory(startMenuPath);

        //    string shortcutLocation = Path.Combine(startMenuPath, "i-Freeze.lnk");

        //    string targetApplicationPath = @"C:\Users\Public\Ice Lock\i-FreezeRestart.exe";

        //    WshShell shell = new WshShell();
        //    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

        //    shortcut.Description = "i-Freeze";
        //    shortcut.IconLocation = @"C:\Users\Public\Ice Lock\MixSheld.ico";
        //    shortcut.TargetPath = targetApplicationPath;
        //    shortcut.WorkingDirectory = @"C:\Users\Public\Ice Lock";

        //    shortcut.Save();
        //}

        //private async Task CreateStartupShortcut()
        //{
        //    string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        //    string shortcutLocation = Path.Combine(startupPath, "i-Freeze.lnk");

        //    string targetApplicationPath = @"C:\Users\Public\Ice Lock\i-FreezeWatch.exe";

        //    WshShell shell = new WshShell();
        //    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

        //    shortcut.Description = "i-Freeze Auto Start";
        //    shortcut.IconLocation = @"C:\Users\Public\Ice Lock\MixSheld.ico";
        //    shortcut.TargetPath = targetApplicationPath;
        //    shortcut.WorkingDirectory = @"C:\Users\Public\Ice Lock";

        //    shortcut.Save();
        //}

        //private async Task CreateShortcutOnDesktop()
        //{
        //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //    if (!Directory.Exists(desktopPath))
        //        Directory.CreateDirectory(desktopPath);

        //    string shortcutLocation = Path.Combine(desktopPath, "i-Freeze.lnk");

        //    string targetApplicationPath = @"C:\Users\Public\Ice Lock\i-FreezeRestart.exe";

        //    WshShell shell = new WshShell();
        //    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

        //    shortcut.Description = "i-Freeze";
        //    shortcut.IconLocation = @"C:\Users\Public\Ice Lock\MixSheld.ico";
        //    shortcut.TargetPath = targetApplicationPath;
        //    shortcut.WorkingDirectory = @"C:\Users\Public\Ice Lock";

        //    shortcut.Save();
        //}


        private static class NativeMethods
        {
            /// <summary>
            /// Brings the thread that created the specified window into the
            /// foreground and activates the window.
            /// </summary>
            [DllImport("user32.dll")]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);

            /// <summary>Shows a Window</summary>
            [DllImport("user32.dll")]
            internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
        }

        /// <summary>
        /// Enumeration of the different ways of showing a window.
        /// </summary>
        internal enum WindowShowStyle : uint
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            Maximize = 3,
            ShowNormalNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActivate = 7,
            ShowNoActivate = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimized = 11
        }
    }
}