using i_freeze.Utilities;
using i_freeze.View;
using Infrastructure.DataContext;
using Infrastructure.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class FilesProtectionViewModel : ViewModelBase, IDisposable
    {
        // Make this public static for backwards compatibility with XAML code-behind
        public static List<FilesProtections> _selectedList = new List<FilesProtections>();

        private bool _disposed = false;

        public ICommand LockFileCommand { get; }
        public ICommand UnlockFileCommand { get; }

        public FilesProtectionViewModel()
        {
            _blockedFiles = new ObservableCollection<FilesProtections>();
            LockFileCommand = new RelayCommand(Lock);
            UnlockFileCommand = new RelayCommand(Unlock);
            Read();
        }

        public async void Lock(object obj)
        {
            await AddLockAsync();
            RefreshFiles();
        }

        private async void Unlock(object obj)
        {
            await RemoveLockAsync();
            RefreshFiles();
        }

        private void RefreshFiles()
        {
            _blockedFiles.Clear();
            Read();
            BlockedFiles = _blockedFiles;
        }

        //----------------------- Binding ---------------------//
        private ObservableCollection<FilesProtections> _blockedFiles;
        public ObservableCollection<FilesProtections> BlockedFiles
        {
            get { return _blockedFiles; }
            set
            {
                _blockedFiles = value;
                OnPropertyChanged();
            }
        }

        private FilesProtections _file;
        public FilesProtections File
        {
            get { return _file; }
            set
            {
                _file = value;
                OnPropertyChanged();
            }
        }

        //----------------------- CRUD ------------------------//
        public List<FilesProtections> Database_BlockedFiles { get; private set; }

        public void Read()
        {
            try
            {
                using (var context = new FilesProtectionContext())
                {
                    Database_BlockedFiles = context.FilesProtection.ToList();
                    foreach (var file in Database_BlockedFiles)
                    {
                        _blockedFiles.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = DeviceManagement.SendLogs(ex.Message, "FilesProtectionViewModel Read method");
            }
        }

        //public async Task AddLockAsync()
        //{
        //    try
        //    {
        //        using (var folderBrowser = new FolderBrowserDialog())
        //        {
        //            folderBrowser.Description = "Select the folder to lock.";
        //            folderBrowser.ShowNewFolderButton = true;

        //            if (folderBrowser.ShowDialog() == DialogResult.OK)
        //            {
        //                string folderPath = folderBrowser.SelectedPath;

        //                if (string.IsNullOrWhiteSpace(folderPath))
        //                    return;

        //                using (var context = new FilesProtectionContext())
        //                {
        //                    if (!context.FilesProtection.Any(x => x.Path == folderPath))
        //                    {
        //                        string quotedPath = $"\"{folderPath}\"";
        //                        string exePath = "C:\\Users\\Public\\Ice Lock\\encrypt_folder.exe";

        //                        using (var runner = new ExeRunner(exePath, quotedPath))
        //                        {
        //                            await runner.RunProcessAsync();
        //                        }

        //                        context.FilesProtection.Add(new FilesProtections { Path = folderPath });
        //                        await context.SaveChangesAsync();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DeviceManagement.SendLogs(ex.Message, "FilesProtectionViewModel AddLockAsync method");
        //    }
        //}

        public async Task AddLockAsync()
        {
            try
            {

                string selectedPath = null;

                var dlg = new ChooseFileFolderWindow();
                dlg.Owner = System.Windows.Application.Current.MainWindow;
                bool? res = dlg.ShowDialog();
                if (string.IsNullOrWhiteSpace(dlg.SelectedPath))
                    return;

                 selectedPath = dlg.SelectedPath;

                if (string.IsNullOrWhiteSpace(selectedPath))
                    return;

                // Basic validation
                if (!System.IO.File.Exists(selectedPath) && !Directory.Exists(selectedPath))
                {
                    MessageBox.Show("The selected path doesn't exist.", "Invalid selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var context = new FilesProtectionContext())
                {
                    // avoid duplicates
                    if (!context.FilesProtection.Any(x => x.Path == selectedPath))
                    {
                        // Quote path so arguments with spaces are preserved
                        string quotedPath = $"\"{selectedPath}\"";
                        string exePath = @"C:\Users\Public\Ice Lock\encrypt_folder.exe"; // keep your exe path

                        using (var runner = new ExeRunner(exePath, quotedPath))
                        {
                            await runner.RunProcessAsync();
                        }

                        context.FilesProtection.Add(new FilesProtections { Path = selectedPath });
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        MessageBox.Show("This path is already protected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "FilesProtectionViewModel AddLockAsync method");
            }
        }

        public async Task RemoveLockAsync()
        {
            try
            {
                using (var context = new FilesProtectionContext())
                {
                    foreach (var selectedFile in _selectedList.ToList())
                    {
                        if (selectedFile != null)
                        {
                            var user = context.FilesProtection.FirstOrDefault(x => x.Id == selectedFile.Id);
                            if (user != null)
                            {
                                var path = user.Path;
                                string quotedPath = $"\"{path}\"";
                                string exePath = "C:\\Users\\Public\\Ice Lock\\decrypt_folder.exe";

                                using (var runner = new ExeRunner(exePath, quotedPath))
                                {
                                    await runner.RunProcessAsync();

                                }

                                context.Remove(user);
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "FilesProtectionViewModel RemoveLockAsync method");
            }
        }

        #region IDisposable Implementation
        public override void Dispose()
        {
            if (!_disposed)
            {
                _selectedList?.Clear();
                _blockedFiles?.Clear();
                _disposed = true;
            }
            base.Dispose();
        }
        #endregion
    }
}