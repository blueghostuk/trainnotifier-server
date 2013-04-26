using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class ScheduleRepository : DbRepository
    {
        public AtocCode GetAtocCode(string code)
        {
            const string sql = "SELECT [AtocCode] AS [Code], [Name] FROM [AtocCode] WHERE [AtocCode] = @code";

            return ExecuteScalar<AtocCode>(sql, new { code });
        }

        public void InsertAtocCode(string code, string name = null)
        {
            const string sql = "INSERT INTO [AtocCode]([AtocCode],[Name]) VALUES (@code, @name)";

            ExecuteNonQuery(sql, new { code, name });
        }

        public void InsertSchedule(ScheduleTrain train, ScheduleSource source = ScheduleSource.CIF)
        {
            using (var ts = GetTransactionScope())
            {
                const string sql = @"
                INSERT INTO [natrail].[dbo].[ScheduleTrain]
                       ([TrainUid]
                       ,[StartDate]
                       ,[EndDate]
                       ,[AtocCode]
                       ,[RunsMonday]
                       ,[RunsTuesday]
                       ,[RunsWednesday]
                       ,[RunsThursday]
                       ,[RunsFriday]
                       ,[RunsSaturday]
                       ,[RunsSunday]
                       ,[RunsBankHoliday]
                       ,[STPIndicatorId]
                       ,[ScheduleStatusId]
                       ,[OriginStopTiplocId]
                       ,[DestinationStopTiplocId]
                       ,[Source])
                    OUTPUT [inserted].[ScheduleId]
                    VALUES
                       (@TrainUid
                       ,@StartDate
                       ,@EndDate
                       ,@Code
                       ,@Monday
                       ,@Tuesday
                       ,@Wednesday
                       ,@Thursday
                       ,@Friday
                       ,@Saturday
                       ,@Sunday
                       ,@BankHoliday
                       ,@STPIndicator
                       ,@Status
                       ,@OriginTiplocId
                       ,@DestinationTiplocId
                       ,@Source)";

                Guid id = ExecuteInsert(sql, new
                {
                    train.TrainUid,
                    train.StartDate,
                    train.EndDate,
                    train.AtocCode.Code,
                    train.Schedule.Monday,
                    train.Schedule.Tuesday,
                    train.Schedule.Wednesday,
                    train.Schedule.Thursday,
                    train.Schedule.Friday,
                    train.Schedule.Saturday,
                    train.Schedule.Sunday,
                    train.Schedule.BankHoliday,
                    train.STPIndicator,
                    train.Status,
                    OriginTiplocId = train.Origin != null ? new short?(train.Origin.TiplocId) : default(short?),
                    DestinationTiplocId = train.Destination != null ? new short?(train.Destination.TiplocId) : default(short?),
                    Source = source
                });
                if (source == ScheduleSource.VSTP)
                {
                    Trace.TraceInformation("Saving VSTP Schedule for UID: {0} with ID {1}",
                        train.TrainUid, id);
                }

                InsertScheduleStops(id, train.Stops);
                ts.Complete();
            }
        }

        public void InsertScheduleStops(Guid scheduleId, IEnumerable<ScheduleStop> stops)
        {
            if (stops == null || !stops.Any())
                return;

            const string sql = @"
                INSERT INTO [natrail].[dbo].[ScheduleTrainStop]
                           ([ScheduleId]
                           ,[StopNumber]
                           ,[TiplocId]
                           ,[Arrival]
                           ,[Departure]
                           ,[Pass]
                           ,[PublicArrival]
                           ,[PublicDeparture]
                           ,[Line]
                           ,[Path]
                           ,[Platform]
                           ,[EngineeringAllowance]
                           ,[PathingAllowance]
                           ,[PerformanceAllowance]
                           ,[Origin]
                           ,[Intermediate]
                           ,[Terminate])
                     VALUES
                           (@scheduleId
                           ,@StopNumber
                           ,@TiplocId
                           ,@Arrival
                           ,@Departure
                           ,@Pass
                           ,@PublicArrival
                           ,@PublicDeparture
                           ,@Line
                           ,@Path
                           ,@Platform
                           ,@EngineeringAllowance
                           ,@PathingAllowance
                           ,@PerformanceAllowance
                           ,@Origin
                           ,@Intermediate
                           ,@Terminate)";
            foreach (ScheduleStop stop in stops)
            {
                ExecuteNonQuery(sql, new
                {
                    scheduleId,
                    stop.StopNumber,
                    stop.Tiploc.TiplocId,
                    stop.Arrival,
                    stop.Departure,
                    stop.Pass,
                    stop.PublicArrival,
                    stop.PublicDeparture,
                    stop.Line,
                    stop.Path,
                    stop.Platform,
                    stop.EngineeringAllowance,
                    stop.PathingAllowance,
                    stop.PerformanceAllowance,
                    stop.Origin,
                    stop.Intermediate,
                    stop.Terminate
                });
            }
        }

        public void DeleteSchedule(ScheduleTrain train)
        {
            const string sql = @"
                UPDATE [natrail].[dbo].[ScheduleTrain]
                SET [Deleted] = 1
                WHERE [TrainUid] = @TrainUid
                    AND [StartDate] = @StartDate
                    AND [STPIndicatorId] = @STPIndicator";

            ExecuteNonQuery(sql, new
            {
                train.TrainUid,
                train.StartDate,
                train.STPIndicator
            });
        }

        public ScheduleTrain GetTrainByUid(string trainUid, DateTime date)
        {
            const string sql = @"
                SELECT TOP 1
                     [ScheduleTrain].[ScheduleId]
                    ,[ScheduleTrain].[TrainUid]
                    ,[ScheduleTrain].[StartDate]
                    ,[ScheduleTrain].[EndDate]
                    ,[ScheduleTrain].[ScheduleStatusId] AS [Status]
                    ,[ScheduleTrain].[STPIndicatorId] AS [STPIndicator]
                    ,[AtocCode].[AtocCode] AS [Code]
                    ,[AtocCode].[Name]
                    ,[ScheduleTrain].[RunsMonday]
                    ,[ScheduleTrain].[RunsTuesday]
                    ,[ScheduleTrain].[RunsWednesday]
                    ,[ScheduleTrain].[RunsThursday]
                    ,[ScheduleTrain].[RunsFriday]
                    ,[ScheduleTrain].[RunsSaturday]
                    ,[ScheduleTrain].[RunsSunday]
                    ,[ScheduleTrain].[RunsBankHoliday]
                    ,[OriginTiploc].[TiplocId]
                    ,[OriginTiploc].[Tiploc]
                    ,[OriginTiploc].[Nalco]
                    ,[OriginTiploc].[Description]
                    ,[OriginTiploc].[Stanox]
                    ,[OriginTiploc].[CRS]
                    ,[DestTiploc].[TiplocId]
                    ,[DestTiploc].[Tiploc]
                    ,[DestTiploc].[Nalco]
                    ,[DestTiploc].[Description]
                    ,[DestTiploc].[Stanox]
                    ,[DestTiploc].[CRS]
                FROM [ScheduleTrain]
                INNER JOIN [LiveTrain] ON [ScheduleTrain].[ScheduleId] = [LiveTrain].[ScheduleTrain]
                LEFT JOIN [AtocCode]  ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
                LEFT JOIN  [Tiploc] [OriginTiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [OriginTiploc].[TiplocId]
                LEFT JOIN  [Tiploc] [DestTiploc] ON [ScheduleTrain].[DestinationStopTiplocId] = [DestTiploc].[TiplocId]
                WHERE [ScheduleTrain].[TrainUid] = @trainUid AND [LiveTrain].[OriginDepartTimestamp] >= @date
                ORDER BY [LiveTrain].[OriginDepartTimestamp] ASC";

            ScheduleTrain train = null;

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                // should be SingleOrDefault - but need to work around db bugs for now
                train = dbConnection.Query<ScheduleTrain, AtocCode, dynamic, TiplocCode, TiplocCode, ScheduleTrain>(
                    sql,
                    (st, ac, d, ot, dt) =>
                    {
                        st.AtocCode = ac;
                        st.Origin = ot;
                        st.Destination = dt;
                        st.Schedule = new Schedule
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
                        return st;
                    },
                    new
                    {
                        trainUid,
                        date
                    },
                    splitOn: "Code,RunsMonday,TiplocId,TiplocId").FirstOrDefault();

            }

            if (train != null)
            {
                train.Stops = GetStopsById(train.ScheduleId);
            }

            return train;
        }

        /// <summary>
        /// Get stops for a schedule
        /// </summary>
        /// <param name="scheduleId">schedule id</param>
        /// <returns>stops for schedule</returns>
        public IEnumerable<ScheduleStop> GetStopsById(Guid scheduleId)
        {
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
                WHERE [ScheduleId] = @scheduleId
                ORDER BY [ScheduleTrainStop].[StopNumber]";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                return dbConnection.Query<ScheduleStop, StationTiploc, ScheduleStop>(
                    sql,
                    (st, t) =>
                    {
                        st.Tiploc = t;
                        return st;
                    },
                    new { scheduleId },
                    splitOn: "TiplocId");
            }
        }

        public IEnumerable<ScheduleViewModel> GetForDate(string stanox, DateTime date)
        {
            TiplocRepository tr = new TiplocRepository();
            var tiplocs = tr.GetByStanoxs(stanox);
            if (tiplocs == null || !tiplocs.Any())
            {
                return Enumerable.Empty<ScheduleViewModel>();
            }
            const string sql = @"
                SELECT 
	                [ScheduleId]
	                ,[TrainUid]
	                ,[ScheduleStatusId]
	                ,[STPIndicatorId]
	                ,[Departure]
	                ,[PublicDeparture]
	                ,[AtocCode] AS [Code]
	                ,[Name]
	                ,[OriginTiplocId] AS [TiplocId]
	                ,[OriginTiploc] AS [Tiploc]
	                ,[OriginNalco] AS [Nalco]
	                ,[OriginDescription] AS [Description]
	                ,[OriginStanox] AS [Stanox]
	                ,[OriginCRS] AS [CRS]
	                ,[DestTiplocId] AS [TiplocId]
	                ,[DestTiploc] AS [Tiploc]
	                ,[DestNalco] AS [Nalco]
	                ,[DestDescription] AS [Description]
	                ,[DestStanox] AS [Stanox]
                 FROM( 
	                SELECT
		                 [ScheduleTrain].[ScheduleId]
		                ,[ScheduleTrain].[TrainUid]
		                ,[ScheduleTrain].[ScheduleStatusId]
		                ,[ScheduleTrain].[STPIndicatorId]
		                ,[FirstStop].[Departure]
		                ,[FirstStop].[PublicDeparture]
		                ,[AtocCode].[AtocCode]
		                ,[AtocCode].[Name]
		                ,[OriginTiploc].[TiplocId] AS [OriginTiplocId]
		                ,[OriginTiploc].[Tiploc] AS [OriginTiploc]
		                ,[OriginTiploc].[Nalco] AS [OriginNalco]
		                ,[OriginTiploc].[Description] AS [OriginDescription]
		                ,[OriginTiploc].[Stanox] AS [OriginStanox]
		                ,[OriginTiploc].[CRS] AS [OriginCRS]
		                ,[DestTiploc].[TiplocId] AS [DestTiplocId]
		                ,[DestTiploc].[Tiploc] AS [DestTiploc]
		                ,[DestTiploc].[Nalco] AS [DestNalco]
		                ,[DestTiploc].[Description] AS [DestDescription]
		                ,[DestTiploc].[Stanox] AS [DestStanox]
		                ,[DestTiploc].[CRS] AS [DestCRS]
		                ,ROW_NUMBER() OVER (PARTITION BY [ScheduleTrain].[TrainUid] ORDER BY [ScheduleTrain].[STPIndicatorId]) AS [RowNumber]
	                FROM [ScheduleTrain]
	                INNER JOIN [ScheduleTrainStop] ON [ScheduleTrain].[ScheduleId] = [ScheduleTrainStop].[ScheduleId]
	                INNER JOIN [ScheduleTrainStop] AS [FirstStop] ON [ScheduleTrain].[ScheduleId] = [FirstStop].[ScheduleId]
		                AND [FirstStop].[StopNumber] = 0
	                LEFT JOIN [AtocCode]  ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
	                LEFT JOIN  [Tiploc] [OriginTiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [OriginTiploc].[TiplocId]
	                LEFT JOIN  [Tiploc] [DestTiploc] ON [ScheduleTrain].[DestinationStopTiplocId] = [DestTiploc].[TiplocId]
	                WHERE [ScheduleTrain].[Runs{0}] = 1 
		                AND [ScheduleTrainStop].[TiplocId] IN @tiplocIds
		                AND @date >= [StartDate]
		                AND @date <= [EndDate]) AS [Results]
                WHERE   [Results].[RowNumber] = 1
                ORDER BY [Departure], [PublicDeparture]";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                return dbConnection.Query<ScheduleViewModel, AtocCode, TiplocCode, TiplocCode, ScheduleViewModel>(
                    string.Format(sql, date.DayOfWeek.ToString()),
                    (st, ac, ot, dt) =>
                    {
                        st.AtocCode = ac;
                        st.Origin = ot;
                        st.Destination = dt;
                        return st;
                    },
                    new
                    {
                        tiplocIds = tiplocs.Select(t => t.TiplocId),
                        date
                    },
                    splitOn: "Code,TiplocId,TiplocId");

            }
        }
    }
}
