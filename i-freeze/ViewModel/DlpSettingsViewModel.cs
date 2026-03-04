using CommunityToolkit.Mvvm.Input;
using Infrastructure.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
     public class DlpSettingsViewModel : ViewModelBase
     {

    //    private FileItem _selectedFile;
    //    public FileItem SelectedFile
    //    {
    //        get => _selectedFile;
    //        set { _selectedFile = value; OnPropertyChanged(); }
    //    }

    //    public ICommand AddFilePathCommand { get; }
    //    public ICommand DeleteSelectedCommand { get; }
    //    public ICommand OpenSelectedCommand { get; }
    //    public ICommand ResetAllCommand { get; }
    //    public ICommand UpdateSettingsCommand { get; }

    //    public DlpSettingsViewModel()
    //    {
    //        // Sample data (replace with your real data source)
    //        MonitoredFiles.Add(new FileItem { FilePath = @"D:\Flothers\Code_Secure\DLP\test\i-Freeze.pdf", Label = "HR" });
    //        MonitoredFiles.Add(new FileItem { FilePath = @"D:\Flothers\Code_Secure\DLP\test\Peak_exes.txt", Label = "HR" });
    //        MonitoredFiles.Add(new FileItem { FilePath = @"D:\Flothers\Code_Secure\DLP\test\requirements.txt", Label = "HR" });
    //        MonitoredFiles.Add(new FileItem { FilePath = @"D:\Flothers\Code_Secure\DLP\test\i-Freeze Files.docx", Label = "HR" });

    //        AddFilePathCommand = new RelayCommand(AddFilePath);
    //        DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => SelectedFile != null);
    //        OpenSelectedCommand = new RelayCommand(OpenSelected, () => SelectedFile != null);
    //        ResetAllCommand = new RelayCommand(ResetAll);
    //        UpdateSettingsCommand = new RelayCommand(UpdateSettings);
    //    }

    //    private void AddFilePath()
    //    {
    //        var dlg = new OpenFileDialog { CheckFileExists = true, Multiselect = false };
    //        if (dlg.ShowDialog() == true)
    //        {
    //            MonitoredFiles.Add(new FileItem { FilePath = dlg.FileName, Label = "HR" });
    //        }
    //    }

    //    private void DeleteSelected()
    //    {
    //        if (SelectedFile == null) return;
    //        MonitoredFiles.Remove(SelectedFile);
    //        SelectedFile = null;
    //    }

    //    private void OpenSelected()
    //    {
    //        if (SelectedFile == null) return;
    //        try
    //        {
    //            if (!File.Exists(SelectedFile.FilePath))
    //            {
    //                MessageBox.Show("File not found: " + SelectedFile.FilePath, "Open", MessageBoxButton.OK, MessageBoxImage.Warning);
    //                return;
    //            }

    //            var psi = new ProcessStartInfo
    //            {
    //                FileName = SelectedFile.FilePath,
    //                UseShellExecute = true
    //            };
    //            Process.Start(psi);
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show("Could not open file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //        }
    //    }

    //    private void ResetAll()
    //    {
    //        if (MessageBox.Show("Clear all monitored files?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
    //        {
    //            MonitoredFiles.Clear();
    //        }
    //    }

    //    private void UpdateSettings()
    //    {
    //        // Replace this with your save logic
    //        MessageBox.Show("Settings updated (" + MonitoredFiles.Count + " items).", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
    //    }
    }
}

