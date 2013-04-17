using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class TrainMovementStepViewModel
    {
        [DataMember]
        public string TrainId { get; set; }

        [DataMember]
        public TrainMovementEventType EventTypeId { get; set; }

        [DataMember]
        public string EventType
        {
            get
            {
                switch (EventTypeId)
                {
                    case TrainMovementEventType.Arrival:
                        return "ARRIVAL";
                    case TrainMovementEventType.Departure:
                        return "DEPARTURE";
                }
                return null;
            }
            set
            {
                switch (value)
                {
                    case "ARRIVAL":
                        EventTypeId = TrainMovementEventType.Arrival;
                        break;
                    case "DEPARTURE":
                        EventTypeId = TrainMovementEventType.Departure;
                        break;
                }
            }
        }

        [DataMember]
        public DateTime? PlannedTime { get; set; }

        [DataMember]
        public DateTime ActualTimeStamp { get; set; }

        [DataMember]
        public string Stanox { get; set; }

        [DataMember]
        public TiplocCode Station { get; set; }

        [DataMember]
        public string Line { get; set; }

        [DataMember]
        public string Platform { get; set; }

        [DataMember]
        public byte? ScheduleStopNumber { get; set; }
    }
}
