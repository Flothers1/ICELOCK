using i_freeze.DTOs;
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
using System.Threading.Tasks;

namespace i_freeze.Services
{
    public class PolicyService
    {
        private readonly HttpClient _http;
        public PolicyService(HttpClient http = null)
        {
            _http = http ?? new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            })
            { Timeout = TimeSpan.FromSeconds(30) };
        }
        public async Task<List<UserLabel>> FetchAndSaveUserPoliciesAsync(string username,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

            var url = $"https://security.flothers.com:8443/api/ScreenShotAnalyserPolicy/GetUserPolicies?username={Uri.EscapeDataString(username)}";
            HttpResponseMessage resp;
            try
            {
                resp = await _http.GetAsync(url, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HTTP error: " + ex.Message);
                throw;
            }
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine($"API returned {(int)resp.StatusCode}: {body}");
                throw new InvalidOperationException($"Failed to fetch policies: {resp.StatusCode}");
            }
            var json = await resp.Content.ReadAsStringAsync(cancellationToken);
            var dtos = JsonConvert.DeserializeObject<List<UserLabelDTO>>(json) ?? new List<UserLabelDTO>();

            var saved = new List<UserLabel>();
            using (var context = new PolicyContext())
            {
                foreach(var dto in dtos)
                {
                  
                    var exists = context.UserLabels.FirstOrDefault(u =>
                        u.GroupClassification == dto.groupClassification &&
                        u.PermissionClassification == dto.permissionClassification &&
                        u.DlpUserId == dto.dlpUserId);
              
                    if (exists != null)
                    {
                        // optional: update any fields that might have changed
                        exists.GroupClassification = dto.groupClassification;
                        exists.PermissionClassification = dto.permissionClassification;
                        exists.DlpUserId = dto.dlpUserId;
                        saved.Add(exists);
                        continue;
                    }
                    var entity = new UserLabel
                    {
                        GroupClassification = dto.groupClassification,
                        PermissionClassification = dto.permissionClassification,
                        DlpUserId = dto.dlpUserId
                    };

                    context.UserLabels.Add(entity);
                    saved.Add(entity);
                }
                await context.SaveChangesAsync(cancellationToken);
            }
            return saved;
        }
    }
}
