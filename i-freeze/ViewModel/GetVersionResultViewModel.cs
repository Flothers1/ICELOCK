using i_freeze.Commands;
using i_freeze.View;
using Infrastructure;
using Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace i_freeze.ViewModel
{
    public class GetVersionResultViewModel : ViewModelBase
    {

        public ICommand CloseCommand { get; }


        public GetVersionResultViewModel()
        {
            ForegroundColor = new SolidColorBrush(Colors.White);
            FontSize = 14;
            CloseCommand = new CloseMessageBoxWindowCommand();
            ShowVersionResult();
        }

        private string _review;
        public string Review
        {
            get { return _review; }

            set
            {
                _review = value;
                OnPropertyChanged();
            }
        }


        private System.Windows.Media.Brush _foregroundColor;
        public System.Windows.Media.Brush ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                _foregroundColor = value;
                OnPropertyChanged("ForegroundColor");
            }
        }

        private double _fontSize;
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                }
            }
        }

        void ShowVersionResult() // Function to Show Scan Result
        {
            string readText = File.ReadAllText(@"C:\Users\Public\Ice Lock\OutOfDateNew.txt"); // Read text that output of scan.exe 
            Review = readText;

            if (readText.Contains("Warning!") == true)
            {
                ForegroundColor = new SolidColorBrush(Colors.White);
            }
            else if (readText.Length == 0)
            {
                CloseWindowCommand.CloseWindow();
            }
            else
            {
                //ScanResult.VerticalAlignment = VerticalAlignment.Center;
                //ScanResult.HorizontalAlignment = HorizontalAlignment.Center;
                FontSize = 20;
                Review = "All applications are up to date";
            }
        }
    }
}
