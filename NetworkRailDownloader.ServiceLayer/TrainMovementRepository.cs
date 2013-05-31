using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Api;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class TrainMovementRepository : DbRepository
    {
        private static readonly TiplocRepository _tiplocRepository = new TiplocRepository();

        private sealed class ScheduleHolder
        {
            public Guid ScheduleId { get; set; }
            public string TrainUid { get; set; }
            public STPIndicator STPIndicatorId { get; set; }
            public TimeSpan? Arrival { get; set; }
            public TimeSpan? Departure { get; set; }
            public TimeSpan? Pass { get; set; }
            public DateTime StartDate { get; set; }

            public TimeSpan AggregateTime
            {
                get
                {
                    return Arrival ?? Departure ?? Pass ?? TimeSpan.Zero;
                }
            }
        }

        private IEnumerable<ScheduleHolder> GetTerminatingAtSchedules(IEnumerable<short> tiplocs, DateTime date)
        {
            if (!tiplocs.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
	                ,[ScheduleTrain].[TrainUid]
	                ,[ScheduleTrain].[STPIndicatorId]
	                ,[ScheduleTrainStop].[Arrival]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] ON [ScheduleTrain].[ScheduleId] = [ScheduleTrainStop].[ScheduleId]
                WHERE [ScheduleTrainStop].[Terminate] = 1
	                AND [ScheduleTrainStop].[TiplocId] IN @tiplocs
	                AND [ScheduleTrain].[DestinationStopTiplocId] IN @tiplocs
	                AND [ScheduleTrain].[Runs{0}] = 1
	                AND @date >= [ScheduleTrain].[StartDate]
	                AND @date <= [ScheduleTrain].[EndDate]
	                AND [ScheduleTrain].[Deleted] = 0";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql, date.DayOfWeek), new
            {
                tiplocs,
                date = date.Date
            });
        }

        private IEnumerable<ScheduleHolder> GetStartingAtSchedules(IEnumerable<short> tiplocs, DateTime date)
        {
            if (!tiplocs.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
	                ,[ScheduleTrain].[TrainUid]
	                ,[ScheduleTrain].[STPIndicatorId]
	                ,[ScheduleTrainStop].[Departure]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] ON [ScheduleTrain].[ScheduleId] = [ScheduleTrainStop].[ScheduleId]
                WHERE [ScheduleTrainStop].[Origin] = 1
	                AND [ScheduleTrainStop].[TiplocId] IN @tiplocs
	                AND [ScheduleTrain].[OriginStopTiplocId] IN @tiplocs
	                AND [ScheduleTrain].[Runs{0}] = 1
	                AND @date >= [ScheduleTrain].[StartDate]
	                AND @date <= [ScheduleTrain].[EndDate]
	                AND [ScheduleTrain].[Deleted] = 0";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql, date.DayOfWeek), new
            {
                tiplocs,
                date = date.Date
            });
        }

        private IEnumerable<ScheduleHolder> GetCallingAtSchedules(IEnumerable<short> tiplocs, DateTime date)
        {
            if (!tiplocs.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
                    ,[ScheduleTrain].[TrainUid]
                    ,[ScheduleTrain].[STPIndicatorId]
                    ,[ScheduleTrainStop].[Arrival]
                    ,[ScheduleTrainStop].[Departure]
                    ,[ScheduleTrainStop].[Pass]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] ON [ScheduleTrain].[ScheduleId] = [ScheduleTrainStop].[ScheduleId]
                WHERE [ScheduleTrainStop].[TiplocId] IN @tiplocs
                    AND [ScheduleTrain].[Runs{0}] = 1
                    AND @date >= [ScheduleTrain].[StartDate]
                    AND @date <= [ScheduleTrain].[EndDate]
                    AND [ScheduleTrain].[Deleted] = 0";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql, date.DayOfWeek), new
            {
                tiplocs,
                date = date.Date
            });
        }

        private IEnumerable<ScheduleHolder> GetCallingBetweenSchedules(IEnumerable<short> tiplocsFrom, IEnumerable<short> tiplocsTo, DateTime date)
        {
            if (!tiplocsFrom.Any() || !tiplocsTo.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
                    ,[ScheduleTrain].[TrainUid]
                    ,[ScheduleTrain].[STPIndicatorId]
                    ,[FromStop].[Arrival]
                    ,[FromStop].[Departure]
                    ,[FromStop].[Pass]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] [FromStop] ON [ScheduleTrain].[ScheduleId] = [FromStop].[ScheduleId]
                INNER JOIN [ScheduleTrainStop] [ToStop] ON [ScheduleTrain].[ScheduleId] = [ToStop].[ScheduleId]
                WHERE [FromStop].[TiplocId] IN @tiplocsFrom
	                AND [ToStop].[TiplocId] IN @tiplocsTo
	                AND [FromStop].[StopNumber] < [ToStop].[StopNumber]
                    AND [ScheduleTrain].[Runs{0}] = 1
                    AND @date >= [ScheduleTrain].[StartDate]
                    AND @date <= [ScheduleTrain].[EndDate]
                    AND [ScheduleTrain].[Deleted] = 0";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql, date.DayOfWeek), new
            {
                tiplocsFrom,
                tiplocsTo,
                date = date.Date
            });
        }

        private IEnumerable<ScheduleHolder> GetDistinctSchedules(IEnumerable<ScheduleHolder> schedules)
        {
            return schedules.GroupBy(s => s.ScheduleId)
                .Select(s => s.OrderBy(sub => sub.STPIndicatorId).First());
        }

        private IEnumerable<RunningScheduleTrain> GetSchedules(IEnumerable<Guid> scheduleIds, DateTime date)
        {
            if (!scheduleIds.Any())
                return Enumerable.Empty<RunningScheduleTrain>();

            const string sql = @"
                SELECT [ScheduleTrain].[ScheduleId]
	                ,[ScheduleTrain].[TrainUid]
	                ,[ScheduleTrain].[StartDate]
	                ,[ScheduleTrain].[EndDate]
	                ,[ScheduleTrain].[STPIndicatorId]
                    ,[ScheduleTrain].[ScheduleStatusId]
	                ,[ScheduleTrain].[RunsMonday]
	                ,[ScheduleTrain].[RunsTuesday]
	                ,[ScheduleTrain].[RunsWednesday]
	                ,[ScheduleTrain].[RunsThursday]
	                ,[ScheduleTrain].[RunsFriday]
	                ,[ScheduleTrain].[RunsSaturday]
	                ,[ScheduleTrain].[RunsSunday]
	                ,[ScheduleTrain].[RunsBankHoliday]
	                ,[AtocCode].[AtocCode] AS [Code]
	                ,[AtocCode].[Name]
                FROM [ScheduleTrain]
                LEFT JOIN [AtocCode] ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
                WHERE [ScheduleTrain].[ScheduleId] IN @scheduleIds";

            using (DbConnection connection = CreateAndOpenConnection())
            {
                var schedules = connection.Query<RunningScheduleTrain, dynamic, AtocCode, RunningScheduleTrain>(
                    sql,
                    (s, d, a) =>
                    {
                        s.AtocCode = a;
                        s.Schedule = new Schedule
                        {
                            Monday = d.RunsMonday,
                            Tuesday = d.RunsTuesday,
                            Wednesday = d.RunsWednesday,
                            Thursday = d.RunsThursday,
                            Friday = d.RunsFriday,
                            Saturday = d.RunsSaturday,
                            Sunday = d.RunsSunday,
                            BankHoliday = d.RunsBankHoliday
                        };
                        return s;
                    },
                    new { scheduleIds },
                    splitOn: "RunsMonday,Code");

                var stops = GetStopsForSchedules(connection, scheduleIds);

                return schedules.Select(s =>
                {
                    s.DateFor = date;
                    s.Stops = stops.Where(stop => stop.ScheduleId == s.ScheduleId)
                        .OrderBy(stop => stop.StopNumber);
                    return s;
                });
            }
        }

        private IEnumerable<RunningScheduleRunningStop> GetStopsForSchedules(DbConnection connection, IEnumerable<Guid> scheduleIds)
        {
            if (!scheduleIds.Any())
                return Enumerable.Empty<RunningScheduleRunningStop>();

            const string sql = @"
                SELECT 
                    [ScheduleTrainStop].[ScheduleId]
                    ,[ScheduleTrainStop].[StopNumber]
                    ,[ScheduleTrainStop].[Arrival]
                    ,[ScheduleTrainStop].[Departure]
                    ,[ScheduleTrainStop].[Pass]
                    ,[ScheduleTrainStop].[PublicArrival]
                    ,[ScheduleTrainStop].[PublicDeparture]
                    ,[ScheduleTrainStop].[Line]
                    ,[ScheduleTrainStop].[Path]
                    ,[ScheduleTrainStop].[Platform]
                    ,[ScheduleTrainStop].[EngineeringAllowance]
                    ,[ScheduleTrainStop].[PathingAllowance]
                    ,[ScheduleTrainStop].[PerformanceAllowance]
                    ,[ScheduleTrainStop].[Origin]
                    ,[ScheduleTrainStop].[Intermediate]
                    ,[ScheduleTrainStop].[Terminate]
                    ,[Tiploc].[TiplocId]
                    ,[Tiploc].[Tiploc]
                    ,[Tiploc].[Nalco]
                    ,[Tiploc].[Description]
                    ,[Tiploc].[Stanox]
                    ,[Tiploc].[CRS]
                    ,[Station].[StationName]
                    ,[Station].[Location].[Lat] AS [Lat]
                    ,[Station].[Location].[Long] AS [Lon]
                FROM [ScheduleTrainStop]
                INNER JOIN [Tiploc] ON [ScheduleTrainStop].[TiplocId] = [Tiploc].[TiplocId]
                LEFT JOIN [Station] ON [Station].[TiplocId] = [Tiploc].[TiplocId]
                WHERE [ScheduleId] IN @scheduleIds";

            return connection.Query<RunningScheduleRunningStop, StationTiploc, RunningScheduleRunningStop>(
                sql,
                (st, t) =>
                {
                    st.Tiploc = t;
                    return st;
                },
                new { scheduleIds },
                splitOn: "TiplocId");
        }

        private IEnumerable<RunningScheduleTrain> GetTerminatingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var scheduleIds = GetDistinctSchedules(GetTerminatingAtSchedules(tiplocs, date));

            return GetSchedules(scheduleIds, date, startTime, endTime);
        }

        private IEnumerable<RunningScheduleTrain> GetStartingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var scheduleIds = GetDistinctSchedules(GetStartingAtSchedules(tiplocs, date));

            return GetSchedules(scheduleIds, date, startTime, endTime);
        }

        private IEnumerable<RunningScheduleTrain> GetCallingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var scheduleIds = GetDistinctSchedules(GetCallingAtSchedules(tiplocs, date));

            return GetSchedules(scheduleIds, date, startTime, endTime);
        }

        private IEnumerable<RunningScheduleTrain> GetCallingBetweenSchedules(IEnumerable<short> tiplocsFrom, IEnumerable<short> tiplocsTo, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var scheduleIds = GetDistinctSchedules(GetCallingBetweenSchedules(tiplocsFrom, tiplocsTo, date));

            return GetSchedules(scheduleIds, date, startTime, endTime);
        }

        private static readonly TimeSpan EndOfDay = new TimeSpan(23, 59, 59);

        private IEnumerable<RunningScheduleTrain> GetSchedules(IEnumerable<ScheduleHolder> schedules, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime == TimeSpan.Zero)
                endTime = EndOfDay;

            var filteredScheduleIds = schedules
                .Where(s => s.AggregateTime >= startTime)
                .Where(s => s.AggregateTime < endTime)
                .Select(s => s.ScheduleId);

            return GetSchedules(filteredScheduleIds, date);
        }

        private IEnumerable<RunningTrainActual> GetActualSchedule(IEnumerable<Guid> scheduleIds, DateTime startDate, DateTime endDate)
        {
            if (!scheduleIds.Any())
                return Enumerable.Empty<RunningTrainActual>();

            const string sql = @"
                SELECT [LiveTrain].[Id]
	                ,[LiveTrain].[TrainId]
	                ,[LiveTrain].[Headcode]
	                ,[LiveTrain].[CreationTimestamp] AS [Activated]
	                ,[LiveTrain].[OriginDepartTimestamp]
	                ,[LiveTrain].[TrainServiceCode]
	                ,[LiveTrain].[TrainStateId] AS [State]
                    ,[LiveTrain].[ScheduleTrain] AS [ScheduleId]
                    ,[Tiploc].[TiplocId]
                    ,[Tiploc].[Tiploc]
                    ,[Tiploc].[Nalco]
                    ,[Tiploc].[Description]
                    ,[Tiploc].[Stanox]
                    ,[Tiploc].[CRS]
                    ,[Station].[StationName]
                    ,[Station].[Location].[Lat] AS [Lat]
                    ,[Station].[Location].[Long] AS [Lon]
                FROM [LiveTrain]
                INNER JOIN [Tiploc] ON [LiveTrain].[OriginTiplocId] = [Tiploc].[TiplocId]
                LEFT JOIN [Station] ON [Station].[TiplocId] = [Tiploc].[TiplocId]
                WHERE [LiveTrain].[ScheduleTrain] IN @scheduleIds
	                AND [LiveTrain].[OriginDepartTimestamp] >= @startDate
	                AND [LiveTrain].[OriginDepartTimestamp] < @endDate";

            using (DbConnection connection = CreateAndOpenConnection())
            {
                var actual = connection.Query<RunningTrainActual, StationTiploc, RunningTrainActual>(
                    sql,
                    (a, o) =>
                    {
                        a.ScheduleOrigin = o;
                        return a;
                    },
                    new { scheduleIds, startDate, endDate },
                    splitOn: "TiplocId");

                var stops = GetActualStops(connection, actual.Select(a => a.Id));

                return actual.Select(s =>
                {
                    s.Stops = stops.Where(stop => stop.TrainId == s.Id)
                        .OrderBy(stop => stop.ActualTimestamp);
                    return s;
                });
            }
        }

        private IEnumerable<RunningTrainActualStop> GetActualStops(DbConnection connection, IEnumerable<Guid> liveTrainIds)
        {
            if (!liveTrainIds.Any())
                return Enumerable.Empty<RunningTrainActualStop>();

            const string sql = @"
                SELECT [LiveTrainStop].[TrainStopId]
	                ,[LiveTrainStop].[TrainId]
	                ,[LiveTrainStop].[EventTypeId] AS [EventType]
	                ,[LiveTrainStop].[PlannedTimestamp]
	                ,[LiveTrainStop].[ActualTimestamp]
	                ,[LiveTrainStop].[Platform]
	                ,[LiveTrainStop].[Line]
	                ,[LiveTrainStop].[ScheduleStopNumber]
                    ,[Tiploc].[TiplocId]
                    ,[Tiploc].[Tiploc]
                    ,[Tiploc].[Nalco]
                    ,[Tiploc].[Description]
                    ,[Tiploc].[Stanox]
                    ,[Tiploc].[CRS]
                    ,[Station].[StationName]
                    ,[Station].[Location].[Lat] AS [Lat]
                    ,[Station].[Location].[Long] AS [Lon]
                FROM [LiveTrainStop]
                INNER JOIN [Tiploc] ON [LiveTrainStop].[ReportingTiplocId] = [Tiploc].[TiplocId]
                LEFT JOIN [Station] ON [Station].[TiplocId] = [Tiploc].[TiplocId]
                WHERE [LiveTrainStop].[TrainId] IN @liveTrainIds";

            return connection.Query<RunningTrainActualStop, StationTiploc, RunningTrainActualStop>(
                sql,
                (st, t) =>
                {
                    st.Tiploc = t;
                    return st;
                },
                new { liveTrainIds },
                splitOn: "TiplocId");
        }

        public IEnumerable<TrainMovementResult> TerminatingAtStation(string crsCode, DateTime? startDate, DateTime? endDate)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocs = _tiplocRepository.GetAllByCRSCode(crsCode)
                .Select(t => t.TiplocId);

            if (!tiplocs.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return TerminatingAt(tiplocs, startDate, endDate);
        }

        public IEnumerable<TrainMovementResult> TerminatingAtLocation(string stanox, DateTime? startDate, DateTime? endDate)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocs = _tiplocRepository.GetAllByStanox(stanox)
                .Select(t => t.TiplocId);

            if (!tiplocs.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return TerminatingAt(tiplocs, startDate, endDate);
        }

        private IEnumerable<TrainMovementResult> TerminatingAt(IEnumerable<short> tiplocs, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Value.Date != endDate.Value.Date)
            {
                nextDaySchedules = GetTerminatingAtSchedules(tiplocs, endDate.Value.Date, endDate.Value.Date.TimeOfDay, endDate.Value.TimeOfDay);
                endDate = startDate.Value.Date.AddDays(1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetTerminatingAtSchedules(tiplocs, startDate.Value.Date, startDate.Value.TimeOfDay, endDate.Value.TimeOfDay)
                .Union(nextDaySchedules)
                .ToList();

            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate.Value, endDate.Value);

            IEnumerable<ExtendedCancellation> cancellations = null;
            IEnumerable<Reinstatement> reinstatements = null;
            IEnumerable<ChangeOfOrigin> changeOfOrigins = null;

            if (allActualData.Any())
            {
                using (DbConnection connection = CreateAndOpenConnection())
                {
                    cancellations = GetCancellations(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    reinstatements = GetReinstatements(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    changeOfOrigins = GetChangeOfOrigins(allActualData.Select(s => s.Id), connection)
                        .ToList();
                }
            }
            else
            {
                cancellations = Enumerable.Empty<ExtendedCancellation>();
                reinstatements = Enumerable.Empty<Reinstatement>();
                changeOfOrigins = Enumerable.Empty<ChangeOfOrigin>();
            }

            ICollection<TrainMovementResult> results = new List<TrainMovementResult>(allSchedules.Count());
            foreach (var schedule in allSchedules)
            {
                var actual = allActualData.SingleOrDefault(a => a.ScheduleId == schedule.ScheduleId);
                var can = actual != null ?
                    cancellations.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ExtendedCancellation>();
                var rein = actual != null ?
                    reinstatements.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<Reinstatement>();
                var coo = actual != null ?
                    changeOfOrigins.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ChangeOfOrigin>();
                results.Add(new TrainMovementResult
                {
                    Schedule = schedule,
                    Actual = actual,
                    Cancellations = can,
                    Reinstatements = rein,
                    ChangeOfOrigins = coo
                });
            }

            return results
                .OrderBy(s => s.Schedule.DateFor)
                .ThenBy(s => s.Schedule.DepartureTime);
        }

        public IEnumerable<TrainMovementResult> StartingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocs = _tiplocRepository.GetAllByStanox(stanox)
                .Select(t => t.TiplocId);

            if (!tiplocs.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return StartingAt(tiplocs, startDate, endDate);
        }

        public IEnumerable<TrainMovementResult> StartingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocs = _tiplocRepository.GetAllByCRSCode(crsCode)
                .Select(t => t.TiplocId);

            if (!tiplocs.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return StartingAt(tiplocs, startDate, endDate);
        }

        private IEnumerable<TrainMovementResult> StartingAt(IEnumerable<short> tiplocs, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Value.Date != endDate.Value.Date)
            {
                nextDaySchedules = GetStartingAtSchedules(tiplocs, endDate.Value.Date, endDate.Value.Date.TimeOfDay, endDate.Value.TimeOfDay);
                endDate = startDate.Value.Date.AddDays(1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetStartingAtSchedules(tiplocs, startDate.Value.Date, startDate.Value.TimeOfDay, endDate.Value.TimeOfDay)
                .Union(nextDaySchedules)
                .ToList();

            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate.Value, endDate.Value);

            IEnumerable<ExtendedCancellation> cancellations = null;
            IEnumerable<Reinstatement> reinstatements = null;
            IEnumerable<ChangeOfOrigin> changeOfOrigins = null;

            if (allActualData.Any())
            {
                using (DbConnection connection = CreateAndOpenConnection())
                {
                    cancellations = GetCancellations(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    reinstatements = GetReinstatements(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    changeOfOrigins = GetChangeOfOrigins(allActualData.Select(s => s.Id), connection)
                        .ToList();
                }
            }
            else
            {
                cancellations = Enumerable.Empty<ExtendedCancellation>();
                reinstatements = Enumerable.Empty<Reinstatement>();
                changeOfOrigins = Enumerable.Empty<ChangeOfOrigin>();
            }

            ICollection<TrainMovementResult> results = new List<TrainMovementResult>(allSchedules.Count());
            foreach (var schedule in allSchedules)
            {
                var actual = allActualData.SingleOrDefault(a => a.ScheduleId == schedule.ScheduleId);
                var can = actual != null ?
                    cancellations.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ExtendedCancellation>();
                var rein = actual != null ?
                    reinstatements.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<Reinstatement>();
                var coo = actual != null ?
                    changeOfOrigins.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ChangeOfOrigin>();
                results.Add(new TrainMovementResult
                {
                    Schedule = schedule,
                    Actual = actual,
                    Cancellations = can,
                    Reinstatements = rein,
                    ChangeOfOrigins = coo
                });
            }

            return results
                .OrderBy(s => s.Schedule.DateFor)
                .ThenBy(s => s.Schedule.DepartureTime);
        }

        public IEnumerable<TrainMovementResult> CallingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocs = _tiplocRepository.GetAllByStanox(stanox)
                .Select(t => t.TiplocId);

            if (!tiplocs.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return CallingAt(tiplocs, startDate, endDate);
        }

        public IEnumerable<TrainMovementResult> CallingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocs = _tiplocRepository.GetAllByCRSCode(crsCode)
                .Select(t => t.TiplocId);

            if (!tiplocs.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return CallingAt(tiplocs, startDate, endDate);
        }

        private IEnumerable<TrainMovementResult> CallingAt(IEnumerable<short> tiplocs, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Value.Date != endDate.Value.Date)
            {
                nextDaySchedules = GetCallingAtSchedules(tiplocs, endDate.Value.Date, endDate.Value.Date.TimeOfDay, endDate.Value.TimeOfDay);
                endDate = startDate.Value.Date.AddDays(1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetCallingAtSchedules(tiplocs, startDate.Value.Date, startDate.Value.TimeOfDay, endDate.Value.TimeOfDay)
                .Union(nextDaySchedules)
                .ToList();

            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate.Value, endDate.Value);

            IEnumerable<ExtendedCancellation> cancellations = null;
            IEnumerable<Reinstatement> reinstatements = null;
            IEnumerable<ChangeOfOrigin> changeOfOrigins = null;

            if (allActualData.Any())
            {
                using (DbConnection connection = CreateAndOpenConnection())
                {
                    cancellations = GetCancellations(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    reinstatements = GetReinstatements(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    changeOfOrigins = GetChangeOfOrigins(allActualData.Select(s => s.Id), connection)
                        .ToList();
                }
            }
            else
            {
                cancellations = Enumerable.Empty<ExtendedCancellation>();
                reinstatements = Enumerable.Empty<Reinstatement>();
                changeOfOrigins = Enumerable.Empty<ChangeOfOrigin>();
            }

            ICollection<TrainMovementResult> results = new List<TrainMovementResult>(allSchedules.Count());
            foreach (var schedule in allSchedules)
            {
                var actual = allActualData.SingleOrDefault(a => a.ScheduleId == schedule.ScheduleId);
                var can = actual != null ?
                    cancellations.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ExtendedCancellation>();
                var rein = actual != null ?
                    reinstatements.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<Reinstatement>();
                var coo = actual != null ?
                    changeOfOrigins.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ChangeOfOrigin>();
                results.Add(new TrainMovementResult
                {
                    Schedule = schedule,
                    Actual = actual,
                    Cancellations = can,
                    Reinstatements = rein,
                    ChangeOfOrigins = coo
                });
            }

            return results
                .OrderBy(s => s.Schedule.DateFor)
                .ThenBy(s => s.Schedule.DepartureTime);
        }

        public IEnumerable<TrainMovementResult> CallingBetweenLocations(string fromStanox, string toStanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocsFrom = _tiplocRepository.GetAllByStanox(fromStanox)
                .Select(t => t.TiplocId);
            var tiplocsTo = _tiplocRepository.GetAllByStanox(toStanox)
                .Select(t => t.TiplocId);

            if (!tiplocsFrom.Any() || !tiplocsTo.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return CallingBetween(tiplocsFrom, tiplocsTo, startDate, endDate);
        }

        public IEnumerable<TrainMovementResult> CallingBetweenStations(string fromCrs, string toCrs, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59));

            if ((endDate.Value - startDate.Value) > TimeSpan.FromDays(1))
            {
                endDate = startDate.Value.AddDays(1);
            }

            // get tiploc id to improve query
            var tiplocsFrom = _tiplocRepository.GetAllByCRSCode(fromCrs)
                .Select(t => t.TiplocId);
            var tiplocsTo = _tiplocRepository.GetAllByCRSCode(toCrs)
                .Select(t => t.TiplocId);

            if (!tiplocsFrom.Any() || !tiplocsTo.Any())
                return Enumerable.Empty<TrainMovementResult>();

            return CallingBetween(tiplocsFrom, tiplocsTo, startDate, endDate);
        }

        private IEnumerable<TrainMovementResult> CallingBetween(IEnumerable<short> tiplocsFrom, IEnumerable<short> tiplocsTo, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Value.Date != endDate.Value.Date)
            {
                nextDaySchedules = GetCallingBetweenSchedules(tiplocsFrom, tiplocsTo, endDate.Value.Date, endDate.Value.Date.TimeOfDay, endDate.Value.TimeOfDay);
                endDate = startDate.Value.Date.AddDays(1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetCallingBetweenSchedules(tiplocsFrom, tiplocsTo, startDate.Value.Date, startDate.Value.TimeOfDay, endDate.Value.TimeOfDay)
                .Union(nextDaySchedules)
                .ToList();

            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate.Value, endDate.Value);

            IEnumerable<ExtendedCancellation> cancellations = null;
            IEnumerable<Reinstatement> reinstatements = null;
            IEnumerable<ChangeOfOrigin> changeOfOrigins = null;

            if (allActualData.Any())
            {
                using (DbConnection connection = CreateAndOpenConnection())
                {
                    cancellations = GetCancellations(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    reinstatements = GetReinstatements(allActualData.Select(s => s.Id), connection)
                        .ToList();
                    changeOfOrigins = GetChangeOfOrigins(allActualData.Select(s => s.Id), connection)
                        .ToList();
                }
            }
            else
            {
                cancellations = Enumerable.Empty<ExtendedCancellation>();
                reinstatements = Enumerable.Empty<Reinstatement>();
                changeOfOrigins = Enumerable.Empty<ChangeOfOrigin>();
            }

            ICollection<TrainMovementResult> results = new List<TrainMovementResult>(allSchedules.Count());
            foreach (var schedule in allSchedules)
            {
                var actual = allActualData.SingleOrDefault(a => a.ScheduleId == schedule.ScheduleId);
                var can = actual != null ?
                    cancellations.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ExtendedCancellation>();
                var rein = actual != null ?
                    reinstatements.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<Reinstatement>();
                var coo = actual != null ?
                    changeOfOrigins.Where(c => c.TrainId == actual.Id).ToList() :
                    Enumerable.Empty<ChangeOfOrigin>();
                results.Add(new TrainMovementResult
                {
                    Schedule = schedule,
                    Actual = actual,
                    Cancellations = can,
                    Reinstatements = rein,
                    ChangeOfOrigins = coo
                });
            }

            return results
                .OrderBy(s => s.Schedule.DateFor)
                .ThenBy(s => s.Schedule.DepartureTime);
        }

        [Obsolete("Will be removed in future version")]
        public ViewModelTrainMovement GetTrainMovementById(string trainId)
        {
            const string sql = @"
                SELECT TOP 1
                    [LiveTrain].[OriginDepartTimestamp] AS [SchedOriginDeparture]
                    ,[ScheduleTrain].[TrainUid] AS [TrainUid]
                FROM [LiveTrain]
                INNER JOIN [ScheduleTrain] ON [LiveTrain].[ScheduleTrain] = [ScheduleTrain].[ScheduleId]
                INNER JOIN  [Tiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [Tiploc].[TiplocId]
                WHERE [LiveTrain].[TrainId] = @trainId
                ORDER BY [LiveTrain].[OriginDepartTimestamp] DESC"; // get latest occurance

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                ViewModelTrainMovement tm = dbConnection.Query<ViewModelTrainMovement>(sql, new
                {
                    trainId
                }).FirstOrDefault();
                return tm;
            }
        }

        public ViewModelTrainMovement GetTrainMovementById(string trainUid, DateTime date)
        {
            const string sql = @"
                SELECT TOP 1
                    [LiveTrain].[Id] AS [UniqueId]
                    ,[LiveTrain].[TrainId] AS [Id]
                    ,[LiveTrain].[Headcode] AS [HeadCode]
                    ,[LiveTrain].[CreationTimestamp] AS [Activated]
                    ,[LiveTrain].[OriginDepartTimestamp] AS [SchedOriginDeparture]
                    ,[LiveTrain].[TrainServiceCode] AS [ServiceCode]
                    ,[ScheduleTrain].[TrainUid] AS [TrainUid]
                    ,[AtocCode].[AtocCode] AS [Code]
                    ,[AtocCode].[Name]
                    ,[OriginTiploc].[TiplocId]
                    ,[OriginTiploc].[Tiploc]
                    ,[OriginTiploc].[Nalco]
                    ,[OriginTiploc].[Description]
                    ,[OriginTiploc].[Stanox]
                    ,[OriginTiploc].[CRS]
                FROM [LiveTrain]
                INNER JOIN [ScheduleTrain] ON [LiveTrain].[ScheduleTrain] = [ScheduleTrain].[ScheduleId]
                LEFT JOIN [AtocCode] ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
                INNER JOIN  [Tiploc] [OriginTiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [OriginTiploc].[TiplocId]
                WHERE [ScheduleTrain].[TrainUid] = @trainUid AND [LiveTrain].[OriginDepartTimestamp] >= @date
                ORDER BY [LiveTrain].[OriginDepartTimestamp] ASC";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                ViewModelTrainMovement train = dbConnection.Query<ViewModelTrainMovement, AtocCode, ScheduleTiploc, ViewModelTrainMovement>(
                    sql,
                    (tm, ac, ot) =>
                    {
                        tm.AtocCode = ac;
                        tm.Origin = ot;
                        return tm;
                    },
                    new
                    {
                        trainUid,
                        date
                    },
                    splitOn: "Code,TiplocId").FirstOrDefault();

                if (train != null)
                {
                    train.Cancellation = GetCancellations(new[] { train.UniqueId }, dbConnection).FirstOrDefault();
                    train.Reinstatement = GetReinstatements(new[] { train.UniqueId }, dbConnection).FirstOrDefault();
                    train.ChangeOfOrigin = GetChangeOfOrigins(new[] { train.UniqueId }, dbConnection).FirstOrDefault();
                    train.Steps = GetTmSteps(train.UniqueId, dbConnection);
                    return train;
                }
            }

            return null;
        }

        private IEnumerable<TrainMovementStepViewModel> GetTmSteps(Guid trainId, DbConnection dbConnection)
        {
            const string tmsSql = @"
                SELECT
                    [EventTypeId]
                    ,[PlannedTimestamp] AS [PlannedTime]
                    ,[ActualTimestamp]
                    ,[Platform]
                    ,[Line]
                    ,[ScheduleStopNumber]
                    ,[Tiploc].[Stanox]
                    ,[Tiploc].[TiplocId]
                    ,[Tiploc].[Tiploc]
                    ,[Tiploc].[Nalco]
                    ,[Tiploc].[Description]
                    ,[Tiploc].[Stanox]
                    ,[Tiploc].[CRS]
                    ,[Station].[StationName]
                    ,[Station].[Location].[Lat] AS [Lat]
                    ,[Station].[Location].[Long] AS [Lon]
                FROM [LiveTrainStop]
                INNER JOIN  [Tiploc] ON [LiveTrainStop].[ReportingTiplocId] = [Tiploc].[TiplocId]
                LEFT JOIN [Station] ON [Station].[TiplocId] = [Tiploc].[TiplocId]
                WHERE [TrainId] = @trainId
                ORDER BY [ScheduleStopNumber] ASC, [EventTypeId] DESC";

            IEnumerable<TrainMovementStepViewModel> tmSteps = dbConnection.Query<TrainMovementStepViewModel, ScheduleTiploc, TrainMovementStepViewModel>(
                    tmsSql,
                    (tms, loc) =>
                    {
                        tms.Station = loc;
                        return tms;
                    },
                    new
                    {
                        trainId
                    },
                    splitOn: "TiplocId").ToList();

            return tmSteps;
        }

        private IEnumerable<ExtendedCancellation> GetCancellations(IEnumerable<Guid> trainIds, DbConnection connection)
        {
            const string sql = @"
                SELECT
                    [LiveTrainCancellation].[TrainId]
                    ,[LiveTrainCancellation].[CancelledTimestamp]
                    ,[LiveTrainCancellation].[Stanox] AS [CancelledStanox]
                    ,[LiveTrainCancellation].[ReasonCode]
                    ,[LiveTrainCancellation].[Type]
                    ,[DelayAttributionCodes].[Description]
                    ,[CancelTiploc].[TiplocId]
                    ,[CancelTiploc].[Tiploc]
                    ,[CancelTiploc].[Nalco]
                    ,[CancelTiploc].[Description]
                    ,[CancelTiploc].[Stanox]
                    ,[CancelTiploc].[CRS]
                    ,[Station].[StationName]
                    ,[Station].[Location].[Lat] AS [Lat]
                    ,[Station].[Location].[Long] AS [Lon]
                FROM [LiveTrainCancellation]
                LEFT JOIN [DelayAttributionCodes] ON [LiveTrainCancellation].[ReasonCode] = [DelayAttributionCodes].[ReasonCode]
                LEFT JOIN [Tiploc] AS [CancelTiploc] ON [LiveTrainCancellation].[Stanox] = [CancelTiploc].[Stanox]
                LEFT JOIN [Station] ON [Station].[TiplocId] = [CancelTiploc].[TiplocId]
                WHERE [LiveTrainCancellation].[TrainId] IN @trainIds";

            return connection.Query<ExtendedCancellation, StationTiploc, ExtendedCancellation>(sql,
                (c, t) =>
                {
                    c.CancelledAt = t;
                    return c;
                }, new { trainIds }, splitOn: "TiplocId");
        }

        private IEnumerable<Reinstatement> GetReinstatements(IEnumerable<Guid> trainIds, DbConnection connection)
        {
            const string sql = @"
                SELECT 
                    [LiveTrainReinstatement].[TrainId]
                    ,[LiveTrainReinstatement].[PlannedDepartureTime]
                    ,[ReinstatementTiploc].[TiplocId]
                    ,[ReinstatementTiploc].[Tiploc]
                    ,[ReinstatementTiploc].[Nalco]
                    ,[ReinstatementTiploc].[Description]
                    ,[ReinstatementTiploc].[Stanox]
                    ,[ReinstatementTiploc].[CRS]
                    ,[Station].[StationName]
                    ,[Station].[Location].[Lat] AS [Lat]
                    ,[Station].[Location].[Long] AS [Lon]
                FROM [LiveTrainReinstatement]
                INNER JOIN [Tiploc] AS [ReinstatementTiploc] ON [LiveTrainReinstatement].[ReinstatedTiplocId] = [ReinstatementTiploc].[TiplocId]
                LEFT JOIN [Station] ON [Station].[TiplocId] = [ReinstatementTiploc].[TiplocId]
                WHERE [LiveTrainReinstatement].[TrainId] IN @trainIds";

            return connection.Query<Reinstatement, StationTiploc, Reinstatement>(sql,
                (r, t) =>
                {
                    r.NewOrigin = t;
                    return r;
                }, new { trainIds }, splitOn: "TiplocId");
        }

        private IEnumerable<ChangeOfOrigin> GetChangeOfOrigins(IEnumerable<Guid> trainIds, DbConnection connection)
        {
            const string sql = @"
                SELECT 
                    [LiveTrainChangeOfOrigin].[TrainId]
                    ,[LiveTrainChangeOfOrigin].[NewDepartureTime]
                    ,[LiveTrainChangeOfOrigin].[ReasonCode]
                    ,[ChangeOfOriginDelay].[Description]      
                    ,[ChangeOfOriginTiploc].[TiplocId]
                    ,[ChangeOfOriginTiploc].[Tiploc]
                    ,[ChangeOfOriginTiploc].[Nalco]
                    ,[ChangeOfOriginTiploc].[Description]
                    ,[ChangeOfOriginTiploc].[Stanox]
                    ,[ChangeOfOriginTiploc].[CRS]
                    ,[Station].[StationName]
                    ,[Station].[Location].[Lat] AS [Lat]
                    ,[Station].[Location].[Long] AS [Lon]
                FROM [LiveTrainChangeOfOrigin]
                INNER JOIN [Tiploc] AS [ChangeOfOriginTiploc] ON [LiveTrainChangeOfOrigin].[NewTiplocId] = [ChangeOfOriginTiploc].[TiplocId]
                LEFT JOIN [Station] ON [Station].[TiplocId] = [ChangeOfOriginTiploc].[TiplocId]
                LEFT JOIN [DelayAttributionCodes] [ChangeOfOriginDelay] ON [LiveTrainChangeOfOrigin].[ReasonCode] = [ChangeOfOriginDelay].[ReasonCode]
                WHERE [LiveTrainChangeOfOrigin].[TrainId] IN @trainIds";

            return connection.Query<ChangeOfOrigin, StationTiploc, ChangeOfOrigin>(sql,
                (o, t) =>
                {
                    o.NewOrigin = t;
                    return o;
                }, new { trainIds }, splitOn: "TiplocId");
        }
    }
}
