using i_freeze.Commands;
using i_freeze.Commands.HomeCommands;
using i_freeze.Commands.UpgradeMessageBoxCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class UpgradeBoxWindowViewModel : ViewModelBase
    {

        public string Message { get; set; }

        public ICommand CloseCommand { get; }
        public ICommand UpgradeCommand { get; }

        public UpgradeBoxWindowViewModel()
        {

        }

        public UpgradeBoxWindowViewModel(string message)
        {
            Message = message;
            CloseCommand = new CloseWindowCommand();
            UpgradeCommand = new UpgradeButtonWindowCommand();
        }

    }
}
