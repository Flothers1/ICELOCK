using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Services
{
    public interface IDialogService
    {
        /// <summary>Shows a dialog for the given view-model and returns the DialogResult (bool?)</summary>
        bool? ShowDialog(object viewModel);
    }
}
