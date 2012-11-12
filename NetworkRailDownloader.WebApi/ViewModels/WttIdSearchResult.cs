using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Console.WebApi.ViewModels
{
    public class WttIdSearchResult
    {
        public string TrainId { get; set; }
        public string HeadCode { get; set; }
        public string WttId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime? Depart { get; set; }
        public DateTime? Arrive { get; set; }
    }
}
