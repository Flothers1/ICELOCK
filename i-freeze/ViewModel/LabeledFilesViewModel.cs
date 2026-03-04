using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Utilities;
using i_freeze.View;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class LabeledFilesViewModel : ViewModelBase
    {
        public LabeledFilesViewModel()
        {
            MonitoredFiles = new ObservableCollection<file_rules>();

            AddFilePathCommand = new RelayCommand(async o =>await AddFilePath());
            DeleteSelectedCommand = new RelayCommand(async o =>await DeleteSelected(), o => SelectedFile != null);
            OpenSelectedCommand = new RelayCommand(o => OpenSelected(), o => SelectedFile != null);
            ResetAllCommand = new RelayCommand(async o =>await ResetAll());
            UpdateSettingsCommand = new RelayCommand(async o => await UpdateSettingsAsync());

            // load existing settings (fire-and-forget is ok here; UI will update when load finishes)
            _ = LoadAsync();
        }

        #region Bindable Properties

        private ObservableCollection<file_rules> _monitoredFiles;
        public ObservableCollection<file_rules> MonitoredFiles
        {
            get => _monitoredFiles;
            set
            {
                _monitoredFiles = value;
                OnPropertyChanged();
            }
        }

        private file_rules _selectedFile;
        public file_rules SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
                // raise CanExecute updates
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion

        #region Commands

        public ICommand AddFilePathCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand OpenSelectedCommand { get; }
        public ICommand ResetAllCommand { get; }
        public ICommand UpdateSettingsCommand { get; }

        #endregion

        #region Command Handlers

        private async Task AddFilePath()
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = true,
                CheckFileExists = true
            };

            if (dlg.ShowDialog() != true)
                return;

            var vm = new BatchLabelWindowViewModel(dlg.FileNames);

            var win = new BatchLabelWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            if (win.ShowDialog() == true)
            {
                using var db = new Pattern_SearchContext();
                //using var activationDlp = new ActivateDLPContext();
                //var loggedInUser =await activationDlp.ActivateDLPEntity.FirstOrDefaultAsync();
                //string email = string.Empty;
                //if (loggedInUser != null)
                //{
                //     email = loggedInUser.Email;
                //}
                 
                foreach (var item in vm.Items)
                {
                    if (!MonitoredFilesExists(item.FilePath))
                    {
                        var entity = new file_rules
                        {
                            file_path = item.FilePath,
                            label = item.Label,
                            
                        };

                        db.file_rules.Add(entity);
                        //Safe to call from any thread (background worker, Task, etc.)
                        //
                        Application.Current.Dispatcher.Invoke(() => MonitoredFiles.Add(entity));
                        await db.SaveChangesAsync();

                    }
                }

            }
        }

        private async Task DeleteSelected()
        {
            if (SelectedFile == null) return;

            try
            {
                using var db = new Pattern_SearchContext();
                var toRemove = await db.file_rules
                    .FirstOrDefaultAsync(f => f.Id == SelectedFile.Id);
                if(toRemove != null)
                {
                    db.file_rules.Remove(toRemove);
                    await db.SaveChangesAsync();
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MonitoredFiles.Remove(SelectedFile);
                    SelectedFile = null;
                });
            }
            catch (Exception ex)
            {
                try { new ShowMessage($"Error deleting file: {ex.Message}"); }
                catch { MessageBox.Show($"Error deleting file: {ex.Message}"); }
            }
        }

        private void OpenSelected()
        {
            if (SelectedFile == null) return;

            try
            {
                var info = new ProcessStartInfo(SelectedFile.file_path)
                {
                    UseShellExecute = true
                };
                Process.Start(info);
            }
            catch (Exception ex)
            {
                try { new ShowMessage($"Cannot open file: {ex.Message}"); }
                catch { MessageBox.Show($"Cannot open file: {ex.Message}"); }
            }
        }

        private async Task ResetAll()
        {
            var confirm = MessageBox.Show("Reset all labeled files? This will remove all entries.", "Confirm Reset",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    using var db = new Pattern_SearchContext();
                    db.file_rules.RemoveRange(db.file_rules);
                    await db.SaveChangesAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MonitoredFiles.Clear();
                        SelectedFile = null;
                    });
                }
                catch (Exception ex)
                {
                    try { new ShowMessage($"Error resetting files: {ex.Message}"); }
                    catch { MessageBox.Show($"Error resetting files: {ex.Message}"); }
                }
            }
        }

        private Task UpdateSettingsAsync()
        {
            try
            {
                try { new ShowMessage("Settings updated."); }
                catch { MessageBox.Show("Settings updated."); }
            }
            catch (Exception ex)
            {
                try { new ShowMessage($"Error saving settings: {ex.Message}"); }
                catch { MessageBox.Show($"Error saving settings: {ex.Message}"); }
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Persistence (JSON)

        private bool MonitoredFilesExists(string filePath)
        {
            foreach (var f in MonitoredFiles)
            {
                if (string.Equals(f.file_path, filePath, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private async Task LoadAsync()
        {
            try
            {
                using var ctx = new Pattern_SearchContext();
                var rows = await ctx.file_rules.AsNoTracking().OrderBy(r => r.Id).ToListAsync();

                // Update collection on UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MonitoredFiles.Clear();
                    foreach (var r in rows)
                    {
                        MonitoredFiles.Add(new file_rules
                        {
                            file_path = r.file_path,
                            label = r.label
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                try { await DeviceManagement.SendLogs(ex.Message, "LabeledFilesViewModel.LoadAsync"); }
                catch { /* swallow */ }
            }
        }

     
        #endregion
    }

}

