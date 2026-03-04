using i_freeze.Commands;
using i_freeze.Commands.HomeCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class DeviceRefreshMessageBoxViewModel : ViewModelBase
    {

        public string Message { get; set; }

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }

        public DeviceRefreshMessageBoxViewModel()
        {

        }

        public DeviceRefreshMessageBoxViewModel(string message)
        {
            Message = message;
            YesCommand = new DeviceRefreshCommand();
            NoCommand = new CloseWindowCommand();
        }

    }
}
