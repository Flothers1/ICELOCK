using i_freeze.Commands;
using i_freeze.Commands.CancelScans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    class ProgressBarViewModel : ViewModelBase
    {

        public ICommand CancelCommand { get; }
        public ICommand MinimizeCommand { get; }

        public ProgressBarViewModel()
        {
            CancelCommand = new CancelScan();
            MinimizeCommand = new MinimizeCommand();
        }

    }
}
