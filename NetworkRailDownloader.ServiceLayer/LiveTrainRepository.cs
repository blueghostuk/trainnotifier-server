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

        private static readonly AtocCodeRepository _atocCodeRepository = new AtocCodeRepository();
        private static readonly TiplocRepository _tiplocRepository = new TiplocRepository();
        private static readonly ScheduleRepository _scheduleRepository = new ScheduleRepository();

        public class Activation
        {
            public Guid DbId { get; private set; }
            public string TrainId { get; private set; }

            public Guid? ScheduleDbId { get; set; }

            public byte CurrentStopNumber { get; set; }

            public IEnumerable<ScheduleStop> Stops { get; set; }

            public Activation(Guid dbId, string trainId)
            {
                DbId = dbId;
                TrainId = trainId;

                Stops = Enumerable.Empty<ScheduleStop>();
                CurrentStopNumber = 0;
            }
        }

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
                    AND [OriginDepartTimestamp] >= (GETDATE() - 0.5)
                    AND [ScheduleTrain] IS NOT NULL";

            var activeTrains = Query<dynamic>(sql, new { TrainState.Activated });

            Trace.TraceInformation("Pre loading {0} trains", activeTrains.Count());

            Trace.Flush();

            foreach (var activeTrain in activeTrains)
            {
                _trainActivationCache.Add(activeTrain.TrainId, new Activation(activeTrain.Id, activeTrain.TrainId)
                {
                    ScheduleDbId = activeTrain.ScheduleTrain,
                }, _trainActivationCachePolicy);
            }

            Trace.TraceInformation("Pre loading tiploc cache");

            Trace.Flush();

            _tiplocRepository.PreloadCache();
        }

        public void AddActivation(TrainActivation activation)
        {
            var tiplocs = _tiplocRepository.GetTiplocsByStanox(activation.ScheduleOriginStanox);
            if (tiplocs.Any())
            {
                Trace.TraceInformation("Saving Activation: {0}", activation.TrainId);
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
                    trainId = activation.TrainId,
                    headcode = activation.WorkingTTId.Substring(0, 4),
                    activationDate = activation.Activated,
                    originTime = activation.ScheduleOriginDeparture,
                    serviceCode = activation.ServiceCode,
                    originTiplocId = tiplocs.First().TiplocId,
                    trainStateId = TrainState.Activated
                });

                _trainActivationCache.Add(activation.TrainId, new Activation(id, activation.TrainId)
                {
                    ScheduleDbId = null,
                    CurrentStopNumber = 0
                }, _trainActivationCachePolicy);

                activation.UniqueId = id;

                SetLiveTrainSchedule(activation, tiplocs);
            }
        }

        public class MatchSchedule
        {
            public Guid ScheduleId { get; set; }

            public byte? CategoryTypeId { get; set; }

            public string AtocCode { get; set; }

            public short? DestinationStopTiplocId { get; set; }
        }

        public MatchSchedule GetMatchingSchedule(TrainActivation activation)
        {
            const string sql = @"
                    SELECT TOP 1 
                        [ScheduleId]
                        ,[CategoryTypeId]
                        ,[AtocCode]
                        ,[DestinationStopTiplocId]
                    FROM [ScheduleTrain]
                    WHERE 
                            [TrainUid] = @trainUid
                        AND [StartDate] = @startDate
                        AND [EndDate] = @endDate
                    ORDER BY [Deleted], [STPIndicatorId], [PowerTypeId] DESC";


            var result = ExecuteScalar<MatchSchedule>(sql, new
            {
                // need to trim as schedule inserted trimmed at present
                trainUid = activation.TrainUid.Trim(),
                startDate = activation.ScheduleStartDate.Date,
                endDate = activation.ScheduleEndDate.Date
            });

            if (result != null && string.IsNullOrWhiteSpace(result.AtocCode) && !string.IsNullOrWhiteSpace(activation.TocId))
            {
                try
                {
                    var code = _atocCodeRepository.GetByNumericCode(Convert.ToByte(activation.TocId));
                    if (code != null)
                    {
                        result.AtocCode = code.Code;
                    }
                }
                catch { }
            }

            return result;
        }

        private void SetLiveTrainSchedule(TrainActivation activation, IEnumerable<TiplocCode> tiplocs)
        {
            Trace.TraceInformation("Finding Schedule for activation {0},{1} with departure date {2:dd/MM/yy}",
                activation.TrainId, activation.TrainUid, activation.ScheduleOriginDeparture);

            MatchSchedule scheduleData = GetMatchingSchedule(activation);
            if (scheduleData != null)
            {
                Trace.TraceInformation("Associating Schedule '{0}' for activation '{1}'",
                    scheduleData.ScheduleId,
                    activation.TrainId);

                const string updateSql = @"
                        UPDATE [LiveTrain]
                        SET 
                            [ScheduleTrain] = @scheduleId,
                            [ScheduleTrainUid] = @scheduleUid,
                            [ScheduleTrainAtocCode] = @scheduleAtocId,
                            [ScheduleTrainCategoryTypeId] = @categoryTypeId,    
                            [ScheduleTrainDestinationTiplocId] = @destinationStopTiplocId    
                        WHERE [Id] = @UniqueId";

                ExecuteNonQuery(updateSql, new
                {
                    scheduleId = scheduleData.ScheduleId,
                    scheduleUid = activation.TrainUid,
                    scheduleAtocId = scheduleData.AtocCode,
                    categoryTypeId = scheduleData.CategoryTypeId,
                    destinationStopTiplocId = scheduleData.DestinationStopTiplocId,
                    UniqueId = activation.UniqueId
                });

                ((Activation)_trainActivationCache[activation.TrainId]).ScheduleDbId = scheduleData.ScheduleId;
                if (!((Activation)_trainActivationCache[activation.TrainId]).Stops.Any())
                {
                    ((Activation)_trainActivationCache[activation.TrainId]).Stops = _scheduleRepository.GetStopsById(scheduleData.ScheduleId);
                }
                if (((Activation)_trainActivationCache[activation.TrainId]).Stops.Any())
                {
                    AddFirstArrivalMovement(activation.UniqueId, activation.ScheduleOriginDeparture, 
                        ((Activation)_trainActivationCache[activation.TrainId]).Stops.ElementAt(0));
                }

                // if there was more than 1 tiploc see if we can find the correct one
                if (tiplocs.Count() > 1)
                {
                    foreach (var stop in ((Activation)_trainActivationCache[activation.TrainId]).Stops)
                    {
                        TiplocCode matchedTiploc = tiplocs.FirstOrDefault(t => t.TiplocId == stop.Tiploc.TiplocId);
                        if (matchedTiploc != null)
                        {
                            if (matchedTiploc.TiplocId != tiplocs.First().TiplocId)
                            {
                                UpdateLiveTrainOrigin(activation.UniqueId, matchedTiploc.TiplocId);
                            }

                            break;
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(scheduleData.AtocCode))
                {
                    try
                    {
                        // if VSTP update toc
                        const string vstpTocSql = @"
                                UPDATE [ScheduleTrain]
                                SET [AtocCode] = @atocCode
                                WHERE [ScheduleId] = @scheduleId
                                AND [Source] = 1";

                        ExecuteNonQuery(vstpTocSql, new
                        {
                            scheduleData.ScheduleId,
                            atocCode = scheduleData.AtocCode
                        });
                    }
                    catch { }
                }
            }
            else
            {
                Trace.TraceWarning("Could not find matching schedule for activation: {0}, {1}", activation.TrainId, activation.TrainUid);
            }
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
                            ,[ScheduleStopNumber]
                            ,[Public])
                        VALUES
                            (@trainId
                            ,@eventTypeId
                            ,@plannedTs
                            ,@actualTs
                            ,@reportingTiplocId
                            ,@platform
                            ,@line
                            ,@stopNumber,
                            ,@public)";

                ExecuteNonQuery(insertStop, new
                {
                    trainId,
                    eventTypeId = TrainMovementEventType.Arrival,
                    plannedTs = departureTime,
                    actualTs = departureTime,
                    reportingTiplocId = scheduleStop.Tiploc.TiplocId,
                    platform = scheduleStop.Platform,
                    line = scheduleStop.Line,
                    stopNumber = 0,
                    @public = scheduleStop.PublicArrival.HasValue || scheduleStop.PublicDeparture.HasValue
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

        private bool TrainExists(string trainId, out Activation tm)
        {
            tm = _trainActivationCache.Get(trainId) as Activation;
            return tm != null;
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

        public bool AddMovement(TrainMovementStep tms)
        {
            Activation activation = null;
            if (TrainExists(tms.TrainId, out activation))
            {
                tms.DatabaseId = activation.DbId;
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
                               ,[ScheduleStopNumber]
                               ,[Public])
                         VALUES
                               (@trainId
                               ,@eventTypeId
                               ,@plannedTs
                               ,@actualTs
                               ,@reportingTiplocId
                               ,@platform
                               ,@line
                               ,@stopNumber
                               ,@public)";

                ScheduleStop nextStop = null;
                short? tiplocId = null;

                if (!string.IsNullOrEmpty(tms.Stanox) && activation.ScheduleDbId.HasValue)
                {
                    nextStop = GetNextStop(tms.TrainId, TrainMovementEventTypeField.ParseDataString(tms.EventType), tms.Stanox, activation.CurrentStopNumber);

                    if (nextStop != null)
                    {
                        tiplocId = nextStop.Tiploc.TiplocId;
                        ((Activation)_trainActivationCache[tms.TrainId]).CurrentStopNumber = nextStop.StopNumber;
                    }
                }
                if (nextStop == null && activation.ScheduleDbId.HasValue)
                {
                    var tiplocs = _tiplocRepository.GetTiplocsByStanox(tms.Stanox);
                    if (tiplocs.Any())
                    {
                        if (tiplocs.Count() > 1)
                        {
                            foreach (var stop in ((Activation)_trainActivationCache[tms.TrainId]).Stops)
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
                        trainId = activation.DbId,
                        eventTypeId = TrainMovementEventTypeField.ParseDataString(tms.EventType),
                        plannedTs = tms.PlannedTime,
                        actualTs = tms.ActualTimeStamp,
                        reportingTiplocId = tiplocId,
                        platform = (string.IsNullOrEmpty(tms.Platform) ? default(string) : tms.Platform),
                        line = (string.IsNullOrEmpty(tms.Line) ? default(string) : tms.Line),
                        stopNumber = nextStop != null ? nextStop.StopNumber : default(byte?),
                        @public = nextStop != null && (nextStop.PublicArrival.HasValue || nextStop.PublicDeparture.HasValue)
                    });

                    return true;
                }
            }
            return false;
        }

        private class GetNextStopResult
        {
            public byte StopNumber { get; set; }
            public short TiplocId { get; set; }
            public TimeSpan? PublicDeparture { get; set; }
            public TimeSpan? PublicArrival { get; set; }
        }

        private ScheduleStop GetNextStop(string trainId, TrainMovementEventType eventType, string stanox, byte latestStopNumber)
        {
            // get tiplocs first to optimise next query
            var tiplocs = _tiplocRepository.GetTiplocsByStanox(stanox)
                .Select(t => t.TiplocId)
                .ToList();

            var stops = ((Activation)_trainActivationCache[trainId]).Stops
                .Where(s => tiplocs.Contains(s.Tiploc.TiplocId))
                .Where(s => s.StopNumber >= latestStopNumber);

            if (eventType == TrainMovementEventType.Arrival)
            {
                stops = stops.Where(s => s.StopNumber > 0);
            }

            stops = stops.OrderBy(s => s.StopNumber);

            return stops.FirstOrDefault();
        }

        public bool AddCancellation(CancelledTrainMovementStep cm)
        {
            Activation activation = null;
            if (TrainExists(cm.TrainId, out activation))
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
                    trainId = activation.DbId,
                    canxTs = cm.CancelledTime,
                    canxType = cm.CancelledType,
                    canxReason = cm.CancelledReasonCode,
                    stanox = cm.Stanox
                });

                UpdateTrainState(activation.DbId, TrainState.Cancelled);

                return true;
            }
            return false;
        }

        private bool AddChangeOfOrigin(TrainChangeOfOrigin tOrigin)
        {
            Activation activation = null;
            if (TrainExists(tOrigin.TrainId, out activation))
            {
                TiplocCode tiploc = _tiplocRepository.GetTiplocByStanox(tOrigin.Stanox);
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
                        trainId = activation.DbId,
                        reasonCode = tOrigin.ReasonCode,
                        newTiplocId = tiploc.TiplocId,
                        newDepartureTime = tOrigin.NewDepartureTime,
                        changedTime = tOrigin.ChangedTime
                    });
                }
            }
            return false;
        }

        private bool AddReinstatement(TrainReinstatement tr)
        {
            Activation trainId = null;
            if (TrainExists(tr.TrainId, out trainId))
            {
                TiplocCode tiploc = _tiplocRepository.GetTiplocByStanox(tr.Stanox);
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
                        trainId = trainId.DbId,
                        plannedDepartureTime = tr.NewDepartureTime,
                        reinstatedTiplocId = tiploc.TiplocId,
                        reinstatementTime = tr.ReinstatementTime
                    });
                }
            }
            return false;
        }

        public void UpdateTrainState(Guid trainId, TrainState state)
        {
            switch (state)
            {
                case TrainState.Cancelled:
                case TrainState.Terminated:
                    const string cancelSql = @"
                    UPDATE [dbo].[LiveTrain]
                       SET [TrainStateId] = [TrainStateId] + @state
                     WHERE [Id] = @trainId";

                    ExecuteNonQuery(cancelSql, new { trainId, state });
                    break;
            }
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
            ICollection<TrainMovementStep> trainMovements = new List<TrainMovementStep>();

            foreach (var train in trainData)
            {
                try
                {
                    TrainActivation activation = train as TrainActivation;
                    if (activation != null)
                    {
                        AddActivation(activation);
                    }
                    else
                    {
                        CancelledTrainMovementStep ctms = train as CancelledTrainMovementStep;
                        if (ctms != null)
                        {
                            AddCancellation(ctms);
                        }
                        else
                        {
                            TrainMovementStep tms = train as TrainMovementStep;
                            if (tms != null)
                            {
                                AddMovement(tms);
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
                    UpdateTrainState(groupByTrain.Key, TrainState.Terminated);
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
