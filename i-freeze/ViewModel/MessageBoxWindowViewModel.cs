using i_freeze.Commands;
using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Utilities;
using i_freeze.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class MessageBoxWindowViewModel : ViewModelBase
    {

        public string Message { get; set; }

        public ICommand CloseCommand { get; }
        //public ICommand OkCommand { get; }

        public MessageBoxWindowViewModel() { 
            
        }

        public MessageBoxWindowViewModel(string message) {

            Message = message;
            CloseCommand = new CloseMessageBoxWindowCommand();
        }

    }
}
