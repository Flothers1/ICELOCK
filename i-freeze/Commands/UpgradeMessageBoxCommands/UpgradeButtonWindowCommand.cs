using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Commands.UpgradeMessageBoxCommands
{
    public class UpgradeButtonWindowCommand : CommandBase
    {
        public override void Execute(object parameter)
        {

            Reusable.OpenUrl("http://ifreeze.flothers.com/#plans");
            new CloseWindowCommand().Execute(parameter);
        }


    }
}
