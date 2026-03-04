using Infrastructure.DataContext;
using i_freeze.Utilities;
using i_freeze.View;
using i_freeze.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace i_freeze.Commands.MessageBoxCommands
{
    public class ShowMessage : CommandBase
    {

        
        public ShowMessage(string message) { 
            this.Execute(message);
        }

        public async override void Execute(object parameter)
        {
            try
            {
                MessageBoxWindow messageWindow = new MessageBoxWindow()
                {
                    DataContext = new MessageBoxWindowViewModel(parameter.ToString())
                };

                messageWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ShowMessage class Execute method");
            }
           
        }
    }
}
