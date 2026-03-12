using i_freeze.Commands.MessageBoxCommands;
using i_freeze.DTOs;
using i_freeze.Utilities;
using i_freeze.ViewModel;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization; // for CamelCasePropertyNamesContractResolver
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisioForge.Libs.ZXing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
namespace i_freeze.Services
{
    public class LicenseService
    {
        private readonly HttpClient _http;
        public LicenseService(HttpClient http = null) => _http = http ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        public async Task<ActivateDlpResponse> LoginAndActivateDlpAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
               // throw new ArgumentException("username and password required");
               new ShowMessage("Username and password are required.");

            // 1) Login call
            var loginUrl = "https://security.flothers.com:8443/api/Licenses/loginUsernameandPassReturnDLPlicense";
            var loginPayload = new LoginUsernamePassRequest { Email = email, Password = password };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginPayload), Encoding.UTF8, "application/json");

            var loginResp = await _http.PostAsync(loginUrl, loginContent);
            if (!loginResp.IsSuccessStatusCode)
            {
                var body = await loginResp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Login failed: {loginResp.StatusCode} - {body}");
            }

            var loginString = await loginResp.Content.ReadAsStringAsync();
            Guid licenseGuid = Guid.Empty;

            // parse login response like { "licenseId": "guid" }
            try
            {
                var j = JToken.Parse(loginString);
                if (j.Type == JTokenType.Object)
                {
                    var jObj = (JObject)j;
                    // case-insensitive search for "licenseId"
                    JProperty p = null;
                    foreach (var prop in jObj.Properties())
                        if (string.Equals(prop.Name, "licenseId", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(prop.Name, "license", StringComparison.OrdinalIgnoreCase))
                        { p = prop; break; }

                    if (p != null)
                    {
                        Guid.TryParse(p.Value.ToString(), out licenseGuid);
                    }
                }
                else if (j.Type == JTokenType.String)
                {
                    Guid.TryParse(j.ToString(), out licenseGuid);
                }
            }
            catch
            {
                // fallback regex
                var m = System.Text.RegularExpressions.Regex.Match(loginString, @"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}");
                if (m.Success) Guid.TryParse(m.Value, out licenseGuid);
            }

            if (licenseGuid == Guid.Empty)
                throw new InvalidOperationException("Could not parse license GUID from login response: " + loginString);

            // 2) Build device DTO (use your real implementation)
            var deviceActions = GetDeviceActionsFromDatabase();
            if (deviceActions == null)
                throw new InvalidOperationException("DeviceActions not found in DB. Call UpdateDeviceActions() first.");

            var deviceDto = new ActivateDeviceDlpDTO
            {
                DeviceIp = deviceActions.DeviceIp,
                MacAddress = deviceActions.MacAddress,
                SerialNumber = string.IsNullOrWhiteSpace(deviceActions.SerialNumber) ? null : deviceActions.SerialNumber,
                OperatingSystemVersion = deviceActions.OperatingSystemVersion,
                DeviceName = deviceActions.DeviceName,
                TypeOfLicense = "DLP"
            };

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var requestJson = JsonConvert.SerializeObject(deviceDto, serializerSettings);

            // DEBUG: write request JSON so you can compare to Postman
            Debug.WriteLine("ActivateDeviceDLP REQUEST JSON:");
            Debug.WriteLine(requestJson);

            // send


            // 3) ActivateDeviceDLP/{licenseGuid}
            var activateUrl = $"{DeviceManagement.MainURL}Licenses/ActivateDeviceDLP/{licenseGuid}";
            var activateContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var activateResp = await _http.PostAsync(activateUrl, activateContent);

            // DEBUG: status code
            Debug.WriteLine("ActivateDeviceDLP StatusCode: " + activateResp.StatusCode);


            if (!activateResp.IsSuccessStatusCode)
            {
                var body = await activateResp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"ActivateDeviceDLP failed: {activateResp.StatusCode} - {body}");
            }

            var activateString = await activateResp.Content.ReadAsStringAsync();
            Debug.WriteLine("ActivateDeviceDLP RESPONSE BODY:");
            Debug.WriteLine(activateString);
            // Parse activation response:
            // API may return array [true, "2025-12-31T16:47:00", "guid"] or object with fields.
            var result = new ActivateDlpResponse();
            try
            {
                var token = JToken.Parse(activateString);
                if (token.Type == JTokenType.Boolean)
                {
                    result.IsValid = token.ToString(); // "true"/"false"
                }
                else if (token.Type == JTokenType.Array)
                {
                    var arr = (JArray)token;
                  //  if (arr.Count >= 1) result.IsValid = arr[0].ToObject<bool>();
                    if (arr.Count >= 1) result.IsValid = arr[0].ToString();
                   // if (arr.Count >= 2 && DateTime.TryParse(arr[1].ToString(), out var dt)) result.ExpirationDate = dt;
                    if (arr.Count >= 2)  result.ExpirationDate = arr[1].ToString();
                    if (arr.Count >= 3)  result.DeviceId = arr[2].ToString();
                   
                }
                else if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                //    if (obj.TryGetValue("isValid", StringComparison.OrdinalIgnoreCase, out var v)) result.IsValid = v.ToObject<bool>();
                    if (obj.TryGetValue("isValid", StringComparison.OrdinalIgnoreCase, out var v)) result.IsValid = v.ToString();
                    if (obj.TryGetValue("expirationDate", StringComparison.OrdinalIgnoreCase, out var e)) result.ExpirationDate = e.ToString();
                    if (obj.TryGetValue("deviceId", StringComparison.OrdinalIgnoreCase, out var d)) result.DeviceId = d.ToString();
                }
                else
                {
                    result.IsValid = activateString.Trim();
                }
                if (result.IsValid == true.ToString() )
                {
                   using var db = new ActivateDLPContext();
                   await using var tx =await db.Database.BeginTransactionAsync();
                    try
                    {
                        var otherRows = await db.ActivateDLPEntity
                     .Where(a => !a.Email.ToLower().Equals(email.ToLower())).ToListAsync();
                        if (otherRows.Any())
                        {
                            db.ActivateDLPEntity.RemoveRange(otherRows);
                        }
                        var existing = await db.ActivateDLPEntity
                            .FirstOrDefaultAsync(a => a.Email.ToLower().Equals(email.ToLower()));
                        if (existing != null)
                        {
                            //update
                            existing.License = licenseGuid.ToString();
                            existing.IsValid = result.IsValid ?? string.Empty;
                            existing.ExpirationDate = result.ExpirationDate ?? string.Empty;
                            existing.DeviceId = result.DeviceId ?? string.Empty;
                            existing.CreatedAt = DateTime.UtcNow.ToString("O");
                            existing.Email = email;
                            existing.Password = password;
                        }
                        else
                        {
                            var entity = new ActivateDLPEntity
                            {
                                License = licenseGuid.ToString(),
                                IsValid = result.IsValid ?? string.Empty,
                                ExpirationDate = result.ExpirationDate ?? string.Empty,
                                DeviceId = result.DeviceId ?? string.Empty,
                                CreatedAt = DateTime.UtcNow.ToString("O"),
                                Email = email,
                                Password = password
                            };

                            db.ActivateDLPEntity.Add(entity);
                        }


                        await db.SaveChangesAsync();
                        await tx.CommitAsync();
                        return result;

                    }
                    catch (Exception exInner)
                    {
                        try { await tx.RollbackAsync(); } catch { /* ignore rollback errors */ }
                        Debug.WriteLine("SaveActivateDLPResponseAsync transaction failed: " + exInner);
                        await DeviceManagement.SendLogs(exInner.Message, "SaveActivateDLPResponseAsync");
                        throw;

                    }
                }
            }
            catch (Exception ex)
            {
                // parsing problem — still save raw response
                result.IsValid = activateString.Trim();
                Debug.WriteLine("Error parsing activation response: " + ex);
            }
      
            throw new InvalidOperationException("Could not parse ActivateDeviceDLP response: " + activateString);
        }

       
        public  DeviceActions GetDeviceActionsFromDatabase()
        {

            var deviceManagement = new DeviceManagement();
            deviceManagement.UpdateDeviceActions();
            using (var deviceContext = new DeviceContext())
            {
                DeviceActions deviceActions = deviceContext.DeviceActions.FirstOrDefault();

                if (deviceActions != null)
                {
                    return deviceActions;
                }
            }

            // Replace with your DB or local retrieval
            return new DeviceActions
            {
                DeviceIp = "127.0.0.1",
                MacAddress = "00-11-22-33-44-55",
                SerialNumber = "SN1234",
                OperatingSystemVersion = "10.0",
                DeviceName = "MyDevice"
            };
        }
        public async Task LogoutAsync()
        {
            var result = MessageBox.Show("Are you sure you want to log out? This will remove local policies, patterns, labels and settings.",
                              "Confirm logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            string email = null;
            string deviceId = null;
            try
            {

                using var actDb = new ActivateDLPContext();
                var act = await actDb.ActivateDLPEntity.FirstOrDefaultAsync();
                if (act != null)
                {
                    email = act.Email?.Trim();
                    deviceId = act.DeviceId?.Trim();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read ActivateDLPEntity: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "LogoutAsync.ReadActivateDLPEntity");
            }
            //Remove patterns and classification files
            try
            {
                using var sfctx = new SharedFilesContext();
                var allpatterns = sfctx.SharedFiles.ToList();
                if (allpatterns.Any())
                {
                    sfctx.SharedFiles.RemoveRange(allpatterns);
                }
                await sfctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clear the shared files: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "LogoutAsync.ClearPatternsPolicies");
            }
            //Remove patterns and classification files
            try
            {
                using var pctx = new Pattern_SearchContext();
                var allFileRules = pctx.file_rules.ToList();
                if (allFileRules.Any())
                {
                    pctx.file_rules.RemoveRange(allFileRules);
                }
                var allpatterns = pctx.Patterns.ToList();
                if (allpatterns.Any())
                {
                    pctx.Patterns.RemoveRange(allpatterns);
                }
                await pctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clear Patterns/Policies: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "LogoutAsync.ClearPatternsPolicies");
            }
            // clear labels and policies
            try
            {
                using var dctx = new DataClassificationConfigContext();
                var allsettings = await dctx.DataClassificationSettings.ToListAsync();
                if (allsettings.Any())
                {
                    dctx.DataClassificationSettings.RemoveRange(allsettings);
                    await dctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clear Settings: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "LogoutAsync.ClearUserLabelsSettings");
            }
            //clear labels and plicies
            try
            {
                using var pctx = new PolicyContext();
                var alllabels = await pctx.UserLabels.ToListAsync();
                if (alllabels.Any())
                {
                    pctx.UserLabels.RemoveRange(alllabels);
                }
                var allpolicies = await pctx.Policies.ToListAsync();
                if (allpolicies.Any())
                {
                    pctx.Policies.RemoveRange(allpolicies);
                }
                await pctx.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clear UserLabels/Policies: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "LogoutAsync.ClearUserLabelsPolicies");
            }
            // 4) Remove activation record
            try
            {
                using var actDb = new ActivateDLPContext();
                var allActs = await actDb.ActivateDLPEntity.ToListAsync();
                if (allActs.Any())
                {
                    actDb.ActivateDLPEntity.RemoveRange(allActs);
                    await actDb.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to clear ActivateDLPEntity: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "LogoutAsync.ClearActivateDLPEntity");
            }
            //    string[] processNames = { "DLP_CAF", "DLP_SA", "DLP_RCF", "DLP_PM" };

            try
            {
            
                try { 
                var serviceControl = new WindowsServiceControl();
                await serviceControl.BlockDLPProcess();

                // The entered username and password are correct
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "RunBlockDLPProcess");
                new ShowMessage("Failed to Block the Process: " + ex.Message);
            }
        }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop shut down: " + ex.Message);
                await DeviceManagement.SendLogs(ex.Message, "LogoutAsync.StopDLPProcesses");
            }


            // 6) Navigate back to activation/login view
            AppSettings.Instance.SwitcherView = null;
            AppSettings.Instance.CurrentView = new LoginViewModel();

            new ShowMessage("Logged out and local data cleared for the current user.");
        }
    }
}
