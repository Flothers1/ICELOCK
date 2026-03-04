using i_freeze.Commands.MessageBoxCommands;
using i_freeze.DTOs;
using i_freeze.Utilities;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace i_freeze.Services
{
    public class ClassificationSettingsService
    {
        private readonly HttpClient _http;

        public ClassificationSettingsService(HttpClient http = null)
        {
            _http = http ?? new HttpClient(new HttpClientHandler
            {
                // keep consistent with other code that allows self-signed certs
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            })
            { Timeout = TimeSpan.FromSeconds(30) };
        }
        /// <summary>
        /// Fetches DLP user settings for `username` and upserts a single settings row
        /// into DataClassificationConfigContext.DataClassificationSettings.
        /// </summary>
        public async Task<DataClassificationSettings> FetchAndSaveClassificationSettingsAsync
            (string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            var url = $"https://security.flothers.com:8443/api/DlpUserSettings?email={Uri.EscapeDataString(email)}";
            try
            {
                var resp = await _http.GetAsync(url, cancellationToken);
                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Settings API returned {(int)resp.StatusCode}: {body}");
                    throw new InvalidOperationException($"Failed to fetch settings: {resp.StatusCode}");
                }
                var json = await resp.Content.ReadAsStringAsync(cancellationToken);
                var dto = JsonConvert.DeserializeObject<DlpUserSettingsDTO>(json)
                    ?? throw new InvalidOperationException("Invalid settings response.");
                using (var db = new DataClassificationConfigContext())
                {
                    var existing = await db.DataClassificationSettings.FirstOrDefaultAsync(cancellationToken);
                    // Convert booleans to string representation (you store strings in the model)
                    //function to convert bool to "true"/"false"
                    string ToTxt(bool v) => v.ToString().ToLowerInvariant();
                    if (existing == null)
                    {
                        existing = new DataClassificationSettings
                        {
                            AutomaticClassification = ToTxt(dto.AutomaticClassification),
                            RealtimeClassification = ToTxt(dto.RealtimeClassification),
                            ScreenshotAnalyzer = ToTxt(dto.ScreenshotAnalyzer),
                            DecryptAllData = ToTxt(dto.DecryptAllData)
                        };
                        db.DataClassificationSettings.Add(existing);
                    }
                    else
                    {
                        existing.AutomaticClassification = ToTxt(dto.AutomaticClassification);
                        existing.RealtimeClassification = ToTxt(dto.RealtimeClassification);
                        existing.ScreenshotAnalyzer = ToTxt(dto.ScreenshotAnalyzer);
                        existing.DecryptAllData = ToTxt(dto.DecryptAllData);

                        db.DataClassificationSettings.Update(existing);
                    }
                    await db.SaveChangesAsync(cancellationToken);
                    return existing;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FetchAndSaveSettingsAsync failed: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "SettingsSyncService.FetchAndSaveSettingsAsync");
                throw;
            }
        }


        public async Task ApplyClassificationSettingsAsync(DataClassificationSettings settings)
        {
            var serviceControl = new WindowsServiceControl();

            // 1️⃣ AutomaticClassification → DLP_CAF.exe
            if (string.Equals(settings.AutomaticClassification, "true", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await serviceControl.RunDLP_CAF();
                }
                catch (Exception ex)
                {
                    await DeviceManagement.SendLogs(ex.Message, "RunDLP_CAF");
                    new ShowMessage("Failed to start DLP_CAF: " + ex.Message);
                }
            }

            // 2️⃣ ScreenshotAnalyzer → DLP_SA.exe
            if (string.Equals(settings.ScreenshotAnalyzer, "true", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await serviceControl.RunDLP_SA();
                }
                catch (Exception ex)
                {
                    await DeviceManagement.SendLogs(ex.Message, "RunDLP_SA");
                    new ShowMessage("Failed to start DLP_SA: " + ex.Message);
                }
            }

            // 3️⃣ RealtimeClassification → DLP_RCF.exe
            if (string.Equals(settings.RealtimeClassification, "true", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await serviceControl.RunDLP_RCF();
                }
                catch (Exception ex)
                {
                    await DeviceManagement.SendLogs(ex.Message, "RunDLP_RCF");
                    new ShowMessage("Failed to start DLP_RCF: " + ex.Message);
                }
            }
        }
    }
}
