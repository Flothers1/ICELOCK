using i_freeze.Utilities;
using i_freeze.ViewModel;
using Infrastructure.DataContext;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace i_freeze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            CreateNotifyIcon();
        }

        public void Navigate(Window nextPage)
        {
            this.Content = nextPage.Content;
        }

        private void CreateNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("ICELOCK-MainLogo (2).ico"),
                Visible = true,
                Text = "Ice Lock"
            };

            // Set up the context menu using ContextMenuStrip
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("Open", null, (s, e) => ShowMainWindow()));
            contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) => ExitApplication()));

            _notifyIcon.ContextMenuStrip = contextMenu;

            // Show the window when the icon is clicked
            _notifyIcon.DoubleClick += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        public void ShowMainWindow()
        {
            // Don't recreate ViewModels unnecessarily - major performance fix
            if (AppSettings.Instance.CurrentView == null)
            {
                if (AppSettings.Instance.CurrentView is LoginViewModel)
                {
                    AppSettings.Instance.CurrentView = new LoginViewModel();
                }
                else
                {
                    AppSettings.Instance.CurrentView = new HomeViewModel();
                }
            }

            if (this.WindowState == WindowState.Minimized || !this.IsVisible)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            this.Activate();
        }
        private void ExitApplication()
        {
            try
            {
                // Persist the window flag (same behavior as ShutdownFunction)
                try
                {
                    using (var context = new i_Freeze_WindowContext())
                    {
                        var flag = context.i_Freeze_Window.FirstOrDefault();
                        if (flag != null)
                        {
                            flag.Flag = 0;
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log but continue shutdown
                    File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"),
                        $"ExitApplication: failed to update i_Freeze_Window flag: {ex.Message}{Environment.NewLine}");
                    _ = DeviceManagement.SendLogs(ex.Message, "MainWindow.ExitApplication");
                }

                // Dispose tray icon so it disappears immediately
                try
                {
                    if (_notifyIcon != null)
                    {
                        _notifyIcon.Visible = false;
                        _notifyIcon.Dispose();
                        _notifyIcon = null;
                    }
                }
                catch { /* ignore */ }

                // Try to gracefully stop background work owned by MainWindowViewModel
                try
                {
                    if (this.DataContext is MainWindowViewModel mainVm)
                    {
                        mainVm.Dispose();
                    }
                    else if (System.Windows.Application.Current.MainWindow?.DataContext is MainWindowViewModel appMainVm)
                    {
                        appMainVm.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"),
                        $"ExitApplication: failed to dispose MainWindowViewModel: {ex.Message}{Environment.NewLine}");
                    _ = DeviceManagement.SendLogs(ex.Message, "MainWindow.ExitApplication.DisposeVM");
                }

                // Finally shut down the WPF application
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                // Last-resort: log and force exit
                File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"),
                    $"ExitApplication unexpected error: {ex.Message}{Environment.NewLine}");
                try { _notifyIcon?.Dispose(); } catch { }
                Environment.Exit(0);
            }
        }

        //private void ExitApplication()
        //{
        //    // Set the current view to LogoutViewModel to navigate to the login screen
        //    AppSettings.Instance.CurrentView = new LogoutViewModel();

        //    // Optionally, restore the window if it's minimized or hidden
        //    if (this.WindowState == WindowState.Minimized || !this.IsVisible)
        //    {
        //        this.Show();
        //        this.WindowState = WindowState.Normal;
        //    }
        //    this.Activate();
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Ensure proper cleanup
            _notifyIcon?.Dispose();
            base.OnClosed(e);
        }
    }
}