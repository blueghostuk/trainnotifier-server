using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public sealed class TiplocCode
    {
        [DataMember]
        public short TiplocId { get; set; }
        [DataMember]
        public string Tiploc { get; set; }
        [DataMember]
        public string Nalco { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Stanox { get; set; }
        [DataMember]
        public string CRS { get; set; }
    }
}
