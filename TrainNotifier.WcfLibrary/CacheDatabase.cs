using System.Threading.Tasks;
using TrainNotifier.Common.Model;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.WcfLibrary
{
    public class CacheDatabase : DbRepository
    {
        public Task AddActivation(TrainMovement tm)
        {
            return Task.Factory.StartNew(() =>
            {
                const string insertActivation = @"
                    INSERT INTO LiveTrain
	                SET 
                        TrainId = @trainId,  
                        creation_timestamp = @activationDate, 
                        sched_origin_stanox = @Origin, 
                        origin_dep_timestamp = @OriginTime, 
                        train_service_code = @ServiceCode, 
                        toc_id = @tocId, 
                        train_uid = @trainUid, 
                        sched_wtt_id = @wttid";

                ExecuteNonQuery(insertActivation, new
                {
                    trainId = tm.Id,
                    activationDate = tm.Activated,
                    Origin = tm.SchedOriginStanox,
                    OriginTime = tm.SchedOriginDeparture,
                    ServiceCode = tm.ServiceCode,
                    tocId = tm.TocId,
                    trainUid = tm.TrainUid,
                    wttid = tm.WorkingTTId
                });
            });
        }

        public Task AddMovement(TrainMovement tm, TrainMovementStep tms)
        {
            return Task.Factory.StartNew(() =>
            {
                const string insertStop = @"
                    INSERT INTO LiveTrainStop
	                SET 
                        TrainId = @trainId,  
                        event_type = @eventType, 
                        planned_timestamp = @plannedTs, 
                        actual_timestamp = @actualTs, 
                        reporting_stanox = @stanox, 
                        platform = @platform,
                        line = @line,
                        train_terminated = @terminated";

                ExecuteNonQuery(insertStop, new
                {
                    trainId = tm.Id,
                    eventType = tms.EventType,
                    plannedTs = tms.PlannedTime,
                    actualTs = tms.ActualTimeStamp,
                    stanox = tms.Stanox,
                    platform = tms.Platform,
                    line = tms.Line,
                    terminated = tms.Terminated
                });
            });
        }

        public Task AddCancellation(TrainMovement tm, CancelledTrainMovementStep cm)
        {
            return Task.Factory.StartNew(() =>
            {
                const string insertStop = @"
                    INSERT INTO LiveTrainCancellation
	                SET 
                        TrainId = @trainId,  
                        canx_timestamp = @canxTs, 
                        train_service_code = @serviceCode, 
                        loc_stanox = @stanox, 
                        canx_reason_code = @canxReason, 
                        canx_type = @canxType";

                ExecuteNonQuery(insertStop, new
                {
                    trainId = tm.Id,
                    canxTs = cm.CancelledTime,
                    serviceCode = tm.ServiceCode,
                    stanox = cm.Stanox,
                    canxReason = cm.CancelledReasonCode
                });
            });
        }

    }
}
