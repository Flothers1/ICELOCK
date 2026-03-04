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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace i_freeze.View
{
    /// <summary>
    /// Interaction logic for SettingsAdmin.xaml
    /// </summary>
    public partial class SettingsAdmin : UserControl
    {
        public SettingsAdmin()
        {
            InitializeComponent();
        }

        private void TamperProtection_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Prevent the toggle button from changing its state
            e.Handled = true;
        }
    }
}
