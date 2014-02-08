using Newtonsoft.Json;
using System.Collections.Generic;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.CorpusExtract
{
    public class TiplocContainer
    {
        public IEnumerable<Tiploc> TIPLOCDATA { get; set; }
    }

    public class Tiploc
    {
        public string STANOX { get; set; }
        public string UIC { get; set; }
        [JsonProperty(PropertyName="3ALPHA")]
        public string ALPHA { get; set; }
        public string TIPLOC { get; set; }
        public string NLC { get; set; }
        public string NLCDESC { get; set; }
        public string NLCDESC16 { get; set; }

        public TiplocCode ToTiplocCode()
        {
            return new TiplocCode
            {
                Tiploc = TIPLOC.Trim(),
                Stanox = STANOX.Trim(),
                Nalco = string.IsNullOrWhiteSpace(NLC) ? null : NLC.Trim(),
                Description = string.IsNullOrWhiteSpace(NLCDESC) ? null : NLCDESC.Trim(),
                CRS = string.IsNullOrWhiteSpace(ALPHA) ? null : ALPHA.Trim()
            };
        }
    }
}
