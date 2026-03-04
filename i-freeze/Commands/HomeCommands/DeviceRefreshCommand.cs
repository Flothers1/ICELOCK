using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Utilities;
using i_freeze.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace i_freeze.Commands.HomeCommands
{
    public class DeviceRefreshCommand : CommandBase
    {
        //public override void Execute(object parameter)
        //{
        //    ExeRunner runner = new ExeRunner("CMD.exe", "/c del /s /q \"C:\\Windows\\Temp\"");
        //    runner.runprocess();
        //    Application.Current.Windows[Application.Current.Windows.Count - 1].Close();
        //    new ShowMessage("Windows temporary files are cleared.\nYour device is faster now");
        //}
        
            public override void Execute(object parameter)
            {
                ExeRunner runner = new ExeRunner("CMD.exe", "/c del /s /q \"C:\\Windows\\Temp\"");
                runner.runprocess();

                var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                currentWindow?.Close();

                new ShowMessage("Windows temporary files are cleared.\nYour device is faster now");
            }
        
    }
}
