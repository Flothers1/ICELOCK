using i_freeze.Commands.MessageBoxCommands;
using Infrastructure.DataContext;
using Infrastructure.Model;
using i_freeze.Utilities;
using i_freeze.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Infrastructure;

namespace i_freeze.ViewModel
{
    public class LogoutViewModel : ViewModelBase
    {

        private string _userName;
        public string UserName
        {
            get { return _userName; }

            set
            {
                _userName = value;
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

        public ICommand LogoutCommand { get; }
        public ICommand BtnBackCommand { get; }

        public LogoutViewModel()
        {
            LogoutCommand = new RelayCommand(Logout);
            BtnBackCommand = new RelayCommand(Back);
        }

        private void Logout(object obj)
        {
            using (AppConfigAndLoginContext context = new AppConfigAndLoginContext())
            {
                var user = context.LoginModule.SingleOrDefault(u => u.Username == this.UserName);
                if (user != null && user.Password == this.Password)
                {
                    //string[] processNames = { "i-FreezeWK", "i-FreezeWR", "i-FreezeNM", "i-FreezeAutoUSBS", "i-FreezeMP", "i-FreezePW", "i-FreezeSA", "i-FreezeNSPP", "i-FreezeFSS", "i-FreezeFSSBG", "i-FreezeUSBScan", "i-FreezeNSAD", "i-FreezeScanReg", "i-FreezeMPC", "i-FreezeMPM" };

                    //foreach (string processName in processNames)
                    //{
                    //    foreach (var process in Process.GetProcessesByName(processName))
                    //    {
                    //        process.Kill();
                    //    }
                    //}

                    //foreach (var process in Process.GetProcessesByName("i-Freeze"))
                    //{
                    //    process.Kill();
                    //}
                    string[] processNames = {  "i-FreezeWatch", "ServiceWatch" , "i-Freeze" };

                    foreach (string processName in processNames)
                    {
                        foreach (var process in Process.GetProcessesByName(processName))
                        {
                            process.Kill();
                        }
                    }

                    // The entered username and password are correct
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    // The entered username and password are incorrect
                    new ShowMessage("The Username or password is incorrect");
                    UserName = "";
                    Password = "";
                }
            }

        }

        private async void Back(object obj)
        {
            //try
            //{
            //    using (AppConfigAndLoginContext context = new AppConfigAndLoginContext())
            //    {
            //        var period = context.Application_Configuration.FirstOrDefault().Period;
            //        var checkdate = context.Application_Configuration.FirstOrDefault().CheckDate;
            //        var startDate = context.Application_Configuration.FirstOrDefault().StartDate;
            //        DateTime date1 = DateTime.Now;
            //        TimeSpan difference = checkdate.Subtract(date1);
            //        TimeSpan difference2 = date1.Subtract(startDate);
            //        if (period > 0 && period != null)
            //        {
            //            if (difference2.TotalMilliseconds > 0)
            //            {
            //                if (difference.TotalMilliseconds > 0)
            //                {
            //                    AppSettings.Instance.CurrentView = new HomeViewModel();
            //                }
            //                else
            //                {
            //                    UpdateExpirationDate();
            //                }

            //            }
            //            else
            //            {
            //                new ShowMessage("please adjust the date on your computer");
            //            }

            //        }
            //        else
            //        {
            //            new ShowMessage("please activate your license");
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    await DeviceManagement.SendLogs(ex.Message, "LogOut class btnBack_Click method");
            //}
             


        }

        //public async void UpdateExpirationDate()
        //{
        //    try
        //    {
        //        using (AppConfigAndLoginContext context = new AppConfigAndLoginContext())
        //        {
        //            Configurations user = context.Application_Configuration.FirstOrDefault();
        //            var activationKey = user.Key;

        //            var client = new HttpClient();
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await DeviceManagement.Token());
        //            var response = await client.GetAsync($"{DeviceManagement.MainURL}Licenses/GetExpirationDateById/" + activationKey + "");

        //            if (response.IsSuccessStatusCode)
        //            {

        //                // to update Expiration Date value of license in database => new value coming from Cloud
        //                DateTime ExpirationDate = await response.Content.ReadAsAsync<DateTime>();
        //                user.EndDate = ExpirationDate;
        //                DateTime date1 = DateTime.Now;
        //                DateTime date2 = user.EndDate;
        //                DateTime date3 = date1.AddDays(30);
        //                TimeSpan difference = date2.Subtract(date1);
        //                user.StartDate = date1;
        //                user.CheckDate = date3;
        //                user.Period = difference.TotalDays;
        //                context.SaveChanges();

        //                AppSettings.Instance.CurrentView = new HomeViewModel();
        //            }
        //            else
        //            {
        //                new ShowMessage("The license does not work.");

        //                foreach (var process in Process.GetProcessesByName("Blockusb"))
        //                {
        //                    process.Kill();
        //                }

        //                AppSettings.Instance.CurrentView = new ActivationViewModel();

        //            }
        //        }
                 


        //    }
        //    catch (Exception ex)
        //    {
        //        await DeviceManagement.SendLogs(ex.Message, "LogOut class UpdateExpirationDate method");
        //    }


        //}

    }
}
