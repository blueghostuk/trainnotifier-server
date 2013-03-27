using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.NMSModel
{
    [DataContract]
    public class RtppmData
    {
        [DataMember]
        public DateTime Timestamp { get; set; }

    }
}
