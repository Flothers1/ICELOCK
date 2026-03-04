using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Utilities;
using i_freeze.View;
using i_freeze.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace i_freeze.Commands.HomeCommands
{
    public class ShowRefreshDeviceCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            DeviceRefreshMessageBox messageBox = new DeviceRefreshMessageBox()
            {
                DataContext = new DeviceRefreshMessageBoxViewModel(
                    "Are you sure you want to clear Windows temporary files to speed up your device?")
            };

            bool? result = messageBox.ShowDialog();

            if (result == true) // user clicked Yes
            {
                ClearTempFiles();
            }
        }

        private void ClearTempFiles()
        {
            try
            {
                // User temp folder
                string userTemp = Path.GetTempPath();

                // System temp folder
                string systemTemp = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

                CleanDirectory(userTemp);
                CleanDirectory(systemTemp);

                MessageBox.Show("Temporary files cleared successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing temporary files: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CleanDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal); // remove ReadOnly
                    File.Delete(file);
                }
                catch
                {
                    // Ignore files in use
                }
            }

            foreach (string dir in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                    {
                        Directory.Delete(dir, false); // delete empty folder
                    }
                }
                catch
                {
                    // Ignore locked folders
                }
            }
        }
    }
}