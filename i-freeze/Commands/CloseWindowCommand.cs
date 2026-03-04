using i_freeze.Utilities;
using Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace i_freeze.Commands
{
    public class CloseWindowCommand : CommandBase
    {

        //public override void Execute(object parameter)
        //{

        //    Application.Current.Windows[Application.Current.Windows.Count - 1].Hide();



        //}


        //public override void Execute(object parameter)
        //{
        //    // Ensure we're interacting with the UI thread
        //    var currentWindow = Application.Current.MainWindow;

        //    // Check if the current window is null
        //    if (currentWindow != null)
        //    {
        //        // Hide the current window
        //        currentWindow.Hide();
        //    }
        //}

        public override void Execute(object parameter)
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            if (currentWindow != null)
            {
                currentWindow.Close();
            }
        }

        public static void CloseWindow()
        {
            Application.Current.Windows[Application.Current.Windows.Count - 1].Close();
        }

    }
}
