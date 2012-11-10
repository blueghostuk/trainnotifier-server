using System.Collections.Generic;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Service
{
    public class ArchiveRepository : DbRepository
    {
        public IEnumerable<dynamic> GetBtWttId(string wttId)
        {
            const string sql = @"
                SELECT
                    `TrainId` AS `Id`,
                    `origin_dep_timestamp`,
                    `sched_origin_stanox`,
                    `sched_wtt_id`
                FROM `LiveTrain`
                WHERE `sched_wtt_id` LIKE @wttId";

            wttId += "%";

            return Query<dynamic>(sql, new { wttId });
        }

        public TrainMovement GetTrainMovementById(string trainId)
        {
            const string sql = @"
                SELECT
                    `TrainId` AS `Id`,
                    `creation_timestamp` AS `Activated`,
                    `origin_dep_timestamp` AS `SchedOriginDeparture`,
                    `train_service_code` AS `ServiceCode`,
                    `toc_id` AS `TocId`,
                    `train_uid` AS `TrainUid`,
                    `sched_origin_stanox` AS `SchedOriginStanox`,
                    `sched_wtt_id` AS `WorkingTTId`
                FROM `LiveTrain`
                WHERE `TrainId` = @trainId
                ORDER BY `sched_wtt_id` LIMIT 1";

            TrainMovement tm = ExecuteScalar<TrainMovement>(sql, new { trainId });
            if (tm != null)
            {
                const string tmsSql = @"
                    SELECT
                        `event_type` AS `EventType`,
                        `planned_timestamp` AS `PlannedTime`,
                        `actual_timestamp` AS `ActualTimeStamp`,
                        `reporting_stanox` AS `Stanox`,
                        `platform` AS `Platform`,
                        `line` AS `Line`,
                        `train_terminated` AS `Terminated`
                    FROM `LiveTrainStop`
                    WHERE `TrainId` = @trainId";

                IEnumerable<TrainMovementStep> tmSteps = Query<TrainMovementStep>(tmsSql, new { trainId })
                    .ToList();
                foreach (var tms in tmSteps)
                {
                    if (tms.Terminated)
                        tms.State = State.Terminated;
                }

                tm.Steps = tmSteps;

                return tm;
            }

            return null;
        }

        public void AddActivation(TrainMovement tm)
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
        }

        private bool TrainExists(string trainId)
        {
            return ExecuteScalar<object>("SELECT 1 FROM `LiveTrain` WHERE `TrainId` = @trainId", new { trainId }) != null;
        }

        public void AddMovement(string trainId, TrainMovementStep tms)
        {
            if (TrainExists(trainId))
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
                    trainId = trainId,
                    eventType = tms.EventType,
                    plannedTs = tms.PlannedTime,
                    actualTs = tms.ActualTimeStamp,
                    stanox = tms.Stanox,
                    platform = tms.Platform,
                    line = tms.Line,
                    terminated = tms.State == State.Terminated
                });
            }
        }

        public void AddCancellation(string trainId, CancelledTrainMovementStep cm)
        {
            if (TrainExists(trainId))
            {
                const string insertStop = @"
                    INSERT INTO LiveTrainCancellation
	                SET 
                        TrainId = @trainId,  
                        canx_timestamp = @canxTs, 
                        loc_stanox = @stanox, 
                        canx_reason_code = @canxReason, 
                        canx_type = @canxType";

                ExecuteNonQuery(insertStop, new
                {
                    trainId = trainId,
                    canxTs = cm.CancelledTime,
                    stanox = cm.Stanox,
                    canxReason = cm.CancelledReasonCode
                });
            }
        }
    }
}
