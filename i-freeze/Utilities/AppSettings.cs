using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace i_freeze.Utilities
{
    // class that contains all static variables
    public class AppSettings : INotifyPropertyChanged
    {
        private static AppSettings _instance;


        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }

            set
            {

                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private object _switcherView;
        public object SwitcherView
        {
            get { return _switcherView; }

            set
            {

                _switcherView = value;
                OnPropertyChanged(nameof(SwitcherView));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private AppSettings()
        {
        }

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSettings();
                }
                return _instance;
            }
        }
    }
}
