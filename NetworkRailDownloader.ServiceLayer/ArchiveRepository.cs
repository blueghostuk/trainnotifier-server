﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using TrainNotifier.Common.Model;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Service
{
    public class ArchiveRepository : DbRepository
    {
        public ArchiveRepository()
            : base("archive")
        { }

        public IEnumerable<dynamic> SearchByWttId(string wttId)
        {
            const string sql = @"
                SELECT
                    TrainId,
                    Headcode,
                    OriginDepartTimestamp,
                    OriginStanox,
                    SchedWttId
                FROM LiveTrain
                WHERE SchedWttId LIKE @wttId";

            wttId += "%";

            return Query<dynamic>(sql, new { wttId });
        }

        public IEnumerable<TrainMovement> SearchByOrigin(string stanox)
        {
            const string sql = @"
                SELECT
                    TrainId AS Id,
                    Headcode AS HeadCode,
                    CreationTimestamp AS Activated,
                    OriginDepartTimestamp AS SchedOriginDeparture,
                    TrainServiceCode AS ServiceCode,
                    Toc AS TocId,
                    TrainUid AS TrainUid,
                    OriginStanox AS SchedOriginStanox,
                    SchedWttId AS WorkingTTId
                FROM LiveTrain
                WHERE OriginStanox = @stanox 
                    AND OriginDepartTimestamp >= DATEADD(day, -1, GETDATE())
                ORDER BY OriginDepartTimestamp";

            IEnumerable<TrainMovement> tms = Query<TrainMovement>(sql, new { stanox });

            // TODO: get more detail
            return tms;
        }

        public IEnumerable<TrainMovement> TrainsCallingAtStation(string stanox)
        {
            const string sql = @"
                SELECT
                    [LiveTrain].[TrainId] AS Id,
                    Headcode AS HeadCode,
                    CreationTimestamp AS Activated,
                    OriginDepartTimestamp AS SchedOriginDeparture,
                    TrainServiceCode AS ServiceCode,
                    Toc AS TocId,
                    TrainUid AS TrainUid,
                    OriginStanox AS SchedOriginStanox,
                    SchedWttId AS WorkingTTId
                FROM LiveTrain
                INNER JOIN LiveTrainStop ON LiveTrain.Id = LiveTrainStop.TrainId
                WHERE LiveTrainStop.ReportingStanox = @stanox 
                    AND OriginDepartTimestamp >= DATEADD(day, -1, GETDATE())
                ORDER BY OriginDepartTimestamp";

            IEnumerable<TrainMovement> tms = Query<TrainMovement>(sql, new { stanox });

            // TODO: get more detail
            return tms;
        }

        public IEnumerable<TrainMovement> SearchByHeadcode(string headcode)
        {
            const string sql = @"
                SELECT
                    TrainId AS Id,
                    Headcode AS HeadCode,
                    CreationTimestamp AS Activated,
                    OriginDepartTimestamp AS SchedOriginDeparture,
                    TrainServiceCode AS ServiceCode,
                    Toc AS TocId,
                    TrainUid AS TrainUid,
                    OriginStanox AS SchedOriginStanox,
                    SchedWttId AS WorkingTTId
                FROM LiveTrain
                WHERE Headcode = @headcode 
                    aAND OriginDepartTimestamp >= DATEADD(day, -1, GETDATE())
                ORDER BY OriginDepartTimestamp";

            IEnumerable<TrainMovement> tms = Query<TrainMovement>(sql, new { headcode });

            // TODO: get more detail
            return tms;
        }

        public TrainMovement GetTrainMovementById(string trainId)
        {
            const string sql = @"
                SELECT TOP 1
                    Id AS UniqueId,
                    TrainId AS Id,
                    Headcode AS HeadCode,
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
                           ,[SchedWttId]
                           ,[StateId])
                     VALUES
                           (@trainId
                           ,@headcode
                           ,@activationDate
                           ,@OriginTime
                           ,@ServiceCode
                           ,@tocId
                           ,@trainUid
                           ,@Origin
                           ,@wttid
                           ,@state)";

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
                wttid = tm.WorkingTTId,
                state = tm.State
            });
        }

        private bool TrainExists(string trainId, out Guid? dbId)
        {
            dbId = ExecuteScalar<Guid?>("SELECT Id FROM LiveTrain WHERE TrainId = @trainId", new { trainId });
            return dbId.HasValue && dbId.Value != Guid.Empty;
        }

        private bool RunningTrainExists(string id, out Guid? dbId)
        {
            try
            {
                dbId = ExecuteScalar<Guid?>("SELECT Id FROM LiveTrain WHERE Headcode = @id AND StateId = @state", new { id, state = TrainState.InProgress });
                return dbId.HasValue && dbId.Value != Guid.Empty;
            }
            catch (InvalidOperationException) { } // TODO: more than one found
            dbId = null;
            return false;
        }

        public bool AddMovement(TrainMovementStep tms)
        {
            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
            //{
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

                    UpdateTrainState(trainId.Value, tms.State == State.Terminated ? TrainState.Terminated : TrainState.InProgress);

                    //ts.Complete();
                    return true;
                }
            //}
            return false;
        }

        public bool AddCancellation(CancelledTrainMovementStep cm)
        {
            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
            //{
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

                    UpdateTrainState(trainId.Value, TrainState.Cancelled);

                    //ts.Complete();
                    return true;
                }
            //}
            return false;
        }

        public void UpdateTrainState(Guid trainId, TrainState state)
        {
            const string updateState = @"
                    UPDATE [natrail].[dbo].[LiveTrain]
                       SET [StateId] = @state
                     WHERE [Id] = @trainId";

            ExecuteNonQuery(updateState, new { state, trainId });
        }

        public void AddTrainDescriber(TrainDescriber td)
        {
            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
            //{
                Guid? trainId = null;
                if (RunningTrainExists(td.Description, out trainId))
                {
                    Trace.TraceInformation("Saving TD to: {0}", trainId.Value);

                    switch (td.Type)
                    {
                        case "CA":
                            CaTD caVal = td as CaTD;
                            if (caVal != null)
                            {
                                const string casql = @"
                                    INSERT INTO [natrail].[dbo].[LiveTrainBerth]
                                               ([TrainId]
                                               ,[MessageType]
                                               ,[Timestamp]
                                               ,[AreaId]
                                               ,[From]
                                               ,[To])
                                         VALUES
                                               (@trainId
                                               ,@type
                                               ,@ts
                                               ,@area
                                               ,@from
                                               ,@to)";
                                ExecuteNonQuery(casql, new
                                {
                                    trainId = trainId.Value,
                                    type = caVal.Type,
                                    ts = caVal.Time,
                                    area = caVal.AreaId,
                                    from = caVal.From,
                                    to = caVal.To
                                });
                            }
                            break;
                        case "CB":
                            CbTD cbVal = td as CbTD;
                            if (cbVal != null)
                            {
                                const string cbsql = @"
                                    INSERT INTO [natrail].[dbo].[LiveTrainBerth]
                                               ([TrainId]
                                               ,[MessageType]
                                               ,[Timestamp]
                                               ,[AreaId]
                                               ,[From])
                                         VALUES
                                               (@trainId
                                               ,@type
                                               ,@ts
                                               ,@area
                                               ,@from)";
                                ExecuteNonQuery(cbsql, new
                                {
                                    trainId = trainId.Value,
                                    type = cbVal.Type,
                                    ts = cbVal.Time,
                                    area = cbVal.AreaId,
                                    from = cbVal.From
                                });
                            }
                            break;
                        case "CC":
                            CcTD ccVal = td as CcTD;
                            if (ccVal != null)
                            {
                                const string ccsql = @"
                                    INSERT INTO [natrail].[dbo].[LiveTrainBerth]
                                               ([TrainId]
                                               ,[MessageType]
                                               ,[Timestamp]
                                               ,[AreaId]
                                               ,[To])
                                         VALUES
                                               (@trainId
                                               ,@type
                                               ,@ts
                                               ,@area
                                               ,@to)";
                                ExecuteNonQuery(ccsql, new
                                {
                                    trainId = trainId.Value,
                                    type = ccVal.Type,
                                    ts = ccVal.Time,
                                    area = ccVal.AreaId,
                                    to = ccVal.To
                                });
                            }
                            break;
                        //case "CT":
                        //default:
                    }
                }
                //ts.Complete();
            //}
        }
    }
}
