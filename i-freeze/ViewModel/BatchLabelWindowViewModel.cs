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
using System.Windows.Forms;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class BatchLabelWindowViewModel : ViewModelBase
    {
        public ObservableCollection<BatchLabelItemViewModel> Items { get; } = new();

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set { _dialogResult = value; OnPropertyChanged(); }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public BatchLabelWindowViewModel(IEnumerable<string> paths)
        {
            foreach (var p in paths)
            {
                Items.Add(new BatchLabelItemViewModel
                {
                    FilePath = p,
                    FileName = Path.GetFileName(p),
                    Label = string.Empty
                });
            }

            OkCommand = new RelayCommand(_ => DialogResult = true);
            CancelCommand = new RelayCommand(_ => DialogResult = false);
        }
    }
}
