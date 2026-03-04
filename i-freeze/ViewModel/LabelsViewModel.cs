using i_freeze.Utilities;
using Infrastructure.DataContext; // contains PolicyContext
using Infrastructure.Model;       // contains UserLabel
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class LabelsViewModel : ViewModelBase
    {
        public ObservableCollection<string> PermittedLabels { get; }
        public ObservableCollection<string> ClassificationLabels { get; }

        public ICommand RefreshCommand { get; }

        public LabelsViewModel()
        {
            PermittedLabels = new ObservableCollection<string>();
            ClassificationLabels = new ObservableCollection<string>();

            RefreshCommand = new RelayCommand(async _ => await LoadAsync());

            // initial load
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            try
            {
                using var ctx = new PolicyContext();

                var permitted = await ctx.Set<UserLabel>()
                    .AsNoTracking()
                    .Where(u => !string.IsNullOrWhiteSpace(u.PermissionClassification))
                    .Select(u => u.PermissionClassification.Trim())
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();

                var classification = await ctx.Set<UserLabel>()
                    .AsNoTracking()
                    .Where(u => !string.IsNullOrWhiteSpace(u.GroupClassification))
                    .Select(u => u.GroupClassification.Trim())
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    PermittedLabels.Clear();
                    foreach (var p in permitted) PermittedLabels.Add(p);

                    ClassificationLabels.Clear();
                    foreach (var c in classification) ClassificationLabels.Add(c);
                });
            }
            catch (Exception ex)
            {
                // non-fatal: log if possible
                try { await DeviceManagement.SendLogs(ex.Message, "LabelsViewModel.LoadAsync"); } catch { }
            }
        }
    }
}
