using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using i_freeze.Commands;
using i_freeze.Utilities;

namespace i_freeze.ViewModel
{
    public class MessageBoxWindowYesNoViewModel : ViewModelBase
    {

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }
        public ICommand CloseCommand { get; }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message)); 
            }
        }


        public MessageBoxWindowYesNoViewModel()
        {
            YesCommand = new RelayCommand<object>(ExecuteYes);
            NoCommand = new RelayCommand<object>(ExecuteNo);
            CloseCommand = new RelayCommand<object>(CloseWindow);

        }
        private void CloseWindow(object parameter)
        {
            // Close the window
            Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.IsActive)?.Close();
        }

        private void ExecuteYes(object parameter)
        {
            CloseDialog(true);
        }

        private void ExecuteNo(object parameter)
        {
            CloseDialog(false);
        }

        private void CloseDialog(bool result)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is View.MessageBoxWindowYesNo)
                {
                    window.DialogResult = result;
                    window.Close();
                    break;
                }
            }
        }
    }
}
