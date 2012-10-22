using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRailDownloader.WebApi.ViewModels
{
    public class Stanox
    {
        public string Tiploc { get; set; }
        public string Nalco { get; set; }
        public string Description { get; set; }
        public string CRS { get; set; }
        public string StationName { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
    }
}
