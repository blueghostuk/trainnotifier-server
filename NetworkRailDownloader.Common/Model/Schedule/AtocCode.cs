using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public sealed class AtocCode
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
