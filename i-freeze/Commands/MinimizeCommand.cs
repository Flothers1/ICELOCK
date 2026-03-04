using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace i_freeze.Commands
{
    public class MinimizeCommand : CommandBase
    {

        public override void Execute(object parameter)
        {
            Application.Current.Windows[0].WindowState = WindowState.Minimized;
        }
    }
}
