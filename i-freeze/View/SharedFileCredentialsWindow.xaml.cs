using i_freeze.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace i_freeze.View
{
    /// <summary>
    /// Interaction logic for SharedFileCredentialsWindow.xaml
    /// </summary>
    public partial class SharedFileCredentialsWindow : Window
    {
        public SharedFileCredentialsWindow()
        {
            InitializeComponent();

            // subscribe when DataContext is set
            this.DataContextChanged += SharedFileCredentialsWindow_DataContextChanged;
        }

        private void SharedFileCredentialsWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CredentialsDialogViewModel oldVm)
                oldVm.RequestClose -= Vm_RequestClose;

            if (e.NewValue is CredentialsDialogViewModel vm)
            {
                vm.RequestClose += Vm_RequestClose;

                // ensure PasswordBox initial value sync if VM.Password already set
                if (!string.IsNullOrEmpty(vm.Password))
                    PasswordBox.Password = vm.Password;
            }
        }

        private void Vm_RequestClose(bool? dialogResult)
        {
            // Ensure this runs on UI thread
            Dispatcher.Invoke(() =>
            {
                this.DialogResult = dialogResult;
                this.Close();
            });
        }

        // tiny code-behind to push PasswordBox's value into VM.Password
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is CredentialsDialogViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }
    }
}
