﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public ScheduleTrain GetTrainByUid(string trainId, string trainUid)
        {
            const string sql = @"
                SELECT 
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
                INNER JOIN [AtocCode]  ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
                LEFT JOIN  [Tiploc] [OriginTiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [OriginTiploc].[TiplocId]
                LEFT JOIN  [Tiploc] [DestTiploc] ON [ScheduleTrain].[DestinationStopTiplocId] = [DestTiploc].[TiplocId]
                WHERE   [LiveTrain].[TrainId] = @trainId
                    AND [LiveTrain].[TrainUId] = @trainUid";

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
                        trainId,
                        trainUid
                    },
                    splitOn: "Code,RunsMonday,TiplocId,TiplocId").FirstOrDefault();

            }

            if (train != null)
            {
                train.Stops = GetStopsById(train.ScheduleId);
            }

            return train;
        }

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
                FROM [ScheduleTrainStop]
                INNER JOIN [Tiploc] ON [ScheduleTrainStop].[TiplocId] = [Tiploc].[TiplocId]
                WHERE [ScheduleId] = @scheduleId";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                return dbConnection.Query<ScheduleStop, TiplocCode, ScheduleStop>(
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
