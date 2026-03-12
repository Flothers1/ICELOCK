using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Utilities;
using i_freeze.View;
using Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class AdminLoginViewModel : ViewModelBase
    {
		private string _userName;

		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}
		private string _password;

		public string Password
		{
			get { return _password; }
			set { _password = value; }
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
        public ICommand LoginCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public AdminLoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
        }
        public AppConfigAndLoginContext context = new AppConfigAndLoginContext();

        private void ChangePassword(object obj)
        {

            var user = context.LoginModule.SingleOrDefault(u => u.Username == this.UserName);
            if (user != null && user.Password == this.Password)
            {
                // The entered username and password are correct
                AppSettings.Instance.CurrentView = new ChangePasswordViewModel();
                //Switcher.Switch(new ChangePassword());
            }
            else
            {
                // The entered username and password are incorrect
                new ShowMessage("The Username or password is incorrect");
                Password = "";
                UserName = "";


                //loginMessageBox myMessageBox = new loginMessageBox();
                //myMessageBox.ShowMessageBox("The Username or password is incorrect");
                //txtPassword.Clear();
                //txtUsername.Text = "";
                //txtUsername.Focus();
            }



        }

        private void Login(object obj)
        {
            AppConfigAndLoginContext context = new AppConfigAndLoginContext();

            var user = context.LoginModule.SingleOrDefault(u => u.Username == this.UserName);
            if (user != null && user.Password == this.Password)
            {
                // The entered username and password are correct
                AppSettings.Instance.CurrentView = new HomeViewModel();
                AppSettings.Instance.SwitcherView = new SwitcherAdminViewModel();

                //Switcher.Switch(new Home());
            }
            else
            {
                // The entered username and password are incorrect
                new ShowMessage("The Username or password is incorrect");
                Password = "";
                UserName = "";

                //loginMessageBox myMessageBox = new loginMessageBox();
                //myMessageBox.ShowMessageBox("The Username or password is incorrect");
                //txtPassword.Clear();
                //txtUsername.Text = "";
                //txtUsername.Focus();
            }



        }
    }
}
