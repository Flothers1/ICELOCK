using Microsoft.Win32;
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
using Microsoft.WindowsAPICodePack.Dialogs;
namespace i_freeze.View
{
    /// <summary>
    /// Interaction logic for ChooseFileFolderWindow.xaml
    /// </summary>

        public partial class ChooseFileFolderWindow : Window
        {
            public string SelectedPath { get; private set; }

            public ChooseFileFolderWindow()
            {
                InitializeComponent();
            }

            private void CloseButton_Click(object sender, RoutedEventArgs e)
            {
            this.Hide();
            SelectedPath = null;
                Close();
            }
        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                SelectedPath = dialog.FileName;
            }
            Close();

        }
        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Select the folder to lock";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                SelectedPath = dialog.FileName;
            }
            Close();

        }
        private void RootBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // start dragging when left mouse button pressed
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
