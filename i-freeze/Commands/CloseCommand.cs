using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace i_freeze.Commands
{
    public class CloseCommand : CommandBase
    {

        public override void Execute(object parameter)
        {
           // Application.Current.MainWindow.Hide();

            // Application.Current.Shutdown(); // Function that responsible for closing the application and stop running any process
            //Properties.Settings.Default.Save();   // function responsible to Save the last action in checkbox
            Application.Current.Shutdown();

        }

    }
}
