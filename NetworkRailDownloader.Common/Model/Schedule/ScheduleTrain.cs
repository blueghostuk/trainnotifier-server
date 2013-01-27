using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrainNotifier.Common.Model.Schedule
{
    [DataContract]
    public sealed class ScheduleTrain
    {
        [DataMember]
        public Guid ScheduleId { get; set; }
        [DataMember]
        public string TrainUid { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public AtocCode AtocCode { get; set; }
        [DataMember]
        public Schedule Schedule { get; set; }
        [DataMember]
        public Status? Status { get; set; }
        [DataMember]
        public STPIndicator STPIndicator { get; set; }
        [DataMember]
        public TiplocCode Origin { get; set; }
        [DataMember]
        public TiplocCode Destination { get; set; }

        [DataMember]
        public IEnumerable<ScheduleStop> Stops { get; set; }
    }

}
