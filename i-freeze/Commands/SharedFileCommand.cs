using CommunityToolkit.Mvvm.Messaging;
using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Services;
using i_freeze.Utilities;
using i_freeze.ViewModel;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace i_freeze.Commands
{
    public class SharedFileCommand : CommandBase
    {
        private readonly IDialogService _dialogService;
        private const string ApiEndpoint = "https://security.flothers.com:8443/api/DLPSharedFile";
        private const string fileUrlBase = "http://158.220.90.131:8000/view?file=";
        //Shareable link pattern requested
        private const string ShareBase = "https://security.flothers.com/DlpFileAuthorization/OpenFile?fileName=";
        public SharedFileCommand(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }
        public override async void Execute(object parameter)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Title = "Choose file to share",
                    Filter = "PDF files (*.pdf)|*.pdf"
                };
                bool? dlgRes = dlg.ShowDialog();
                if (dlgRes != true) return;

                string selectedFilePath = dlg.FileName;
                string originalFileName = Path.GetFileName(selectedFilePath);
                //get the Owner email
                string userEmail = GetLoggedUserEmail();

                var credVm = new CredentialsDialogViewModel();
                bool? credRes = _dialogService.ShowDialog(credVm);
                if(credRes != true) return;

                string userName = credVm.UserName?.Trim() ?? string.Empty;
                string password = credVm.Password ?? string.Empty; 
                DateTime? expirationDate = credVm.NoExpiration ? null : credVm.ExpirationDate;

                //Build unique file name
                string uniqueCode = Guid.NewGuid().ToString("N").Substring(0, 4);
                string nameOnly = Path.GetFileNameWithoutExtension(originalFileName);
                string ext = Path.GetExtension(originalFileName);
                string sanitizedEmail  = userEmail;
                string uniqueFileName = $"{nameOnly}_{sanitizedEmail}_{uniqueCode}{ext}";
                string shareableUrl = ShareBase + Uri.EscapeDataString(uniqueFileName);
                string sharedAt = DateTime.UtcNow.ToString("o");
                var payload = new 
                {
                    fileName = uniqueFileName,
                    fileURL = fileUrlBase + Uri.EscapeDataString(uniqueFileName),
                    shareableUrl = shareableUrl,
                    fileOwner = userEmail,
                    userName = userName,
                    password = password,
                    sharedOn = sharedAt,
                    expirationDate = expirationDate?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };
                var jsonOptions = new JsonSerializerOptions {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                string json = JsonSerializer.Serialize(payload, jsonOptions);

                using var http = new HttpClient();
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await http.PostAsync(ApiEndpoint, content).ConfigureAwait(false);
                if (resp.IsSuccessStatusCode)
                {
                    //// copy to clipboard and notify on UI thread
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    try { Clipboard.SetText(shareableUrl); } catch { }
                    //    MessageBox.Show($"File metadata submitted successfully.\n\nShareable link (copied to clipboard):\n{shareableUrl}",
                    //        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    new ShowMessage
                      
                    //});
                    DateTime? expirationUtc = expirationDate?.ToUniversalTime();

                    await SaveSharedFileAsync(
                            originalFileName: originalFileName,
                            shareableUrl: shareableUrl,
                            password: password,
                            sharedWithEmail: userName,
                            sharedAt: sharedAt,
                            expirationUtc: expirationUtc
                        ).ConfigureAwait(false);
                    // --- run DLP_Uploadfile.exe (non-admin) ---
                    try
                    {
                        // location of DLP_Uploadfile.exe (change if needed)
                        string exePathA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DLP_Uploadfile.exe");
                        string exePath = "C:\\Users\\Public\\Ice Lock\\DLP_Uploadfile.exe";

                        if (!System.IO.File.Exists(exePath))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                                MessageBox.Show($"Upload tool not found: {exePath}", "Upload Missing", MessageBoxButton.OK, MessageBoxImage.Warning));
                        }
                        else
                        {
                            // helper local function to quote args safely
                            static string QuoteArg(string s)
                            {
                                if (s == null) return "\"\"";
                                return "\"" + s.Replace("\"", "\\\"") + "\"";
                            }

                            // build args: DLP_Uploadfile.exe "<filePath>" "<email>" "<code>"
                            string args = string.Join(" ",
                                QuoteArg(selectedFilePath),
                                QuoteArg(userEmail),
                                QuoteArg(uniqueCode)
                            );

                            using var runner = new i_freeze.Utilities.ExeRunner(exePath, args);

                            // run non-admin (this uses UseShellExecute = false path in the utilities ExeRunner)
                            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5)); // optional timeout
                            ProcessResult result = await runner.RunProcessAsync(cts.Token).ConfigureAwait(false);

                            // report result on UI thread
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (result == null)
                                {
                                    MessageBox.Show("Failed to start upload tool (null result). See logs.", "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                if (result.WasCancelled)
                                {
                                    MessageBox.Show("Upload cancelled (timed out or cancelled).", "Upload", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }

                                if (result.Success)
                                    MessageBox.Show("DLP_Uploadfile.exe completed successfully.", "Upload", MessageBoxButton.OK, MessageBoxImage.Information);
                                else
                                    MessageBox.Show($"DLP_Uploadfile.exe failed (exit code {result.ExitCode}).", "Upload", MessageBoxButton.OK, MessageBoxImage.Warning);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        try { System.IO.File.AppendAllText(@"C:\Users\Public\Ice Lock\Logs\i-freezeLogs.txt", $"{DateTime.UtcNow:o} - SharedFileCommand - Upload exe error - {ex}\r\n"); } catch { }
                        DeviceManagement.SendLogs(ex.Message, "SharedFileCommand Execute - uploadfile");
                        Application.Current.Dispatcher.Invoke(() =>
                            MessageBox.Show($"Unexpected error running upload tool: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                }
                else
                {
                    string err = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Application.Current.Dispatcher.Invoke(() =>
                        MessageBox.Show($"Failed to submit file metadata. Status: {resp.StatusCode}\n{err}", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
              
            }
            catch (Exception ex)
            {
                try { System.IO.File.AppendAllText(@"C:\Users\Public\Ice Lock\Logs\i-freezeLogs.txt", $"{DateTime.UtcNow:o} - SharedFileCommand - {ex}\r\n"); } catch { }
                DeviceManagement.SendLogs(ex.Message, "SharedFileCommand Execute");
                Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }

        private string GetLoggedUserEmail()
        {
            try
            {
                using (var ctx = new ActivateDLPContext())
                {
                    var ent = ctx.ActivateDLPEntity?.FirstOrDefault();
                    if (ent == null) return null;

                    var email = ent.Email;



                    return email;
                }
            }
            catch
            {
                return null;
            }
        }
        private async Task SaveSharedFileAsync(string originalFileName, string shareableUrl, string password, string sharedWithEmail, string sharedAt, DateTime? expirationUtc)
        {
            try
            {
                var record = new SharedFile {
                    FileName = originalFileName,
                    SharedWithEmail = sharedWithEmail,
                    Password = password,
                    Link = shareableUrl,
                    SharedAt = sharedAt,
                    ExpirationDate = expirationUtc?.ToString("o") ?? string.Empty
                };
                using (var ctx = new SharedFilesContext())
                {
                    ctx.SharedFiles.Add(record);
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // log and surface if needed
                try { System.IO.File.AppendAllText(@"C:\Users\Public\Ice Lock\Logs\i-freezeLogs.txt", $"{DateTime.UtcNow:o} - SaveSharedFileAsync - {ex}\r\n"); } catch { }
                DeviceManagement.SendLogs(ex.Message, "SaveSharedFileAsync");
                // optionally rethrow or show UI message on the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show("Warning: Failed to save shared-file record locally. See logs for details.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning));
            }
        }
    }
}

