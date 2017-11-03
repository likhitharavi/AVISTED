using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVISTED.Models
{
    public class ExtractInfo
    {
        public string parameters { get; set; }
        public double latmin { get; set; }
        public double latmax { get; set; }
        public double lonmin { get; set; }
        public double lonmax { get; set; }
        public string stat { get; set; }
        public string path { get; set; }
        public string format { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string outFormat { get; set; }
        public Boolean saveDownload { get; set; }
        public string fileName { get; set; }
    }
}
