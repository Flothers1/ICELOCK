using i_freeze.Commands.MessageBoxCommands;
using Infrastructure.DataContext;
using i_freeze.Utilities;
using i_freeze.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Infrastructure;

namespace i_freeze.ViewModel
{
    public class ChangePasswordViewModel : ViewModelBase
    {

        private AppConfigAndLoginContext context = new AppConfigAndLoginContext();

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }

            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }

            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmCommand { get; }

        public ChangePasswordViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
        }


        private void Confirm(object obj)
        {
            string newPassword1 = NewPassword;
            string newPassword2 = ConfirmPassword;

            if (newPassword1 == newPassword2)
            {
                var user = context.LoginModule.SingleOrDefault(u => u.Id == 1);
                if (user != null)
                {
                    user.Password = newPassword1;
                    context.SaveChanges();
                    new ShowMessage("Password changed successfully!");
                    AppSettings.Instance.CurrentView = new LoginViewModel();

                    //loginMessageBox myMessageBox = new loginMessageBox();
                    //myMessageBox.ShowMessageBox("Password changed successfully!");
                    //Switcher.Switch(new Login());
                }
                else
                {
                    new ShowMessage("Error: User not found.");
                    //loginMessageBox myMessageBox = new loginMessageBox();
                    //myMessageBox.ShowMessageBox("Error: User not found.");
                }
            }
            else
            {
                new ShowMessage("Error: Passwords do not match.");
                //loginMessageBox myMessageBox = new loginMessageBox();
                //myMessageBox.ShowMessageBox("Error: Passwords do not match.");
            }
             

        }

    }
}
