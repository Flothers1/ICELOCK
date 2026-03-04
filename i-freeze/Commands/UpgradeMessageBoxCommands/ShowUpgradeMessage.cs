using i_freeze.Utilities;
using i_freeze.View;
using i_freeze.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Commands.UpgradeMessageBoxCommands
{
    public class ShowUpgradeMessage : CommandBase
    {
        public ShowUpgradeMessage() {
            //this.Execute(null);
        }

        public override void Execute(object parameter)
        {
            UpgradeBoxWindow messageWindow = new UpgradeBoxWindow()
            {
                DataContext = new UpgradeBoxWindowViewModel("This feature is not available in the free version." + Environment.NewLine +
                        "you can upgrade your license to freeze your risks.")
            };
            messageWindow.ShowDialog();
        }

    }
}
