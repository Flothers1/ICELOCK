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
using static System.Net.WebRequestMethods;

namespace i_freeze.Services
{
    public class PatternService
    {
        private readonly HttpClient _http;
        public PatternService(HttpClient httpClient = null)
        {
            _http = httpClient ?? new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            })
            { Timeout = TimeSpan.FromSeconds(60) };
        }
        /// <summary>
        /// Fetch patterns for the provided email and add any new patterns (does not remove existing rows).
        /// Returns the list of newly added PatternEntity rows.
        /// </summary>
        public async Task<List<PatternEntity>> FetchAndAppendPatternsAsync(string email,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
            var url = $"https://security.flothers.com:8443/api/PatternItem?email={Uri.EscapeDataString(email)}";
            HttpResponseMessage resp;
            try
            {
                resp = await _http.GetAsync(url, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PatternService HTTP error: " + ex.Message);
                throw;
            }

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine($"Pattern API returned {(int)resp.StatusCode}: {body}");
                throw new InvalidOperationException($"Failed to fetch patterns: {resp.StatusCode}");
            }
            var json = await resp.Content.ReadAsStringAsync(cancellationToken);
            var items = JsonConvert.DeserializeObject<List<PatternItemDTO>>(json) ?? new List<PatternItemDTO>();

            var toAdd = new List<PatternEntity>();
            var labelActionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                if (item?.labelGroup == null) continue;

                var labelName = item.labelGroup.labelName ?? string.Empty;
                var action = item.action ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(labelName))
                {
                    labelActionMap[labelName] = action;
                }
                if (item.labelGroup.patterns == null) continue;

                foreach (var pat in item.labelGroup.patterns)
                {
                    if (string.IsNullOrWhiteSpace(pat)) continue;

                    toAdd.Add(new PatternEntity
                    {
                        Pattern = pat,
                        Label = labelName,
                    });
                }
            }

            var distinctToAdd = toAdd
            .Where(p => !string.IsNullOrWhiteSpace(p.Pattern) && !string.IsNullOrWhiteSpace(p.Label))
            .GroupBy(p => new
            {
                Pattern = p.Pattern.Trim().ToLowerInvariant(),
                Label = p.Label.Trim().ToLowerInvariant()
            })
            .Select(g => g.First())
            .ToList();


            var newlyAdded = new List<PatternEntity>();
            using (var db = new Pattern_SearchContext())
            {
                // retrieve existing triples (Pattern, LabelName, Action) from DB for quick comparison
                // normalize whitespace/casing as you prefer
                var existingTriples = await db.Patterns
                    .Select(p => new { Pattern = p.Pattern, p.Label})
                    .ToListAsync(cancellationToken);

                var existingSet = new HashSet<string>(
                    existingTriples.Select(e => $"{e.Pattern}|||{e.Label}"),
                    StringComparer.OrdinalIgnoreCase);

                foreach (var candidate in distinctToAdd)
                {
                    var key = $"{candidate.Pattern}|||{candidate.Label}";
                    if (!existingSet.Contains(key))
                    {
                        // new => insert
                        newlyAdded.Add(candidate);
                        existingSet.Add(key); // avoid adding duplicates within this run
                    }
                }

                if (newlyAdded.Any())
                {
                    await db.Patterns.AddRangeAsync(newlyAdded, cancellationToken);
                    await db.SaveChangesAsync(cancellationToken);
                }
            }
            //Persist label actions in PolicyContext
            try
            {
                using (var db = new PolicyContext())
                {
                    foreach (var kv in labelActionMap)
                    {
                        var label = kv.Key;
                        var action = kv.Value ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(label))
                            continue;
                        var normalizedLabel = label.Trim().ToLower();

                        var existingPolicy = await db.Policies
                            .FirstOrDefaultAsync(p =>
                                p.Label != null &&
                                p.Label.ToLower() == normalizedLabel,
                                cancellationToken);


                        if (existingPolicy == null)
                        {
                            // insert new policy
                            var policy = new Policies
                            {
                                Label = label,
                                Action = action
                            };

                            // safe insert: if unique index exists, could throw on concurrent runs,
                            // but we already verified not found above (race still possible)
                            db.Policies.Add(policy);
                        }
                        else if (existingPolicy.Action != action)
                        {
                            // update existing policy action if different
                            existingPolicy.Action = action;
                            db.Policies.Update(existingPolicy);
                        }
                        await db.SaveChangesAsync(cancellationToken);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return newlyAdded;
        }
    }
}
