using i_freeze.View;
using i_freeze.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace i_freeze.Services
{
    public class DialogService : IDialogService
    {
        public bool? ShowDialog(object viewModel)
        {
            if (viewModel is CredentialsDialogViewModel vm)
            {
                var wnd = new SharedFileCredentialsWindow
                {
                    DataContext = vm,
                    Owner = Application.Current?.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                return wnd.ShowDialog();
            }

            throw new InvalidOperationException("No window mapping for this view-model.");
        }
    }
}
