using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class CredentialsDialogViewModel : ViewModelBase
    {
        private string _userName;
        private string _password;
        private DateTime? _expirationDate;
        private bool _noExpiration = true;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        // Password is populated by Window code-behind (PasswordBox -> VM)
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public DateTime? ExpirationDate
        {
            get => _expirationDate;
            set => SetProperty(ref _expirationDate, value);
        }

        public bool NoExpiration
        {
            get => _noExpiration;
            set
            {
                if (SetProperty(ref _noExpiration, value) && value)
                {
                    ExpirationDate = null;
                }
            }
        }

        // Event the view listens to in order to close the window and set DialogResult
        public event Action<bool?> RequestClose;

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public CredentialsDialogViewModel()
        {
            OkCommand = new RelayCommand(OnOk);     // replace RelayCommand with your command impl
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnOk()
        {
            // basic validation, adjust as needed
            if (string.IsNullOrWhiteSpace(UserName))
            {
                // If you have a validation mechanism, use it; otherwise notify user via dialog service caller
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                return;
            }

            RequestClose?.Invoke(true);
        }

        private void OnCancel()
        {
            RequestClose?.Invoke(false);
        }
    }

}
