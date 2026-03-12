
using i_freeze.Commands;
using i_freeze.Commands.UpgradeMessageBoxCommands;
using i_freeze.Services;
using i_freeze.Utilities;
using i_freeze.View;
using IceLockWorker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class SwitcherUserViewModel : ViewModelBase
    {
        public ICommand HomeCommand { get; }
        public ICommand LabeledFilesCommand { get; }
        public ICommand AdminAccessCommand { get; }

        public ICommand SettingsCommand { get; }
        public ICommand LabelsCommand { get; }
        public ICommand SharedFilesCommand { get; }
        public ICommand FilesProtectionCommand { get; }
        public ICommand DataControlsCommand { get; }

        public ICommand LogoutCommand { get; }

        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        private void Home(object obj) => AppSettings.Instance.CurrentView = new HomeViewModel();
        private void LabeledFiles(object obj) => AppSettings.Instance.CurrentView = new LabeledFilesViewModel();
        private void Admin(object obj) => AppSettings.Instance.CurrentView = new AdminLoginViewModel();

        private void Settings(object obj) => AppSettings.Instance.CurrentView = new SettingsViewModel();
        private void Labels(object obj) => AppSettings.Instance.CurrentView = new LabelsViewModel();
        private void SharedFiles(object obj) => AppSettings.Instance.CurrentView = new SharedFilesViewModel();
        private void FilesProtection(object obj) => AppSettings.Instance.CurrentView = new FilesProtectionViewModel();
        private void DataControls(object obj) => AppSettings.Instance.CurrentView = new DataControlsViewModel();


        private const string Remote_DesktopPathiFreeze = @"C:\Users\Public\Ice Lock\Remote_Desktop.exe";

        private const string Remote_DesktopNameiFreeze = "Remote_Desktop"; // Without the ".exe" part


        public SwitcherUserViewModel()
        {
            HomeCommand = new RelayCommand(Home);
            LabeledFilesCommand = new RelayCommand(LabeledFiles);
            AdminAccessCommand = new RelayCommand(Admin);
            SettingsCommand = new RelayCommand(Settings);
            LabelsCommand = new RelayCommand(Labels);
            SharedFilesCommand = new RelayCommand(SharedFiles);
            FilesProtectionCommand = new RelayCommand(FilesProtection);
            DataControlsCommand = new RelayCommand(DataControls);

            var licenseService = new LicenseService();
            LogoutCommand = new RelayCommand(async _ => await licenseService.LogoutAsync());

        }
    }
}
