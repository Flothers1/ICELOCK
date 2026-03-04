using i_freeze.Utilities;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace i_freeze.ViewModel
{
    public class RemoteDesktopViewModel : ViewModelBase
    {


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });

            e.Handled = true;
        }

        public string IP
        {
            get { return GetIPAddress(); }

        }

        public ICommand ConnectCommand { get; }
        public ICommand AllowCommand { get; }
        public ICommand OpenLinkCommand { get; }



        public RemoteDesktopViewModel()
        {
            ConnectCommand = new RelayCommand(Connect);
            AllowCommand = new RelayCommand(Allow);
            OpenLinkCommand = new RelayCommand(OpenLink);

        }

        private string GetIPAddress()
        {
            string ipAddress = string.Empty;

            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                        foreach (UnicastIPAddressInformation address in ipProperties.UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ipAddress = address.Address.ToString();
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(ipAddress))
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                DeviceManagement.SendLogs(ex.Message, "DeviceManagement class GetIPAddress method");
            }

            return ipAddress;
        }

        private void Connect(object obj)
        {
            //string remoteClientPath = Path.Combine(MainProjectPath.ProjectPath, "RemoteDesktopClient");

            //ExeRunner runner = new ExeRunner(
            //    "cmd.exe",
            //    $"/C cd /d \"{remoteClientPath}\" && java Start.java"
            //);

            ExeRunner runner = new ExeRunner("cmd.exe", @"/C cd /d C:\Users\Public\Ice Lock\RemoteDesktopClient && java Start.java");
            runner.runprocess();
        }

        private void Allow(object obj)
        {
            //string remoteServerPath = Path.Combine(MainProjectPath.ProjectPath, "RemoteDesktopServer");

            //ExeRunner runner = new ExeRunner(
            //    "cmd.exe",
            //    $"/C cd /d \"{remoteServerPath}\" && java start.java"
            //);
            ExeRunner runner = new ExeRunner("cmd.exe", @"/C cd /d C:\Users\Public\Ice Lock\RemoteDesktopServer && java start.java");
            runner.runprocess();
        }

        private void OpenLink(object parameter)
        {
            if (parameter is string path)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        
    }
}
