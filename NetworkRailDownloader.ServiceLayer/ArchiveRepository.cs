using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Service
{
    public class ArchiveRepository : DbRepository
    {
        public ArchiveRepository()
            : base("archive")
        { }

        public IEnumerable<dynamic> GetBtWttId(string wttId)
        {
            const string sql = @"
                SELECT
                    TrainId,
                    OriginDepartTimestamp,
                    OriginStanox,
                    SchedWttId
                FROM LiveTrain
                WHERE SchedWttId LIKE @wttId";

            wttId += "%";

            return Query<dynamic>(sql, new { wttId });
        }

        public TrainMovement GetTrainMovementById(string trainId)
        {
            const string sql = @"
                SELECT TOP 1
                    Id AS UniqueId,
                    TrainId AS Id,
                    CreationTimestamp AS Activated,
                    OriginDepartTimestamp AS SchedOriginDeparture,
                    TrainServiceCode AS ServiceCode,
                    Toc AS TocId,
                    TrainUid AS TrainUid,
                    OriginStanox AS SchedOriginStanox,
                    SchedWttId AS WorkingTTId
                FROM LiveTrain
                WHERE TrainId = @trainId
                ORDER BY SchedWttId";

            TrainMovement tm = ExecuteScalar<TrainMovement>(sql, new { trainId });
            if (tm != null)
            {
                const string tmsSql = @"
                    SELECT
                        EventType,
                        PlannedTimestamp AS PlannedTime,
                        ActualTimestamp AS ActualTimeStamp,
                        ReportingStanox AS Stanox,
                        Platform AS Platform,
                        Line AS Line,
                        TrainTerminated AS Terminated
                    FROM LiveTrainStop
                    WHERE TrainId = @trainId";

                IEnumerable<TrainMovementStep> tmSteps = Query<TrainMovementStep>(tmsSql, new { trainId = tm.UniqueId })
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
            Trace.TraceInformation("Saving Activation: {0}", tm.TrainId);
            const string insertActivation = @"
                INSERT INTO [natrail].[dbo].[LiveTrain]
                           ([TrainId]
                           ,[Headcode]
                           ,[CreationTimestamp]
                           ,[OriginDepartTimestamp]
                           ,[TrainServiceCode]
                           ,[Toc]
                           ,[TrainUid]
                           ,[OriginStanox]
                           ,[SchedWttId])
                     VALUES
                           (@trainId
                           ,@headcode
                           ,@activationDate
                           ,@OriginTime
                           ,@ServiceCode
                           ,@tocId
                           ,@trainUid
                           ,@Origin
                           ,@wttid)";

            ExecuteNonQuery(insertActivation, new
            {
                trainId = tm.Id,
                headcode = tm.Id.Substring(2, 4),
                activationDate = tm.Activated,
                Origin = tm.SchedOriginStanox,
                OriginTime = tm.SchedOriginDeparture,
                ServiceCode = tm.ServiceCode,
                tocId = tm.TocId,
                trainUid = tm.TrainUid,
                wttid = tm.WorkingTTId
            });
        }

        private bool TrainExists(string trainId, out Guid? id)
        {
            id = ExecuteScalar<Guid?>("SELECT Id FROM LiveTrain WHERE TrainId = @trainId", new { trainId });
            return id.HasValue && id.Value != Guid.Empty;
        }

        public bool AddMovement(TrainMovementStep tms)
        {
            Guid? trainId = null;
            if (TrainExists(tms.TrainId, out trainId))
            {
                Trace.TraceInformation("Saving Movement to: {0}", tms.TrainId);
                const string insertStop = @"
                    INSERT INTO [natrail].[dbo].[LiveTrainStop]
                               ([TrainId]
                               ,[EventType]
                               ,[PlannedTimestamp]
                               ,[ActualTimestamp]
                               ,[ReportingStanox]
                               ,[Platform]
                               ,[Line]
                               ,[TrainTerminated])
                         VALUES
                               (@trainId
                               ,@eventType
                               ,@plannedTs
                               ,@actualTs
                               ,@stanox
                               ,@platform
                               ,@line
                               ,@terminated)";

                ExecuteNonQuery(insertStop, new
                {
                    trainId,
                    eventType = tms.EventType,
                    plannedTs = tms.PlannedTime,
                    actualTs = tms.ActualTimeStamp,
                    stanox = tms.Stanox,
                    platform = tms.Platform,
                    line = tms.Line,
                    terminated = tms.State == State.Terminated
                });

                return true;
            }
            return false;
        }

        public bool AddCancellation(CancelledTrainMovementStep cm)
        {
            Guid? trainId = null;
            if (TrainExists(cm.TrainId, out trainId))
            {
                Trace.TraceInformation("Saving Cancellation to: {0}", cm.TrainId);
                const string insertStop = @"
                    INSERT INTO [natrail].[dbo].[LiveTrainCancellation]
                               ([TrainId]
                               ,[CancelledTimestamp]
                               ,[ReasonCode]
                               ,[Stanox]
                               ,[Type])
                         VALUES
                               (@trainId
                               ,@canxTs
                               ,@canxReason
                               ,@stanox
                               ,@canxType)";

                ExecuteNonQuery(insertStop, new
                {
                    trainId,
                    canxTs = cm.CancelledTime,
                    canxType = cm.CancelledType,
                    canxReason = cm.CancelledReasonCode,
                    stanox = cm.Stanox
                });

                return true;
            }
            return false;
        }

        public IEnumerable<TrainMovement> GetTrainMovementsOrigin(string stanox)
        {
            const string sql = @"
                SELECT
                    TrainId AS Id,
                    CreationTimestamp AS Activated,
                    OriginDepartTimestamp AS SchedOriginDeparture,
                    TrainServiceCode AS ServiceCode,
                    Toc AS TocId,
                    TrainUid AS TrainUid,
                    OriginStanox AS SchedOriginStanox,
                    SchedWttId AS WorkingTTId
                FROM LiveTrain
                WHERE OriginStanox = @stanox
                ORDER BY OriginDepartTimestamp";

            IEnumerable<TrainMovement> tms = Query<TrainMovement>(sql, new { stanox });

            // TODO: get more detail
            return tms;                
        }
    }
}
