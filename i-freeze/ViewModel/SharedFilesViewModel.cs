using i_freeze.Commands.MessageBoxCommands;
using i_freeze.DTOs;
using i_freeze.Utilities;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class SharedFilesViewModel: ViewModelBase
    {
        public ObservableCollection<SharedFile> SharedFiles { get; } = new ObservableCollection<SharedFile>();

        private SharedFile _selectedFile;
        public SharedFile SelectedFile
        {
            get => _selectedFile;
            set { _selectedFile = value; OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand RemoveAllCommand { get; }

        public ICommand OpenLinkCommand { get; }
        public ICommand CopyLinkCommand { get; }

        public SharedFilesViewModel()
        {
            RefreshCommand = new RelayCommand(async _ => await LoadAsync());
            RemoveCommand = new RelayCommand(async _ => await RemoveSelectedAsync(), _ => SelectedFile != null);
            RemoveAllCommand = new RelayCommand(async _ => await RemoveAllAsync());

            OpenLinkCommand = new RelayCommand(async p => await OpenLinkAsync(p as SharedFile));
            CopyLinkCommand = new RelayCommand(p => CopyLink(p as SharedFile));

            // initial load
            _ = LoadAsync();
        }
        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                SharedFiles.Clear();

                using (var db = new SharedFilesContext())
                {
                    // pull everything (no ordering in SQL since SharedAt is text)
                    var items = await db.SharedFiles.ToListAsync(cancellationToken);

                    // sort in memory by parsing SharedAt (ISO format expected)
                    var ordered = items
                        .OrderByDescending(s =>
                        {
                            if (string.IsNullOrWhiteSpace(s.SharedAt)) return DateTime.MinValue;
                            if (DateTime.TryParse(s.SharedAt, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                                return dt;
                            // fallback: try general parse
                            if (DateTime.TryParse(s.SharedAt, out dt)) return dt;
                            return DateTime.MinValue;
                        })
                        .ToList();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var it in ordered)
                            SharedFiles.Add(it);
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadAsync failed: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "SharedFilesViewModel.LoadAsync");
                new ShowMessage("Failed to load shared files: " + ex.Message);
            }
        }

        private async Task OpenLinkAsync(SharedFile file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.Link))
            {
                new ShowMessage("No link available.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = file.Link,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OpenLinkAsync failed: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "SharedFilesViewModel.OpenLinkAsync");
                new ShowMessage("Could not open link: " + ex.Message);
            }
        }

        private void CopyLink(SharedFile file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.Link))
            {
                new ShowMessage("No link to copy.");
                return;
            }

            try
            {
                Clipboard.SetText(file.Link);
                new ShowMessage("Link copied to clipboard.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CopyLink failed: " + ex.Message);
                _ = DeviceManagement.SendLogs(ex.Message, "SharedFilesViewModel.CopyLink");
                new ShowMessage("Failed to copy link: " + ex.Message);
            }
        }
        private async Task RemoveSelectedAsync()
        {
            try
            {
                if (SelectedFile == null)
                {
                    new ShowMessage("No file selected");
                    return;
                }
                var confirm = MessageBox.Show($"Remove selected file '{SelectedFile.FileName}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm != MessageBoxResult.Yes) return;

                using (var db = new SharedFilesContext())
                {
                    var entity = await db.SharedFiles
                       .FirstOrDefaultAsync(s =>
                           s.FileName == SelectedFile.FileName &&
                           s.Link == SelectedFile.Link &&
                           s.SharedAt == SelectedFile.SharedAt);
                    if (entity == null)
                    {
                        // If not found in DB, remove from collection to keep UI consistent
                        Application.Current.Dispatcher.Invoke(() => SharedFiles.Remove(SelectedFile));
                        SelectedFile = null;
                        new ShowMessage("Selected file was not found in database but removed from the list.");
                        return;
                    }
                    db.SharedFiles.Remove(entity);
                    await db.SaveChangesAsync();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SharedFiles.Remove(SelectedFile);
                        SelectedFile = null;
                    });
                    new ShowMessage("Selected file removed.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RemoveSelectedAsync failed: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "SharedFilesViewModel.RemoveSelectedAsync");
                new ShowMessage("Remove selected failed: " + ex.Message);
            }
        }

        private async Task RemoveAllAsync()
        {
            try
            {
                var confirm = MessageBox.Show("Remove all shared files?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm != MessageBoxResult.Yes) return;

                using (var db = new SharedFilesContext())
                {
                    var all = await db.SharedFiles.ToListAsync();
                    if (!all.Any())
                    {
                        new ShowMessage("No shared files found.");
                        return;
                    }

                    db.SharedFiles.RemoveRange(all);
                    await db.SaveChangesAsync();

                    SharedFiles.Clear();

                    new ShowMessage($"Removed {all.Count} shared files.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RemoveAllAsync failed: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "SharedFilesViewModel.RemoveAllAsync");
                new ShowMessage("Remove all failed: " + ex.Message);
            }
        }

    }
}

