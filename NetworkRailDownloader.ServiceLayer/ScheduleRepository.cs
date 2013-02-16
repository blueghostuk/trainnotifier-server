using System;
using System.Collections.Generic;
using System.Linq;
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

        public void InsertSchedule(ScheduleTrain train)
        {
            using (var ts = GetTransactionScope())
            {
                const string sql = @"
                INSERT INTO [natrail].[dbo].[ScheduleTrain]
                       ([TrainUid]
                       ,[StartDate]
                       ,[EndDate]
                       ,[Active]
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
                       ,[DestinationStopTiplocId])
                    OUTPUT [inserted].[ScheduleId]
                    VALUES
                       (@TrainUid
                       ,@StartDate
                       ,@EndDate
                       ,@Active
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
                       ,@DestinationTiplocId)";

                Guid id = ExecuteInsert(sql, new
                {
                    train.TrainUid,
                    train.StartDate,
                    train.EndDate,
                    train.Active,
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
                    DestinationTiplocId = train.Destination != null ? new short?(train.Destination.TiplocId) : default(short?)
                });

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
    }
}
