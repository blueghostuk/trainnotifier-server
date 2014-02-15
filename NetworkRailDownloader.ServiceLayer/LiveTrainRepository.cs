using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.Model.SmartExtract;

namespace TrainNotifier.Service
{
    public class LiveTrainRepository : DbRepository, IDisposable
    {
        private static readonly ObjectCache _trainActivationCache = MemoryCache.Default;
        private static readonly CacheItemPolicy _trainActivationCachePolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromHours(12)
        };

        private static readonly TiplocRepository _tiplocRepository = new TiplocRepository();
        private static readonly ScheduleRepository _scheduleRepository = new ScheduleRepository();
        private static readonly TrainMovementRepository _tmRepo = new TrainMovementRepository();

        /// <summary>
        /// Pre-loads trains activated or in progress set to depart from 12 hours ago into the future
        /// </summary>
        public void PreLoadActivations()
        {
            const string sql = @"
                SELECT 
                    [Id]
                    ,[TrainId]
                    ,[ScheduleTrain]
                FROM [LiveTrain] 
                WHERE [TrainStateId] = @activated
                    AND [OriginDepartTimestamp] >= (GETDATE() - 0.5)";

            var activeTrains = Query<dynamic>(sql, new { TrainState.Activated });

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

        public void AddActivation(TrainMovement tm, DbConnection existingConnection = null)
        {
            var tiplocs = _tiplocRepository.GetAllByStanox(tm.SchedOriginStanox);
            if (tiplocs.Any())
            {
                Trace.TraceInformation("Saving Activation: {0}", tm.TrainId);
                const string insertActivation = @"
                INSERT INTO [natrail].[dbo].[LiveTrain]
                    ([TrainId]
                    ,[Headcode]
                    ,[CreationTimestamp]
                    ,[OriginDepartTimestamp]
                    ,[TrainServiceCode]
                    ,[OriginTiplocId]
                    ,[TrainStateId])
                OUTPUT [inserted].[Id]
                VALUES
                    (@trainId
                    ,@headcode
                    ,@activationDate
                    ,@originTime
                    ,@serviceCode
                    ,@originTiplocId
                    ,@trainStateId)";

                Guid id = ExecuteInsert(insertActivation, new
                {
                    trainId = tm.Id,
                    headcode = tm.WorkingTTId.Substring(0, 4),
                    activationDate = tm.Activated,
                    originTime = tm.SchedOriginDeparture,
                    serviceCode = tm.ServiceCode,
                    originTiplocId = tiplocs.First().TiplocId,
                    trainStateId = TrainState.Activated
                }, existingConnection);

                _trainActivationCache.Add(tm.Id, new TrainMovementSchedule
                {
                    Id = id,
                    Schedule = null,
                    StopNumber = 0
                }, _trainActivationCachePolicy);

                tm.UniqueId = id;

                SetLiveTrainSchedule(tm, tiplocs);
            }
        }

        private Task SetLiveTrainSchedule(TrainMovement tm, IEnumerable<TiplocCode> tiplocs)
        {
            return Task.Run(() =>
            {
                if (!tm.SchedOriginDeparture.HasValue)
                {
                    Trace.TraceInformation("Activation '{0}' has no departure date, cannot assign schedume", tm.TrainId);
                    return;
                }

                Trace.TraceInformation("Associating Schedule for activation {0},{1} with departure date {2:dd/MM/yy}", tm.TrainId, tm.TrainUid, tm.SchedOriginDeparture.Value);
                // until deleted schedules are understood better include them but order them out
//                const string sql = @"
//                    SELECT TOP 1 [ScheduleId]
//                    FROM [ScheduleTrain]
//                    WHERE 
//                            [TrainUid] = @trainUid
//                        AND @date >= [StartDate]
//                        AND @date <= [EndDate]
//                        AND [Deleted] = 0
//                        {0}
//                    ORDER BY [STPIndicatorId]";

                // order by deleted first to get non-deleted, then by stp indicator as normal, then by powertype to get the
                // one (If any) that has a power type
                const string sql = @"
                    SELECT TOP 1 [ScheduleId]
                    FROM [ScheduleTrain]
                    WHERE 
                            [TrainUid] = @trainUid
                        AND @date >= [StartDate]
                        AND @date <= [EndDate]
                        {0}
                    ORDER BY [Deleted], [STPIndicatorId], [PowerTypeId] DESC";

                var date = tm.SchedOriginDeparture.Value.Date;

                Guid? scheduleId = ExecuteScalar<Guid>(string.Format(sql, GetDatePartSql(date)), new
                {
                    // need to trim as schedule inserted trimmed at present
                    trainUid = tm.TrainUid.Trim(),
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

                    ((TrainMovementSchedule)_trainActivationCache[tm.Id]).Schedule = scheduleId.Value;

                    IEnumerable<ScheduleStop> stops = _scheduleRepository.GetStopsById(scheduleId.Value);
                    AddFirstArrivalMovement(tm.UniqueId, tm.SchedOriginDeparture.GetValueOrDefault(DateTime.UtcNow), stops.ElementAt(0));

                    // if there was more than 1 tiploc see if we can find the correct one
                    if (tiplocs.Count() > 1)
                    {
                        foreach (var stop in stops)
                        {
                            TiplocCode matchedTiploc = tiplocs.FirstOrDefault(t => t.TiplocId == stop.Tiploc.TiplocId);
                            if (matchedTiploc != null)
                            {
                                if (matchedTiploc.TiplocId != tiplocs.First().TiplocId)
                                {
                                    UpdateLiveTrainOrigin(tm.UniqueId, matchedTiploc.TiplocId);
                                }

                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(tm.TocId))
                    {
                        // if VSTP update toc
                        const string vstpTocSql = @"
                        UPDATE [ScheduleTrain]
                        SET [AtocCode] = @atocCode
                        WHERE [ScheduleId] = @scheduleId
                        AND [Source] = 1";

                        ExecuteNonQuery(vstpTocSql, new
                        {
                            scheduleId,
                            atocCode = tm.TocId
                        });
                    }
                }
                else
                {
                    Trace.TraceWarning("Could not find matching schedule for activation: {0}, {1}", tm.TrainId, tm.TrainUid);
                }
            });
        }

        private void AddFirstArrivalMovement(Guid trainId, DateTime departureTime, ScheduleStop scheduleStop)
        {
            try
            {
                const string insertStop = @"
                INSERT INTO [dbo].[LiveTrainStop]
                            ([TrainId]
                            ,[EventTypeId]
                            ,[PlannedTimestamp]
                            ,[ActualTimestamp]
                            ,[ReportingTiplocId]
                            ,[Platform]
                            ,[Line]
                            ,[ScheduleStopNumber])
                        VALUES
                            (@trainId
                            ,@eventTypeId
                            ,@plannedTs
                            ,@actualTs
                            ,@reportingTiplocId
                            ,@platform
                            ,@line
                            ,@stopNumber)";

                ExecuteNonQuery(insertStop, new
                {
                    trainId,
                    eventTypeId = TrainMovementEventType.Arrival,
                    plannedTs = departureTime,
                    actualTs = departureTime,
                    reportingTiplocId = scheduleStop.Tiploc.TiplocId,
                    platform = scheduleStop.Platform,
                    line = scheduleStop.Line,
                    stopNumber = 0
                });
            }
            catch { }
        }

        private void UpdateLiveTrainOrigin(Guid trainId, short tiplocId)
        {
            const string sql = @"
                UPDATE [LiveTrain]
                SET [OriginTiplocId] = @tiplocId
                WHERE [Id] = @trainId";

            ExecuteNonQuery(sql, new { trainId, tiplocId });
        }

        private static string GetDatePartSql(DateTime date)
        {
            const string sql = "AND [Runs{0}] = 1";
            return string.Format(sql, date.DayOfWeek.ToString());
        }

        private bool TrainExists(string trainId, out TrainMovementSchedule tm, DbConnection existingConnection = null)
        {
            tm = _trainActivationCache.Get(trainId) as TrainMovementSchedule;
            //if (tm == null)
            //{
            //    TrainIdTranslator details = TrainIdTranslator.ParseDataString(trainId);
            //    var trains = _tmRepo.GetTrainMovementByHeadcode(details.Headcode, DateTime.UtcNow.);
                
            //    if (tm != null)
            //    {
            //        _trainActivationCache.Add(trainId, tm, _trainActivationCachePolicy);
            //    }
            //}
            return tm != null && tm.Id != Guid.Empty;
        }

        /// <summary>
        /// Update a train movement
        /// </summary>
        /// <param name="trainId">id of live train</param>
        /// <param name="tiplocIds">reporting tiploc(s)</param>
        /// <param name="eventType">arrival or departure to update</param>
        /// <param name="actualTime">actual time to set</param>
        /// <param name="source">source of data</param>
        /// <returns>true if a row updated</returns>
        public bool UpdateMovement(Guid trainId, TDElement td, IEnumerable<short> tiplocIds, TrainMovementEventType eventType, DateTime actualTime, LiveTrainStopSource source = LiveTrainStopSource.TD)
        {
            const string sql = @"
                UPDATE [dbo].[LiveTrainStop]
                SET  [ActualTimestamp] = @actualTime
                    ,[Platform] = @platform
                    ,[LiveTrainStopSourceId] = @source
                WHERE   [TrainId] = @trainId
                    AND [ReportingTiplocId] IN @tiplocIds
                    AND [EventTypeId] = @eventType
                    AND [LiveTrainStopSourceId] != @source
                    AND [ActualTimeStamp] >= @timeBefore 
                    AND [ActualTimeStamp] <= @timeAfter";

            return ExecuteNonQuery(sql, new
            {
                trainId,
                platform = td.PLATFORM,
                tiplocIds,
                eventType,
                actualTime,
                source,
                timeBefore = actualTime.AddMinutes(-145),
                timeAfter = actualTime.AddMinutes(30)
            }) > 0;
        }

        public bool AddMovement(TrainMovementStep tms, DbConnection existingConnection = null)
        {
            TrainMovementSchedule trainId = null;
            if (TrainExists(tms.TrainId, out trainId, existingConnection) && trainId.Schedule.HasValue && trainId.Schedule != Guid.Empty)
            {
                tms.DatabaseId = trainId.Id;
                Trace.TraceInformation("Saving Movement to: {0}", tms.TrainId);
                const string insertStop = @"
                    INSERT INTO [dbo].[LiveTrainStop]
                               ([TrainId]
                               ,[EventTypeId]
                               ,[PlannedTimestamp]
                               ,[ActualTimestamp]
                               ,[ReportingTiplocId]
                               ,[Platform]
                               ,[Line]
                               ,[ScheduleStopNumber])
                         VALUES
                               (@trainId
                               ,@eventTypeId
                               ,@plannedTs
                               ,@actualTs
                               ,@reportingTiplocId
                               ,@platform
                               ,@line
                               ,@stopNumber)";

                byte? stopNumber = null;
                short? tiplocId = null;

                if (!string.IsNullOrEmpty(tms.Stanox))
                {
                    dynamic values = GetNextStop(trainId.Schedule.Value, TrainMovementEventTypeField.ParseDataString(tms.EventType), tms.Stanox, trainId.StopNumber);

                    try
                    {
                        stopNumber = values.StopNumber;
                        tiplocId = values.TiplocId;

                        if (stopNumber.HasValue)
                        {
                            ((TrainMovementSchedule)_trainActivationCache[tms.TrainId]).StopNumber = stopNumber.Value;
                        }
                    }
                    catch { }
                }
                if (!tiplocId.HasValue)
                {
                    var tiplocs = _tiplocRepository.GetAllByStanox(tms.Stanox);
                    if (tiplocs.Any())
                    {
                        if (tiplocs.Count() > 1)
                        {
                            var stops = _scheduleRepository.GetStopsById(trainId.Schedule.Value);
                            foreach (var stop in stops)
                            {
                                var matchedTiploc = tiplocs.FirstOrDefault(t => t.TiplocId == stop.Tiploc.TiplocId);
                                if (matchedTiploc != null)
                                {
                                    if (matchedTiploc.TiplocId != tiplocs.First().TiplocId)
                                    {
                                        tiplocId = matchedTiploc.TiplocId;
                                    }

                                    break;
                                }
                            }
                        }
                        if (!tiplocId.HasValue)
                        {
                            tiplocId = tiplocs.First().TiplocId;
                        }
                    }
                }
                if (tiplocId.HasValue)
                {
                    ExecuteNonQuery(insertStop, new
                    {
                        trainId = trainId.Id,
                        eventTypeId = TrainMovementEventTypeField.ParseDataString(tms.EventType),
                        plannedTs = tms.PlannedTime,
                        actualTs = tms.ActualTimeStamp,
                        reportingTiplocId = tiplocId,
                        platform = (string.IsNullOrEmpty(tms.Platform) ? default(string) : tms.Platform),
                        line = (string.IsNullOrEmpty(tms.Line) ? default(string) : tms.Line),
                        stopNumber = stopNumber
                    }, existingConnection);
                    
                    return true;
                }
            }
            return false;
        }

        private dynamic GetNextStop(Guid scheduleId,TrainMovementEventType eventType, string stanox, byte latestStopNumber)
        {
            const string sql = @"
                SELECT TOP 1
                      [ScheduleTrainStop].[StopNumber],
                      [Tiploc].[TiplocId]
                FROM [ScheduleTrainStop]
                INNER JOIN [Tiploc] ON [ScheduleTrainStop].[TiplocId] = [Tiploc].[TiplocId]
                WHERE [ScheduleId] = @scheduleId
                AND [Tiploc].[Stanox] = @stanox
                AND [ScheduleTrainStop].[StopNumber] >= @latestStopNumber";

            const string extraWhere = " AND [ScheduleTrainStop].[StopNumber] > 0";

            return ExecuteScalar<dynamic>(string.Concat(sql, eventType == TrainMovementEventType.Arrival ? extraWhere : null), new
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
                    INSERT INTO [dbo].[LiveTrainCancellation]
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

        private bool AddChangeOfOrigin(TrainChangeOfOrigin tOrigin, DbConnection existingConnection = null)
        {
            TrainMovementSchedule trainId = null;
            if (TrainExists(tOrigin.TrainId, out trainId, existingConnection))
            {
                TiplocCode tiploc = _tiplocRepository.GetByStanox(tOrigin.Stanox);
                if (tiploc != null)
                {
                    Trace.TraceInformation("Saving Change of Origin to: {0} @ {1}", tOrigin.TrainId, tOrigin.Stanox);

                    const string insertStop = @"
                        INSERT INTO [LiveTrainChangeOfOrigin]
                           ([TrainId]
                           ,[ReasonCode]
                           ,[NewTiplocId]
                           ,[NewDepartureTime]
                           ,[ChangedTime])
                        VALUES
                           (@trainId
                           ,@reasonCode
                           ,@newTiplocId
                           ,@newDepartureTime
                           ,@changedTime)";

                    ExecuteNonQuery(insertStop, new
                    {
                        trainId = trainId.Id,
                        reasonCode = tOrigin.ReasonCode,
                        newTiplocId = tiploc.TiplocId,
                        newDepartureTime = tOrigin.NewDepartureTime,
                        changedTime = tOrigin.ChangedTime
                    }, existingConnection);
                }
            }
            return false;
        }

        private bool AddReinstatement(TrainReinstatement tr, DbConnection existingConnection = null)
        {
            TrainMovementSchedule trainId = null;
            if (TrainExists(tr.TrainId, out trainId, existingConnection))
            {
                TiplocCode tiploc = _tiplocRepository.GetByStanox(tr.Stanox);
                if (tiploc != null)
                {
                    Trace.TraceInformation("Saving Reinstatement of: {0} @ {1}", tr.TrainId, tr.Stanox);

                    const string insertStop = @"
                        INSERT INTO [LiveTrainReinstatement]
                           ([TrainId]
                           ,[PlannedDepartureTime]
                           ,[ReinstatedTiplocId]
                           ,[ReinstatementTime])
                        VALUES
                           (@trainId
                           ,@plannedDepartureTime
                           ,@reinstatedTiplocId
                           ,@reinstatementTime)";

                    ExecuteNonQuery(insertStop, new
                    {
                        trainId = trainId.Id,
                        plannedDepartureTime = tr.NewDepartureTime,
                        reinstatedTiplocId = tiploc.TiplocId,
                        reinstatementTime = tr.ReinstatementTime
                    }, existingConnection);
                }
            }
            return false;
        }

        public void UpdateTrainState(Guid trainId, TrainState state, DbConnection existingConnection = null)
        {
            switch (state)
            {
                case TrainState.Cancelled:
                case TrainState.Terminated:
                    const string cancelSql = @"
                    UPDATE [dbo].[LiveTrain]
                       SET [TrainStateId] = [TrainStateId] + @state
                     WHERE [Id] = @trainId";

                    ExecuteNonQuery(cancelSql, new { trainId, state }, existingConnection);
                    break;
            }
        }

        public string GetHeadCode(string trainId)
        {
            const string sql = @"
                SELECT TOP 1
                    [Headcode]
                FROM [natrail].[dbo].[LiveTrain]
                WHERE [TrainId] = @trainId
                ORDER BY [OriginDepartTimestamp] DESC";

            return ExecuteScalar<string>(sql, new { trainId });
        }

        private ConcurrentQueue<ITrainData> _failedDataCache = new ConcurrentQueue<ITrainData>();

        private Timer _timer;

        public void StartTimer()
        {
            _timer = new Timer(
                this.ProcessQueue,
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));
        }

        private void ProcessQueue(object ignored)
        {
            Trace.TraceInformation("Processing failed items queue. Item Count - {0}", _failedDataCache.Count);
            while (!_failedDataCache.IsEmpty)
            {
                ITrainData data = null;
                if (_failedDataCache.TryPeek(out data))
                {
                    try
                    {
                        BatchInsertTrainData(new[] { data }, false);
                        _failedDataCache.TryDequeue(out data);
                    }
                    catch (SqlException e)
                    {
                        // if not timeout then remove from queue
                        if (e.Number != -2)
                        {
                            _failedDataCache.TryDequeue(out data);
                        }
                    }
                }
            }
        }

        public void BatchInsertTrainData(IEnumerable<ITrainData> trainData, bool onFailAddToQueue = true)
        {
            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                ICollection<TrainMovementStep> trainMovements = new List<TrainMovementStep>();

                foreach (var train in trainData)
                {
                    try
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
                                else
                                {
                                    TrainChangeOfOrigin tOrigin = train as TrainChangeOfOrigin;
                                    if (tOrigin != null)
                                    {
                                        AddChangeOfOrigin(tOrigin);
                                    }
                                    else
                                    {
                                        TrainReinstatement tr = train as TrainReinstatement;
                                        if (tr != null)
                                        {
                                            AddReinstatement(tr);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        // check for timeout only
                        if (e.Number == -2)
                        {
                            Trace.TraceError(e.ToString());
                            if (onFailAddToQueue)
                            {
                                _failedDataCache.Enqueue(train);
                            }
                            else
                            {
                                throw;
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                // only update once per train
                foreach (var groupByTrain in trainMovements
                    .Where(tm => tm.DatabaseId.HasValue)
                    .GroupBy(tm => tm.DatabaseId.Value))
                {
                    if (groupByTrain.Any(tm => tm.State == State.Terminated))
                    {
                        UpdateTrainState(groupByTrain.Key, TrainState.Terminated, dbConnection);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
