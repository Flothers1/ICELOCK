
using i_freeze.Commands;
using i_freeze.Commands.HomeCommands;
using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Services;
using i_freeze.Utilities;
using Infrastructure;
using Infrastructure.DataContext;
using System.IO;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    class HomeViewModel : ViewModelBase
    {

        public ICommand RefreshCommand { get; }
        public ICommand SharedFile { get; }
        public ICommand ScanCommand { get; }
        public ICommand EncryptFilesCommand { get; }
        public ICommand SecurityUpdateCommand { get; }
        private readonly IDialogService _dialogService;
        AppConfigAndLoginContext _appConfigAndLoginContext;

        private string _version;
        public string Version
        {
            get { return _version; }
        }


        public HomeViewModel()
        {
            try
            {
                _dialogService = new DialogService();
                SharedFile = new SharedFileCommand(_dialogService);
                RefreshCommand = new ShowRefreshDeviceCommand();
                //ScanCommand = new ScanCommand();
                SecurityUpdateCommand = new SecurityUpdateCommand();

                _appConfigAndLoginContext = new AppConfigAndLoginContext();
                var vm = new FilesProtectionViewModel();
                EncryptFilesCommand = new RelayCommand(vm.Lock);


                //_version = _appConfigAndLoginContext.Application_Configuration.FirstOrDefault().VersionNumber.ToString();
            }
            catch (Exception ex)
            {
                //File.AppendAllText(
                //    Path.Combine(MainProjectPath.ProjectPath, "Logs", "i-freezeLogs.txt"),
                //    ex.Message
                //);

                File.AppendAllText(@"C:\Users\Public\Ice Lock\Logs\i-freezeLogs.txt", ex.Message);
                DeviceManagement.SendLogs(ex.Message, "HomeViewModel class HomeViewModel Constractor");
            }

        }


    }
}
