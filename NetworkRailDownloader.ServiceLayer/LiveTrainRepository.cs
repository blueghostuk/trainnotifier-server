﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using Dapper;

namespace TrainNotifier.Service
{
    public class LiveTrainRepository : DbRepository
    {
        private static readonly ObjectCache _trainActivationCache = MemoryCache.Default;
        private static readonly CacheItemPolicy _trainActivationCachePolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromHours(12)
        };

        /// <summary>
        /// Pre-loads trains activated or in progress set to depart from 12 hours ago into the future
        /// </summary>
        public void PreLoadActivations()
        {
            const string sql = @"SELECT [Id], [TrainId], [ScheduleTrain] FROM [LiveTrain] WHERE [Activated] = 1 AND [Terminated] = 0 AND [OriginDepartTimestamp] >= (GETDATE() - 0.5)";

            var activeTrains = Query<dynamic>(sql);

            Trace.TraceInformation("Pre loading {0} trains", activeTrains.Count());

            Trace.Flush();

            foreach (var activeTrain in activeTrains)
            {
                _trainActivationCache.Add(activeTrain.TrainId, new TrainMovementSchedule
                {
                    Id = activeTrain.Id,
                    Schedule = activeTrain.ScheduleTrain,
                    StopNumber = 0
                }, _trainActivationCachePolicy);
            }
        }

        public IEnumerable<TrainMovement> SearchByWttId(string wttId)
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
                WHERE SchedWttId LIKE @wttId";

            wttId += "%";

            return Query<TrainMovement>(sql, new { wttId });
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
            // distinct as can include arrival/departure
            const string sql = @"
                SELECT DISTINCT
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

        public IEnumerable<ExtendedTrainMovement> GetTrainMovementById(string trainId)
        {
            const string sql = @"
                SELECT TOP 1
                    [LiveTrain].[Id] AS [UniqueId]
                    ,[LiveTrain].[TrainId] AS [Id]
                    ,[LiveTrain].[Headcode] AS [HeadCode]
                    ,[LiveTrain].[CreationTimestamp] AS [Activated]
                    ,[LiveTrain].[OriginDepartTimestamp] AS [SchedOriginDeparture]
                    ,[LiveTrain].[TrainServiceCode] AS [ServiceCode]
                    ,[LiveTrain].[Toc] AS [TocId]
                    ,[LiveTrain].[TrainUid] AS [TrainUid]
                    ,[LiveTrain].[OriginStanox] AS [SchedOriginStanox]
                    ,[LiveTrain].[SchedWttId] AS [WorkingTTId]
                    ,[LiveTrainCancellation].[Stanox] AS [CancelledStanox]
                    ,[LiveTrainCancellation].[CancelledTimestamp]
                    ,[LiveTrainCancellation].[ReasonCode]
                    ,[DelayAttributionCodes].[Description]
                    ,[LiveTrainCancellation].[Type]
                    ,[CancelTiploc].[TiplocId]
                    ,[CancelTiploc].[Tiploc]
                    ,[CancelTiploc].[Nalco]
                    ,[CancelTiploc].[Description]
                    ,[CancelTiploc].[Stanox]
                    ,[CancelTiploc].[CRS]
                FROM [LiveTrain]
                LEFT JOIN [LiveTrainCancellation] ON [LiveTrain].[Id] = [LiveTrainCancellation].[TrainId]
                LEFT JOIN [DelayAttributionCodes] ON [LiveTrainCancellation].[ReasonCode] = [DelayAttributionCodes].[ReasonCode]
                LEFT JOIN [Tiploc] AS [CancelTiploc] ON [LiveTrainCancellation].[Stanox] = [CancelTiploc].[Stanox]
                WHERE [LiveTrain].[TrainId] = @trainId
                ORDER BY [LiveTrain].[OriginDepartTimestamp] DESC"; // get latest occurance

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                IEnumerable<ExtendedTrainMovement> trains = dbConnection.Query<ExtendedTrainMovement, ExtendedCancellation, TiplocCode, ExtendedTrainMovement>(
                    sql,
                    (t, c, s) =>
                    {
                        if (c != null)
                            c.CancelledAt = s;
                        t.Cancellation = c;
                        return t;
                    },
                    new
                    {
                        trainId
                    },
                    splitOn: "CancelledStanox,TiplocId");

                foreach (ExtendedTrainMovement tm in trains)
                {
                    const string tmsSql = @"
                        SELECT
                            [EventType]
                            ,[PlannedTimestamp] AS [PlannedTime]
                            ,[ActualTimestamp]
                            ,[ReportingStanox] AS [Stanox]
                            ,[Platform]
                            ,[Line] 
                            ,[TrainTerminated] AS [Terminated]
                            ,[ScheduleStopNumber]
                        FROM [LiveTrainStop]
                        WHERE [TrainId] = @trainId";

                    IEnumerable<TrainMovementStep> tmSteps = Query<TrainMovementStep>(tmsSql, new { trainId = tm.UniqueId }, dbConnection)
                        .ToList();
                    foreach (var tms in tmSteps)
                    {
                        if (tms.Terminated)
                            tms.State = State.Terminated;
                    }

                    tm.Steps = tmSteps;

                    yield return tm;
                }
            }
        }

        public ExtendedTrainMovement GetTrainMovementById(string trainId, string trainUid)
        {
            const string sql = @"
                SELECT TOP 1
                    [LiveTrain].[Id] AS [UniqueId]
                    ,[LiveTrain].[TrainId] AS [Id]
                    ,[LiveTrain].[Headcode] AS [HeadCode]
                    ,[LiveTrain].[CreationTimestamp] AS [Activated]
                    ,[LiveTrain].[OriginDepartTimestamp] AS [SchedOriginDeparture]
                    ,[LiveTrain].[TrainServiceCode] AS [ServiceCode]
                    ,[LiveTrain].[Toc] AS [TocId]
                    ,[LiveTrain].[TrainUid] AS [TrainUid]
                    ,[LiveTrain].[OriginStanox] AS [SchedOriginStanox]
                    ,[LiveTrain].[SchedWttId] AS [WorkingTTId]
                    ,[LiveTrainCancellation].[Stanox] AS [CancelledStanox]
                    ,[LiveTrainCancellation].[CancelledTimestamp]
                    ,[LiveTrainCancellation].[ReasonCode]
                    ,[DelayAttributionCodes].[Description]
                    ,[LiveTrainCancellation].[Type]
                    ,[CancelTiploc].[TiplocId]
                    ,[CancelTiploc].[Tiploc]
                    ,[CancelTiploc].[Nalco]
                    ,[CancelTiploc].[Description]
                    ,[CancelTiploc].[Stanox]
                    ,[CancelTiploc].[CRS]
                FROM [LiveTrain]
                LEFT JOIN [LiveTrainCancellation] ON [LiveTrain].[Id] = [LiveTrainCancellation].[TrainId]
                LEFT JOIN [DelayAttributionCodes] ON [LiveTrainCancellation].[ReasonCode] = [DelayAttributionCodes].[ReasonCode]
                LEFT JOIN [Tiploc] AS [CancelTiploc] ON [LiveTrainCancellation].[Stanox] = [CancelTiploc].[Stanox]
                WHERE [LiveTrain].[TrainId] = @trainId AND [LiveTrain].[TrainUid] = @trainUid
                ORDER BY [LiveTrain].[OriginDepartTimestamp] DESC"; // get latest occurance

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                ExtendedTrainMovement tm = dbConnection.Query<ExtendedTrainMovement, ExtendedCancellation, TiplocCode, ExtendedTrainMovement>(
                    sql,
                    (t, c, s) =>
                    {
                        if (c != null)
                            c.CancelledAt = s;
                        t.Cancellation = c;
                        return t;
                    },
                    new
                    {
                        trainId,
                        trainUid
                    },
                    splitOn: "CancelledStanox,TiplocId").FirstOrDefault();

                if (tm != null)
                {
                    const string tmsSql = @"
                        SELECT
                            [EventType]
                            ,[PlannedTimestamp] AS [PlannedTime]
                            ,[ActualTimestamp]
                            ,[ReportingStanox] AS [Stanox]
                            ,[Platform]
                            ,[Line] 
                            ,[TrainTerminated] AS [Terminated]
                            ,[ScheduleStopNumber]
                        FROM [LiveTrainStop]
                        WHERE [TrainId] = @trainId";

                    IEnumerable<TrainMovementStep> tmSteps = Query<TrainMovementStep>(tmsSql, new { trainId = tm.UniqueId }, dbConnection)
                        .ToList();
                    foreach (var tms in tmSteps)
                    {
                        if (tms.Terminated)
                            tms.State = State.Terminated;
                    }

                    tm.Steps = tmSteps;

                    return tm;
                }
            }

            return null;
        }

        public void AddActivation(TrainMovement tm, DbConnection existingConnection = null)
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
                    ,[Activated]
                    ,[Cancelled]
                    ,[Terminated])
                OUTPUT [inserted].[Id]
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
                    ,1
                    ,0
                    ,0)";

            Guid id = ExecuteInsert(insertActivation, new
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
            }, existingConnection);

            _trainActivationCache.Add(tm.Id, new TrainMovementSchedule
            {
                Id = id,
                Schedule = null,
                StopNumber = 0
            }, _trainActivationCachePolicy);

            tm.UniqueId = id;

            SetLiveTrainSchedule(tm);
        }

        private void SetLiveTrainSchedule(TrainMovement tm)
        {
            Task.Run(() =>
            {
                if (!tm.SchedOriginDeparture.HasValue)
                {
                    Trace.TraceInformation("Activation '{0}' has no departure date, cannot assign schedume", tm.TrainId);
                    return;
                }

                Trace.TraceInformation("Associating Schedule for activation {0} with departure date {1:dd/MM/yy}", tm.TrainId, tm.SchedOriginDeparture.Value);
                const string sql = @"
                    SELECT TOP 1 [ScheduleId]
                    FROM [ScheduleTrain]
                    WHERE 
                            [TrainUid] = @TrainUid
                        AND @date >= [StartDate]
                        AND @date <= [EndDate]
                        {0}
                        AND [Deleted] = 0
                    ORDER BY [STPIndicatorId]";

                var date = tm.SchedOriginDeparture.Value.Date;

                Guid? scheduleId = ExecuteScalar<Guid>(string.Format(sql, GetDatePartSql(date)), new
                {
                    tm.TrainUid,
                    date
                });

                if (scheduleId.HasValue && scheduleId != Guid.Empty)
                {
                    Trace.TraceInformation("Associating Schedule '{0}' for activation '{1}'", scheduleId, tm.TrainId);
                    const string updateSql = @"
                        UPDATE [LiveTrain]
                        SET [ScheduleTrain] = @scheduleId
                        WHERE [Id] = @UniqueId";

                    ExecuteNonQuery(updateSql, new
                    {
                        scheduleId,
                        tm.UniqueId
                    });

                    if (scheduleId.HasValue)
                    {
                        ((TrainMovementSchedule)_trainActivationCache[tm.Id]).Schedule = scheduleId.Value;
                    }
                }
                else
                {
                    Trace.TraceWarning("Could not find matching schedule for activation: {0}", tm.TrainId);
                }
            });
        }

        private static string GetDatePartSql(DateTime date)
        {
            const string sql = "AND [Runs{0}] = 1";
            return string.Format(sql, date.DayOfWeek.ToString());
        }

        private bool TrainExists(string trainId, out TrainMovementSchedule tm, DbConnection existingConnection = null)
        {
            tm = _trainActivationCache.Get(trainId) as TrainMovementSchedule;
            if (tm == null)
            {
                tm = ExecuteScalar<TrainMovementSchedule>("SELECT [Id], [ScheduleTrain] AS [Schedule] FROM [LiveTrain] WHERE [TrainId] = @trainId", new { trainId }, existingConnection);
                if (tm != null)
                {
                    _trainActivationCache.Add(trainId, tm, _trainActivationCachePolicy);
                }
            }
            return tm != null && tm.Id != Guid.Empty;
        }

        private bool RunningTrainExists(string id, out Guid? dbId, DbConnection existingConnection = null)
        {
            try
            {
                dbId = ExecuteScalar<Guid?>("SELECT [Id] FROM [LiveTrain] WHERE [Headcode] = @id AND [Activated] = 1 AND [Cancelled] = 0 AND [Terminated] = 0", null, existingConnection);
                return dbId.HasValue && dbId.Value != Guid.Empty;
            }
            catch (InvalidOperationException) { } // TODO: more than one found
            dbId = null;
            return false;
        }

        public bool AddMovement(TrainMovementStep tms, DbConnection existingConnection = null)
        {
            TrainMovementSchedule trainId = null;
            if (TrainExists(tms.TrainId, out trainId, existingConnection))
            {
                tms.DatabaseId = trainId.Id;
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
                               ,[TrainTerminated]
                               ,[ScheduleStopNumber])
                         VALUES
                               (@trainId
                               ,@eventType
                               ,@plannedTs
                               ,@actualTs
                               ,@stanox
                               ,@platform
                               ,@line
                               ,@terminated
                               ,@stopNumber)";

                byte? stopNumber = null;
                if (trainId.Schedule.HasValue
                    && trainId.Schedule != Guid.Empty
                    && !string.IsNullOrEmpty(tms.Stanox))
                {
                    stopNumber = GetNextStop(trainId.Schedule.Value, tms.Stanox, trainId.StopNumber);

                    if (stopNumber.HasValue)
                    {
                        ((TrainMovementSchedule)_trainActivationCache[tms.TrainId]).StopNumber = stopNumber.Value;
                    }
                }

                ExecuteNonQuery(insertStop, new
                {
                    trainId = trainId.Id,
                    eventType = tms.EventType,
                    plannedTs = tms.PlannedTime,
                    actualTs = tms.ActualTimeStamp,
                    stanox = tms.Stanox,
                    platform = tms.Platform,
                    line = tms.Line,
                    terminated = tms.State == State.Terminated,
                    stopNumber = stopNumber
                }, existingConnection);

                if (tms.State == State.Terminated)
                {
                    _trainActivationCache.Remove(tms.TrainId);
                }

                return true;
            }
            return false;
        }

        private byte? GetNextStop(Guid scheduleId, string stanox, byte latestStopNumber)
        {
            const string sql = @"
                SELECT TOP 1
                      [ScheduleTrainStop].[StopNumber]
                FROM [ScheduleTrainStop]
                INNER JOIN [Tiploc] ON [ScheduleTrainStop].[TiplocId] = [Tiploc].[TiplocId]
                WHERE [ScheduleId] = @scheduleId
                AND [Tiploc].[Stanox] = @stanox
                AND [ScheduleTrainStop].[StopNumber] >= @latestStopNumber";

            return ExecuteScalar<byte?>(sql, new
            {
                scheduleId,
                stanox,
                latestStopNumber
            });
        }

        public bool AddCancellation(CancelledTrainMovementStep cm, DbConnection existingConnection = null)
        {
            TrainMovementSchedule trainId = null;
            if (TrainExists(cm.TrainId, out trainId, existingConnection))
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
                    trainId = trainId.Id,
                    canxTs = cm.CancelledTime,
                    canxType = cm.CancelledType,
                    canxReason = cm.CancelledReasonCode,
                    stanox = cm.Stanox
                }, existingConnection);

                UpdateTrainState(trainId.Id, TrainState.Cancelled);

                return true;
            }
            return false;
        }

        public void UpdateTrainState(Guid trainId, TrainState state, DbConnection existingConnection = null)
        {
            switch (state)
            {
                case TrainState.Activated:
                    const string activateSql = @"
                    UPDATE [natrail].[dbo].[LiveTrain]
                       SET Activated = 1
                     WHERE [Id] = @trainId";

                    ExecuteNonQuery(activateSql, new { trainId }, existingConnection);
                    break;

                case TrainState.Cancelled:
                    const string cancelSql = @"
                    UPDATE [natrail].[dbo].[LiveTrain]
                       SET Cancelled = 1
                     WHERE [Id] = @trainId";

                    ExecuteNonQuery(cancelSql, new { trainId }, existingConnection);
                    break;

                case TrainState.Terminated:
                    const string termSql = @"
                    UPDATE [natrail].[dbo].[LiveTrain]
                       SET Terminated = 1
                     WHERE [Id] = @trainId";

                    ExecuteNonQuery(termSql, new { trainId }, existingConnection);
                    break;

                // in progress - dont need to do anything
            }
        }

        public void AddTrainDescriber(TrainDescriber td, DbConnection existingConnection = null)
        {
            Guid? trainId = null;
            if (RunningTrainExists(td.Description, out trainId, existingConnection))
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
                            }, existingConnection);
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
                            }, existingConnection);
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
                            }, existingConnection);
                        }
                        break;
                    //case "CT":
                    //default:
                }
            }
        }

        public void BatchInsertTrainData(IEnumerable<ITrainData> trainData)
        {
            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                ICollection<TrainMovementStep> trainMovements = new List<TrainMovementStep>();

                foreach (var train in trainData)
                {
                    TrainMovement tm = train as TrainMovement;
                    if (tm != null)
                    {
                        AddActivation(tm, dbConnection);
                    }
                    else
                    {
                        CancelledTrainMovementStep ctms = train as CancelledTrainMovementStep;
                        if (ctms != null)
                        {
                            AddCancellation(ctms, dbConnection);
                        }
                        else
                        {
                            TrainMovementStep tms = train as TrainMovementStep;
                            if (tms != null)
                            {
                                AddMovement(tms, dbConnection);
                                trainMovements.Add(tms);
                            }
                        }
                    }
                }

                // only update once per train
                foreach (var groupByTrain in trainMovements.Where(tm => tm.DatabaseId.HasValue).GroupBy(tm => tm.DatabaseId.Value))
                {
                    UpdateTrainState(groupByTrain.Key, groupByTrain.Any(tm => tm.State == State.Terminated) ?
                        TrainState.Terminated : TrainState.InProgress, dbConnection);
                }
            }
        }

        public void BatchInsertTDData(IEnumerable<TrainDescriber> trainData)
        {
            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                foreach (var train in trainData)
                {
                    AddTrainDescriber(train, dbConnection);
                }
            }
        }
    }
}
