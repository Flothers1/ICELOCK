using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{

   
        public class BatchLabelItemViewModel : ViewModelBase
        {
            private string _label;

            public string FilePath { get; set; }
            public string FileName { get; set; }

            public string Label
            {
                get => _label;
                set { _label = value; OnPropertyChanged(); }
            }


        }

    
}
