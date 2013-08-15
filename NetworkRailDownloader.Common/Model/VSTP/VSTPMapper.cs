using System.Collections.Generic;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Common.Model.VSTP
{
    public static class VSTPMapper
    {
        /// <exception cref="TiplocNotFoundException"></exception>
        public static ScheduleTrain ParseJsonVSTP(dynamic s, ICollection<TiplocCode> tiplocs)
        {
            return ScheduleTrainMapper.ParseJsonVSTPTrain(s.VSTPCIFMsgV1.schedule, tiplocs);
        }
    }
}
