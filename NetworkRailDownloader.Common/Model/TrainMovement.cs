using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{
    [DataContract]
    public class TrainMovement : ITrainData
    {
        private ICollection<TrainMovementStep> _steps;
        private bool _activated,
            _cancelled,
            _terminated;

        [IgnoreDataMember]
        public Guid UniqueId { get; set; }

        [DataMember]
        public DateTime? Activated { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string TrainId { get { return Id; } set { Id = value; } }

        [DataMember]
        public string HeadCode { get; set; }

        [DataMember]
        public string ServiceCode { get; set; }
        [DataMember]
        public string SchedOriginStanox { get; set; }
        [DataMember]
        public DateTime? SchedOriginDeparture { get; set; }
        [DataMember]
        public string TocId { get; set; }
        [DataMember]
        public string WorkingTTId { get; set; }
        [DataMember]
        public string TrainUid { get; set; }

        [DataMember]
        public TrainState State { get; private set; }

        [IgnoreDataMember]
        public bool IsActivated
        {
            get { return _activated; }
            set
            {
                if (value != _activated)
                {
                    _activated = value;
                    if (_activated && State != TrainState.InProgress && State != TrainState.Cancelled)
                    {
                        State = TrainState.Activated;
                    }
                }
            }
        }

        [IgnoreDataMember]
        public bool Cancelled
        {
            get { return _cancelled; }
            set
            {
                if (value != _cancelled)
                {
                    _cancelled = value;
                    if (_cancelled)
                    {
                        State = TrainState.Cancelled;
                    }
                }
            }
        }

        [IgnoreDataMember]
        public bool Terminated
        {
            get { return _terminated; }
            set
            {
                if (value != _terminated)
                {
                    _terminated = value;
                    if (_terminated && State != TrainState.Cancelled)
                    {
                        State = TrainState.Terminated;
                    }
                }
            }
        }

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
            State = TrainState.Activated;
        }

        public void AddTrainMovementStep(TrainMovementStep step)
        {
            _steps.Add(step);
        }
    }

    [DataContract]
    public class ExtendedTrainMovement : TrainMovement
    {
        [DataMember]
        public ExtendedCancellation Cancellation { get; set; }
    }

    [DataContract]
    public class TrainMovementSchedule
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid? Schedule { get; set; }

        [DataMember]
        public byte StopNumber { get; set; }
    }

    [DataContract]
    public enum TrainState : short
    {
        [EnumMember()]
        Activated = 1,
        [EnumMember()]
        Cancelled = 2,
        [EnumMember()]
        InProgress = 3,
        [EnumMember()]
        Terminated = 4
    }

    public static class TrainMovementMapper
    {
        public static TrainMovement MapFromBody(dynamic body)
        {
            string wttId = null;
            try
            {
                wttId = (string)body.schedule_wtt_id;
            }
            catch { }
            string trainUid = null;
            try
            {
                trainUid = (string)body.train_uid;
            }
            catch { }
            return new TrainMovement
            {
                Activated = UnixTsToDateTime(double.Parse((string)body.creation_timestamp)),
                Id = (string)body.train_id,
                SchedOriginDeparture = UnixTsToDateTime(double.Parse((string)body.origin_dep_timestamp)),
                SchedOriginStanox = (string)body.sched_origin_stanox,
                ServiceCode = (string)body.train_service_code,
                TocId = (string)body.toc_id,
                WorkingTTId = wttId,
                TrainUid = trainUid
            };
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }
    }
}
