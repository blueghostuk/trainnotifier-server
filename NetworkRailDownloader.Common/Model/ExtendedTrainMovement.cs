using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model
{

    [DataContract]
    public class ViewModelTrainMovement
    {
        [IgnoreDataMember]
        public Guid UniqueId { get; set; }

        private ICollection<TrainMovementStepViewModel> _steps;

        [DataMember]
        public ScheduleTiploc Origin { get; set; }

        [DataMember]
        public DateTime? Activated { get; set; }

        [DataMember]
        public AtocCode AtocCode { get; set; }

        [DataMember]
        public ExtendedCancellation Cancellation { get; set; }

        [DataMember]
        public Reinstatement Reinstatement { get; set; }

        [DataMember]
        public ChangeOfOrigin ChangeOfOrigin { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string TrainId { get { return Id; } set { Id = value; } }

        [DataMember]
        public string HeadCode { get; set; }

        [DataMember]
        public string ServiceCode { get; set; }

        [DataMember]
        public DateTime? SchedOriginDeparture { get; set; }

        [DataMember]
        public string TrainUid { get; set; }

        public ViewModelTrainMovement()
        {
            _steps = new HashSet<TrainMovementStepViewModel>();
        }

        public void AddTrainMovementStep(TrainMovementStepViewModel step)
        {
            _steps.Add(step);
        }

        [DataMember]
        public IEnumerable<TrainMovementStepViewModel> Steps
        {
            get { return _steps; }
            set { _steps = new HashSet<TrainMovementStepViewModel>(value); }
        }
    }

    [DataContract]
    public class OriginTrainMovement : ViewModelTrainMovement
    {
        [DataMember]
        public ScheduleTiploc Destination { get; set; }
        [IgnoreDataMember]
        public Guid? ScheduleId { get; set; }
        [DataMember]
        public DateTime? ActualDeparture { get; set; }
        [DataMember]
        public DateTime? ActualArrival { get; set; }
    }

    [DataContract]
    public class CallingAtTrainMovement : OriginTrainMovement
    {
        [DataMember]
        public TimeSpan? Pass { get; set; }
    }

    [DataContract]
    public class CallingAtStationsTrainMovement : CallingAtTrainMovement
    {
        [DataMember]
        public TimeSpan? DestExpectedArrival { get; set; }
        [DataMember]
        public TimeSpan? DestExpectedDeparture { get; set; }
        [DataMember]
        public DateTime? DestActualDeparture { get; set; }
        [DataMember]
        public DateTime? DestActualArrival { get; set; }
    }
}
