using CommunityToolkit.Mvvm.Input;
using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Services;
using i_freeze.Utilities;
using i_freeze.View;
using Infrastructure;
using Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace i_freeze.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _email;
        public string Email
        {
            get { return _email; }

            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }

            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool _isFocus;
        public bool IsFocus
        {
            get { return _isFocus; }

            set
            {
                _isFocus = value;
                OnPropertyChanged();
            }
        }

         private readonly LicenseService _licenseService = new LicenseService();

        public ICommand LoginCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand<object>(async p => await LoginAsync(p));
        }


        private async Task LoginAsync(object parameter)
        {
            var password = parameter as string; // from CommandParameter
            try
            {
                var result = await _licenseService.LoginAndActivateDlpAsync(this.Email, this.Password);
                if (result != null && result.IsValid.ToLower()== "true")
                {
                   new ShowMessage($"Welcome to ICE LOCK\r\nData Security Controls are now active");
               
                }
                else
                {
                   new ShowMessage("Activation failed or license invalid.");
                }
                //Initaialize the app setup
                await SetupTheApplicationAsync();
                AppSettings.Instance.SwitcherView = new SwitcherUserViewModel();
                AppSettings.Instance.CurrentView = new HomeViewModel();
            }
            catch (Exception ex)
            {
                new ShowMessage("Error: " + ex.Message);
            }
        }
        private async Task SetupTheApplicationAsync()
        {
            try
            {
                // Initialize main window background tasks
                if (Application.Current.MainWindow is MainWindow mainWindow &&
                    mainWindow.DataContext is MainWindowViewModel mainViewModel)
                {
                    mainViewModel.MainWindow_Loaded();
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "LoginViewModel SetupTheApplicationAsync");
            }
        }
    }


    // Generic async relay command that takes a parameter
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private bool _running;
        public AsyncRelayCommand(Func<T, Task> execute) => _execute = execute;
        public bool CanExecute(object parameter) => !_running;
        public async void Execute(object parameter)
        {
            _running = true; RaiseCanExecuteChanged();
            try { await _execute((T)parameter); }
            finally { _running = false; RaiseCanExecuteChanged(); }
        }
        public event EventHandler CanExecuteChanged;
        private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

