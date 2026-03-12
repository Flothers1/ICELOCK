using i_freeze.Services;
using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class SwitcherAdminViewModel : ViewModelBase
    {
        public ICommand HomeCommand { get; }
        public ICommand LabeledFilesCommand { get; }
        public ICommand AdminAccessCommand { get; }

        public ICommand SettingsCommand { get; }
        public ICommand LabelsCommand { get; }
        public ICommand SharedFilesCommand { get; }
        public ICommand FilesProtectionCommand { get; }

        public ICommand LogoutCommand { get; }
        public SwitcherAdminViewModel()
        {
            HomeCommand = new RelayCommand(Home);
            LabeledFilesCommand = new RelayCommand(LabeledFiles);
            AdminAccessCommand = new RelayCommand(Admin);
            SettingsCommand = new RelayCommand(Settings);
            LabelsCommand = new RelayCommand(Labels);
            SharedFilesCommand = new RelayCommand(SharedFiles);
            FilesProtectionCommand = new RelayCommand(FilesProtection);

            var licenseService = new LicenseService();
            LogoutCommand = new RelayCommand(async _ => await licenseService.LogoutAsync());

        }
        private void Home(object obj) => AppSettings.Instance.CurrentView = new HomeViewModel();
        private void LabeledFiles(object obj) => AppSettings.Instance.CurrentView = new LabeledFilesViewModel();
        private void Admin(object obj) => AppSettings.Instance.CurrentView = new LoginViewModel();

        private void Settings(object obj) => AppSettings.Instance.CurrentView = new SettingsViewModel();
        private void Labels(object obj) => AppSettings.Instance.CurrentView = new LabelsViewModel();
        private void SharedFiles(object obj) => AppSettings.Instance.CurrentView = new SharedFilesViewModel();
        private void FilesProtection(object obj) => AppSettings.Instance.CurrentView = new FilesProtectionViewModel();

    }
}
