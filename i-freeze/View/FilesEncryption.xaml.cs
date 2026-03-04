using Infrastructure.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace i_freeze.View
{
    /// <summary>
    /// Interaction logic for FilesProtection.xaml
    /// </summary>
    public partial class FilesProtection : UserControl
    {
        public FilesProtection()
        {
            InitializeComponent();
        }

        public void GetSelectedList(object sender, SelectionChangedEventArgs e)
        {
            List<FilesProtections> _items = new List<FilesProtections>();
            var selectedItems = FilesList.SelectedItems;
            foreach (FilesProtections item in selectedItems)
            {
                _items.Add(item);
            }
            FilesProtectionViewModel._selectedList = _items;
        }
    }
}
