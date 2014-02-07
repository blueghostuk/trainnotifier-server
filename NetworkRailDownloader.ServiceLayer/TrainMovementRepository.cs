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
        private const string _atocCodeFilter = " AND [ScheduleTrain].[AtocCode] = @atocCode";
        private const string _powerTypeFilter = " AND [ScheduleTrain].[PowerTypeId] = @powerType";

        private static readonly TiplocRepository _tiplocRepository = new TiplocRepository();

        private sealed class ScheduleHolder
        {
            public Guid ScheduleId { get; set; }
            public Guid? StopsScheduleId { get; set; }
            public string TrainUid { get; set; }
            public STPIndicator STPIndicatorId { get; set; }
        }

        private IEnumerable<ScheduleHolder> GetRunningTerminatingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            if (!tiplocs.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
	                ,[ScheduleTrain].[TrainUid]
	                ,[ScheduleTrain].[STPIndicatorId]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] ON [ScheduleTrain].[ScheduleId] = [ScheduleTrainStop].[ScheduleId]
                WHERE [ScheduleTrainStop].[Terminate] = 1
	                AND [ScheduleTrainStop].[TiplocId] IN @tiplocs
	                AND [ScheduleTrain].[DestinationStopTiplocId] IN @tiplocs
	                AND [ScheduleTrain].[Runs{0}] = 1
	                AND @date >= [ScheduleTrain].[StartDate]
	                AND @date <= [ScheduleTrain].[EndDate]
                    AND COALESCE(Arrival, Departure, Pass) >= @startTime
                    AND COALESCE(Arrival, Departure, Pass) < @endTime
	                AND [ScheduleTrain].[Deleted] = 0{1}{2}
                    ORDER BY COALESCE(Arrival, Departure, Pass)";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql,
                date.DayOfWeek,
                (!string.IsNullOrEmpty(atocCode) ? _atocCodeFilter : string.Empty),
                (powerType.HasValue ? _powerTypeFilter : string.Empty)), new
                {
                    tiplocs,
                    date = date.Date,
                    startTime,
                    endTime,
                    atocCode,
                    powerType
                });
        }

        private IEnumerable<ScheduleHolder> GetRunningStartingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            if (!tiplocs.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
	                ,[ScheduleTrain].[TrainUid]
	                ,[ScheduleTrain].[STPIndicatorId]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] ON [ScheduleTrain].[ScheduleId] = [ScheduleTrainStop].[ScheduleId]
                WHERE [ScheduleTrainStop].[Origin] = 1
	                AND [ScheduleTrainStop].[TiplocId] IN @tiplocs
	                AND [ScheduleTrain].[OriginStopTiplocId] IN @tiplocs
	                AND [ScheduleTrain].[Runs{0}] = 1
	                AND @date >= [ScheduleTrain].[StartDate]
	                AND @date <= [ScheduleTrain].[EndDate]
                    AND COALESCE(Arrival, Departure, Pass) >= @startTime
                    AND COALESCE(Arrival, Departure, Pass) < @endTime
	                AND [ScheduleTrain].[Deleted] = 0{1}{2}
                    ORDER BY COALESCE(Arrival, Departure, Pass)";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql,
                date.DayOfWeek,
                (!string.IsNullOrEmpty(atocCode) ? _atocCodeFilter : string.Empty),
                (powerType.HasValue ? _powerTypeFilter : string.Empty)), new
                {
                    tiplocs,
                    date = date.Date,
                    startTime,
                    endTime,
                    atocCode,
                    powerType
                });
        }

        private IEnumerable<ScheduleHolder> GetRunningAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            if (!tiplocs.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
                    ,[ScheduleTrain].[TrainUid]
                    ,[ScheduleTrain].[STPIndicatorId]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] ON [ScheduleTrain].[ScheduleId] = [ScheduleTrainStop].[ScheduleId]
                WHERE [ScheduleTrainStop].[TiplocId] IN @tiplocs
                    AND [ScheduleTrain].[Runs{0}] = 1
                    AND @date >= [ScheduleTrain].[StartDate]
                    AND @date <= [ScheduleTrain].[EndDate]
                    AND COALESCE(Arrival, Departure, Pass) >= @startTime
                    AND COALESCE(Arrival, Departure, Pass) < @endTime
                    AND [ScheduleTrain].[Deleted] = 0{1}{2}
                    ORDER BY COALESCE(Arrival, Departure, Pass)";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql,
                date.DayOfWeek,
                (!string.IsNullOrEmpty(atocCode) ? _atocCodeFilter : string.Empty),
                (powerType.HasValue ? _powerTypeFilter : string.Empty)), new
                {
                    tiplocs,
                    date = date.Date,
                    startTime,
                    endTime,
                    atocCode,
                    powerType
                });
        }

        private IEnumerable<ScheduleHolder> GetRunningCallingBetweenSchedules(IEnumerable<short> tiplocsFrom, IEnumerable<short> tiplocsTo, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            if (!tiplocsFrom.Any() || !tiplocsTo.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
                    ,[ScheduleTrain].[TrainUid]
                    ,[ScheduleTrain].[STPIndicatorId]
                FROM [ScheduleTrain]
                INNER JOIN [ScheduleTrainStop] [FromStop] ON [ScheduleTrain].[ScheduleId] = [FromStop].[ScheduleId]
                INNER JOIN [ScheduleTrainStop] [ToStop] ON [ScheduleTrain].[ScheduleId] = [ToStop].[ScheduleId]
                WHERE [FromStop].[TiplocId] IN @tiplocsFrom
	                AND [ToStop].[TiplocId] IN @tiplocsTo
	                AND [FromStop].[StopNumber] < [ToStop].[StopNumber]
                    AND [ScheduleTrain].[Runs{0}] = 1
                    AND @date >= [ScheduleTrain].[StartDate]
                    AND @date <= [ScheduleTrain].[EndDate]
                    AND COALESCE([FromStop].[Arrival], [FromStop].[Departure], [FromStop].[Pass]) >= @startTime
                    AND COALESCE([FromStop].[Arrival], [FromStop].[Departure], [FromStop].[Pass]) < @endTime
                    AND [ScheduleTrain].[Deleted] = 0{1}{2}
                    ORDER BY COALESCE([FromStop].[Arrival], [FromStop].[Departure], [FromStop].[Pass])";

            return Query<ScheduleHolder>(string.Format(getSchedulesSql,
                date.DayOfWeek,
                (!string.IsNullOrEmpty(atocCode) ? _atocCodeFilter : string.Empty),
                (powerType.HasValue ? _powerTypeFilter : string.Empty)), new
                {
                    tiplocsFrom,
                    tiplocsTo,
                    date = date.Date,
                    startTime,
                    endTime,
                    atocCode,
                    powerType
                });
        }

        private IEnumerable<ScheduleHolder> GetDistinctSchedules(IEnumerable<ScheduleHolder> schedules, DateTime date, string atocCode, PowerType? powerType)
        {
            if (!schedules.Any())
                return Enumerable.Empty<ScheduleHolder>();

            return schedules
                .Union(GetMatchingSchedules(schedules, date, atocCode, powerType))
                .GroupBy(s => s.TrainUid)
                .Select(s =>
                {
                    var firstSchedule = s
                        .OrderBy(sub => sub.STPIndicatorId)
                        .First();
                    // if is cancellation then wont have any stops associated, so get previous schedule if applicable
                    if (firstSchedule.STPIndicatorId == STPIndicator.Cancellation)
                    {
                        firstSchedule.StopsScheduleId = s
                            .Except(new[] { firstSchedule })
                            .OrderBy(sub => sub.STPIndicatorId)
                            .Select(sub => sub.ScheduleId)
                            .FirstOrDefault();
                    }
                    return firstSchedule;
                })
                .ToList();
        }

        /// <summary>
        /// this will pick up cancelled schedules which dont have any stops
        /// </summary>
        private IEnumerable<ScheduleHolder> GetMatchingSchedules(IEnumerable<ScheduleHolder> schedules, DateTime date, string atocCode, PowerType? powerType)
        {
            if (!schedules.Any())
                return Enumerable.Empty<ScheduleHolder>();

            const string sql = @"
                SELECT [ScheduleTrain].[ScheduleId]
                    ,[ScheduleTrain].[TrainUid]
                    ,[ScheduleTrain].[STPIndicatorId]
                    FROM [ScheduleTrain]
                    WHERE 
                        [TrainUid] IN @trainUids
                        AND @date >= [StartDate]
                        AND @date <= [EndDate]
                        AND [Deleted] = 0
                        AND [Runs{0}] = 1{1}{2}
                    ORDER BY [STPIndicatorId]";

            return Query<ScheduleHolder>(string.Format(sql,
                date.DayOfWeek,
                (!string.IsNullOrEmpty(atocCode) ? _atocCodeFilter : string.Empty),
                (powerType.HasValue ? _powerTypeFilter : string.Empty)), new
                {
                    trainUids = schedules.Select(t => t.TrainUid).Distinct(),
                    date = date.Date,
                    atocCode,
                    powerType
                });
        }

        private IEnumerable<RunningScheduleTrain> GetSchedules(IEnumerable<Guid> scheduleIds, DateTime date)
        {
            if (!scheduleIds.Any())
                return Enumerable.Empty<RunningScheduleTrain>();

            const string sql = @"
                SELECT [ScheduleTrain].[ScheduleId]
	                ,[ScheduleTrain].[TrainUid]
	                ,[ScheduleTrain].[Headcode]
	                ,[ScheduleTrain].[StartDate]
	                ,[ScheduleTrain].[EndDate]
	                ,[ScheduleTrain].[STPIndicatorId]
                    ,[ScheduleTrain].[ScheduleStatusId]
                    ,[ScheduleTrain].[PowerTypeId]
                    ,[ScheduleTrain].[CategoryTypeId]
                    ,[ScheduleTrain].[Speed]
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
                var trains = connection.Query<RunningScheduleTrain, dynamic, AtocCode, RunningScheduleTrain>(
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

                return trains.Select(sc =>
                {
                    sc.DateFor = date;
                    sc.Stops = stops.Where(stop => stop.ScheduleId == sc.ScheduleId)
                        .OrderBy(stop => stop.StopNumber)
                        .ToList();
                    return sc;
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

        private IEnumerable<RunningScheduleTrain> GetTerminatingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            var schedules = GetDistinctSchedules(GetRunningTerminatingAtSchedules(tiplocs, date, startTime, endTime, atocCode, powerType), date, atocCode, powerType);

            return GetRunningTrainSchedules(schedules, date);
        }

        private IEnumerable<RunningScheduleTrain> GetStartingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            var schedules = GetDistinctSchedules(GetRunningStartingAtSchedules(tiplocs, date, startTime, endTime, atocCode, powerType), date, atocCode, powerType);

            return GetRunningTrainSchedules(schedules, date);
        }

        private IEnumerable<RunningScheduleTrain> GetCallingAtSchedules(IEnumerable<short> tiplocs, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            var schedules = GetDistinctSchedules(GetRunningAtSchedules(tiplocs, date, startTime, endTime, atocCode, powerType), date, atocCode, powerType);

            return GetRunningTrainSchedules(schedules, date);
        }

        private IEnumerable<RunningScheduleTrain> GetCallingBetweenSchedules(IEnumerable<short> tiplocsFrom, IEnumerable<short> tiplocsTo, DateTime date, TimeSpan startTime, TimeSpan endTime, string atocCode, PowerType? powerType)
        {
            var schedules = GetDistinctSchedules(GetRunningCallingBetweenSchedules(tiplocsFrom, tiplocsTo, date, startTime, endTime, atocCode, powerType), date, atocCode, powerType);

            return GetRunningTrainSchedules(schedules, date);
        }

        private IEnumerable<RunningScheduleTrain> GetRunningTrainSchedules(IEnumerable<ScheduleHolder> schedules, DateTime date)
        {
            var runningSchedules = GetSchedules(schedules.Select(s => s.StopsScheduleId ?? s.ScheduleId), date)
                .ToList();

            foreach (var schedule in runningSchedules)
            {
                var actualSchedule = schedules.Where(s => s.StopsScheduleId == schedule.ScheduleId || s.ScheduleId == schedule.ScheduleId).Single();
                schedule.ScheduleId = actualSchedule.ScheduleId;
                schedule.STPIndicatorId = actualSchedule.STPIndicatorId;
            }

            return runningSchedules;
        }

        private static readonly TimeSpan EndOfDay = new TimeSpan(23, 59, 59);

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
                SELECT [LiveTrainStop].[TrainId]
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

        public IEnumerable<TrainMovementResult> TerminatingAtStation(string crsCode, DateTime? startDate, DateTime? endDate, string atocCode = null, PowerType? powerType = null)
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

            return TerminatingAt(tiplocs, startDate.Value, endDate.Value, atocCode, powerType);
        }

        public IEnumerable<TrainMovementResult> TerminatingAtLocation(string stanox, DateTime? startDate, DateTime? endDate, string atocCode = null, PowerType? powerType = null)
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

            return TerminatingAt(tiplocs, startDate.Value, endDate.Value, atocCode, powerType);
        }

        private IEnumerable<TrainMovementResult> TerminatingAt(IEnumerable<short> tiplocs, DateTime startDate, DateTime endDate, string atocCode, PowerType? powerType)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Date != endDate.Date)
            {
                nextDaySchedules = GetTerminatingAtSchedules(tiplocs, endDate.Date, endDate.Date.TimeOfDay, endDate.TimeOfDay, atocCode, powerType);
                endDate = startDate.Date.AddDays(1).AddMinutes(-1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetTerminatingAtSchedules(tiplocs, startDate.Date, startDate.TimeOfDay, endDate.TimeOfDay, atocCode, powerType)
                .Union(nextDaySchedules)
                .ToList();

            // need to get live running data between these dates
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);
            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate, endDate);

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

        public IEnumerable<TrainMovementResult> StartingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, PowerType? powerType = null)
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

            return StartingAt(tiplocs, startDate.Value, endDate.Value, atocCode, powerType);
        }

        public IEnumerable<TrainMovementResult> StartingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, PowerType? powerType = null)
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

            return StartingAt(tiplocs, startDate.Value, endDate.Value, atocCode, powerType);
        }

        private IEnumerable<TrainMovementResult> StartingAt(IEnumerable<short> tiplocs, DateTime startDate, DateTime endDate, string atocCode, PowerType? powerType)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Date != endDate.Date)
            {
                nextDaySchedules = GetStartingAtSchedules(tiplocs, endDate.Date, endDate.Date.TimeOfDay, endDate.TimeOfDay, atocCode, powerType);
                endDate = startDate.Date.AddDays(1).AddMinutes(-1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetStartingAtSchedules(tiplocs, startDate.Date, startDate.TimeOfDay, endDate.TimeOfDay, atocCode, powerType)
                .Union(nextDaySchedules)
                .ToList();

            // need to get live running data between these dates
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);
            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate, endDate);

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

        public IEnumerable<TrainMovementResult> CallingAtLocation(string stanox, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, PowerType? powerType = null)
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

            return CallingAt(tiplocs, startDate.Value, endDate.Value, atocCode, powerType);
        }

        public IEnumerable<TrainMovementResult> CallingAtStation(string crsCode, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, PowerType? powerType = null)
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

            return CallingAt(tiplocs, startDate.Value, endDate.Value, atocCode, powerType);
        }

        private IEnumerable<TrainMovementResult> CallingAt(IEnumerable<short> tiplocs, DateTime startDate, DateTime endDate, string atocCode, PowerType? powerType)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Date != endDate.Date)
            {
                nextDaySchedules = GetCallingAtSchedules(tiplocs, endDate.Date, endDate.Date.TimeOfDay, endDate.TimeOfDay, atocCode, powerType);
                endDate = startDate.Date.AddDays(1).AddMinutes(-1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetCallingAtSchedules(tiplocs, startDate.Date, startDate.TimeOfDay, endDate.TimeOfDay, atocCode, powerType)
                .Union(nextDaySchedules)
                .ToList();

            // need to get live running data between these dates
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);
            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate, endDate);

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
                .ThenBy(s =>
                {
                    if (s.Schedule.Stops == null || !s.Schedule.Stops.Any())
                        return default(TimeSpan?);

                    var tiplocStops = s.Schedule.Stops.Where(stop => tiplocs.Contains(stop.Tiploc.TiplocId));
                    if (!tiplocStops.Any())
                        return default(TimeSpan?);

                    var firstStop = tiplocStops.First();
                    return firstStop.PublicArrival ?? firstStop.Arrival ?? firstStop.Departure ?? firstStop.PublicDeparture ?? firstStop.Pass;
                });
        }

        public IEnumerable<TrainMovementResult> CallingBetweenLocations(string fromStanox, string toStanox, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, PowerType? powerType = null)
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

            return CallingBetween(tiplocsFrom, tiplocsTo, startDate.Value, endDate.Value, atocCode, powerType);
        }

        public IEnumerable<TrainMovementResult> CallingBetweenStations(string fromCrs, string toCrs, DateTime? startDate = null, DateTime? endDate = null, string atocCode = null, PowerType? powerType = null)
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

            return CallingBetween(tiplocsFrom, tiplocsTo, startDate.Value, endDate.Value, atocCode, powerType);
        }

        private IEnumerable<TrainMovementResult> CallingBetween(IEnumerable<short> tiplocsFrom, IEnumerable<short> tiplocsTo, DateTime startDate, DateTime endDate, string atocCode, PowerType? powerType)
        {
            IEnumerable<RunningScheduleTrain> nextDaySchedules = null;
            if (startDate.Date != endDate.Date)
            {
                nextDaySchedules = GetCallingBetweenSchedules(tiplocsFrom, tiplocsTo, endDate.Date, endDate.Date.TimeOfDay, endDate.TimeOfDay, atocCode, powerType);
                endDate = startDate.Date.AddDays(1).AddMinutes(-1);
            }
            else
            {
                nextDaySchedules = Enumerable.Empty<RunningScheduleTrain>();
            }

            var allSchedules = GetCallingBetweenSchedules(tiplocsFrom, tiplocsTo, startDate.Date, startDate.TimeOfDay, endDate.TimeOfDay, atocCode, powerType)
                .Union(nextDaySchedules)
                .ToList();

            // need to get live running data between these dates
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);
            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate, endDate);

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
                .ThenBy(s =>
                {
                    if (s.Schedule.Stops == null || !s.Schedule.Stops.Any())
                        return default(TimeSpan?);

                    var tiplocStops = s.Schedule.Stops.Where(stop => tiplocsFrom.Contains(stop.Tiploc.TiplocId));
                    if (!tiplocStops.Any())
                        return default(TimeSpan?);

                    var firstStop = tiplocStops.First();
                    return firstStop.Departure ?? firstStop.PublicDeparture ?? firstStop.Pass;
                });
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

        public TrainMovementResult GetTrainMovementById(string trainUid, DateTime date)
        {
            // activated schedule may have been deleted by the daily schedule update
            // so include deleted ones
            const string sql = @"
                SELECT [ScheduleId]
                    FROM [ScheduleTrain]
                    WHERE 
                        [TrainUid] = @trainUid
                        AND @date >= [StartDate]
                        AND @date <= [EndDate]
                        AND [Runs{0}] = 1
                    ORDER BY [Deleted], [STPIndicatorId], [PowerTypeId] DESC";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                IEnumerable<Guid> scheduleIds = Query<Guid>(string.Format(sql, date.DayOfWeek), new { trainUid, date });

                // the current schedule will be the first
                Guid scheduleId = scheduleIds.FirstOrDefault();

                if (scheduleId != Guid.Empty)
                {
                    TrainMovementResult result = new TrainMovementResult();
                    result.Schedule = GetSchedules(scheduleIds, date).FirstOrDefault();
                    result.Actual = GetActualSchedule(scheduleIds, date.Date, date.Date.Add(new TimeSpan(23, 59, 59))).FirstOrDefault();
                    if (result.Actual != null)
                    {
                        var actualIds = new[] { result.Actual.Id };
                        result.Cancellations = GetCancellations(actualIds, dbConnection);
                        result.Reinstatements = GetReinstatements(actualIds, dbConnection);
                        result.ChangeOfOrigins = GetChangeOfOrigins(actualIds, dbConnection);
                    }
                    else
                    {
                        result.Cancellations = Enumerable.Empty<ExtendedCancellation>();
                        result.Reinstatements = Enumerable.Empty<Reinstatement>();
                        result.ChangeOfOrigins = Enumerable.Empty<ChangeOfOrigin>();
                    }

                    return result;
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

        public IEnumerable<TrainMovementResult> GetTrainMovementByHeadcode(string headcode, DateTime date)
        {
            const string getSchedulesSql = @"
                SELECT [ScheduleTrain].[ScheduleId]
	                ,[ScheduleTrain].[TrainUid]
	                ,[ScheduleTrain].[STPIndicatorId]
                FROM [ScheduleTrain]
                WHERE [Headcode] = @headcode
	                AND [ScheduleTrain].[Runs{0}] = 1
	                AND @date >= [ScheduleTrain].[StartDate]
	                AND @date <= [ScheduleTrain].[EndDate]";

            var matchingSchedules = Query<ScheduleHolder>(string.Format(getSchedulesSql, date.DayOfWeek), new
            {
                headcode,
                date = date.Date
            });

            var schedules = matchingSchedules
                .GroupBy(s => s.TrainUid)
                .Select(s =>
                {
                    var firstSchedule = s
                        .OrderBy(sub => sub.STPIndicatorId)
                        .First();
                    // if is cancellation then wont have any stops associated, so get previous schedule if applicable
                    if (firstSchedule.STPIndicatorId == STPIndicator.Cancellation)
                    {
                        firstSchedule.StopsScheduleId = s
                            .Except(new[] { firstSchedule })
                            .OrderBy(sub => sub.STPIndicatorId)
                            .Select(sub => sub.ScheduleId)
                            .FirstOrDefault();
                    }
                    return firstSchedule;
                })
                .ToList();

            var allSchedules = GetRunningTrainSchedules(schedules, date);

            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), date.Date, date.Date.AddDays(1));

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

        private sealed class NearestStationResult
        {
            public Guid ScheduleTrain { get; set; }
        }

        public IEnumerable<TrainMovementResult> NearestTrains(double lat, double lon, int limit)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddMinutes(-15);

            var tiplocs = _tiplocRepository.GetByLocation(lat, lon, 5)
                .Select(t => t.TiplocId)
                .ToList();
            if (!tiplocs.Any())
            {
                return Enumerable.Empty<TrainMovementResult>();
            }

            const string sql = @"
                SELECT DISTINCT TOP({0}) 
	                [LiveTrain].[ScheduleTrain], [LiveTrainStop].[ActualTimestamp]
                FROM [LiveTrainStop] 
                INNER JOIN [LiveTrain] ON [LiveTrainStop].[TrainId] = [LiveTrain].[Id]
                WHERE [LiveTrainStop].[ReportingTiplocId] IN @tiplocs
                AND [LiveTrainStop].[ActualTimestamp] BETWEEN @startDate AND @endDate
                AND [LiveTrain].[ScheduleTrain] IS NOT NULL
                ORDER BY [LiveTrainStop].[ActualTimestamp] DESC";

            var schedules = Query<NearestStationResult>(string.Format(sql, limit), new { startDate, endDate, tiplocs });

            var allSchedules = GetSchedules(schedules.Select(s => s.ScheduleTrain).Distinct(), startDate.Date);

            // need to get live running data between these dates
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);
            var allActualData = GetActualSchedule(allSchedules.Select(s => s.ScheduleId).Distinct(), startDate, endDate);

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
                .ThenBy(s =>
                {
                    if (s.Schedule.Stops == null || !s.Schedule.Stops.Any())
                        return default(TimeSpan?);

                    var tiplocStops = s.Schedule.Stops.Where(stop => tiplocs.Contains(stop.Tiploc.TiplocId));
                    if (!tiplocStops.Any())
                        return default(TimeSpan?);

                    var firstStop = tiplocStops.First();
                    return firstStop.PublicArrival ?? firstStop.Arrival ?? firstStop.Departure ?? firstStop.PublicDeparture ?? firstStop.Pass;
                });
        }
    }
}
