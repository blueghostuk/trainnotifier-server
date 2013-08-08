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
                       ,[Headcode]
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
                       ,[Source]
                       ,[PowerTypeId]
                       ,[CategoryTypeId]
                       ,[Speed])
                    OUTPUT [inserted].[ScheduleId]
                    VALUES
                       (@TrainUid
                       ,@Headcode
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
                       ,@Source
                       ,@PowerTypeId
                       ,@CategoryTypeId
                       ,@Speed)";

                Guid id = ExecuteInsert(sql, new
                {
                    train.TrainUid,
                    train.Headcode,
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
                    Source = source,
                    PowerTypeId = train.PowerType != null ? (byte)train.PowerType : default(byte?),
                    CategoryTypeId = train.TrainCategory != null ? (byte)train.TrainCategory : default(byte?),
                    Speed = train.Speed != null ? (byte)train.Speed : default(byte?)
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

        
    }
}
