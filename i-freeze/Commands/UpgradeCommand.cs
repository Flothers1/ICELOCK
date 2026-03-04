using i_freeze.Utilities;
using i_freeze.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Commands
{
    public class UpgradeCommand : CommandBase
    {
        public UpgradeCommand() 
        {
            ////MainWindowViewModel viewModel = new MainWindowViewModel()
            ////{
            ////    CurrentView = new HomeViewModel()
            ////};
            ////this.Execute(null);
        }
        public override void Execute(object parameter)
        {
            Reusable.OpenUrl("http://ifreeze.flothers.com/#plans");
        }
    }
}
