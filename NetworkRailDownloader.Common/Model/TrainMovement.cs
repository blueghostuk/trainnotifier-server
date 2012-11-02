using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NetworkRailDownloader.Common.Model
{
    [DataContract]
    public sealed class TrainMovement
    {
        private ICollection<TrainMovementStep> _steps;

        [DataMember]
        public DateTime? Activated { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string ServiceCode { get; set; }
        [DataMember]
        public string SchedOriginStanox { get; set; }
        [DataMember]
        public DateTime? SchedOriginDeparture { get; set; }

        [DataMember]
        public IEnumerable<TrainMovementStep> Steps
        {
            get
            {
                if (_steps == null)
                    return Enumerable.Empty<TrainMovementStep>();

                return _steps
                    .OrderBy(s => s.ActualTimeStamp);
            }
            set
            {
                if (value != null)
                {
                    _steps = new HashSet<TrainMovementStep>(value);
                }
                else
                {
                    _steps = new HashSet<TrainMovementStep>();
                }
            }
        }

        public TrainMovement()
        {
            _steps = new HashSet<TrainMovementStep>();
        }

        public void AddTrainMovementStep(TrainMovementStep step)
        {
            _steps.Add(step);
        }

    }
}
