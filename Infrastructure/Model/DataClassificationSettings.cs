using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class DataClassificationSettings
    {
        public int Id { get; set; }
        public string AutomaticClassification { get; set; }
        public string RealtimeClassification { get; set; }
        public string ScreenshotAnalyzer { get; set; }
        public string DecryptAllData { get; set; }

    }
}
