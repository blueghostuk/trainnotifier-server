﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class TrainMovementRepository : DbRepository
    {
        private readonly TiplocRepository _tiplocRepository = new TiplocRepository();

        public IEnumerable<OriginTrainMovement> StartingAt(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.AddDays(1);

            const string sql = @"
                SELECT
                    [LiveTrain].[Id] AS [UniqueId]
                    ,[LiveTrain].[TrainId] AS Id
                    ,[LiveTrain].[Headcode] AS HeadCode
                    ,[LiveTrain].[CreationTimestamp] AS Activated
                    ,[LiveTrain].[OriginDepartTimestamp] AS SchedOriginDeparture
                    ,[LiveTrain].[TrainServiceCode] AS ServiceCode
                    ,[LiveTrain].[Toc] AS TocId
                    ,[LiveTrain].[TrainUid] AS TrainUid
                    ,[LiveTrain].[OriginStanox] AS SchedOriginStanox
                    ,[LiveTrain].[SchedWttId] AS WorkingTTId
                    ,[LiveTrain].[ScheduleTrain] AS ScheduleId
                    ,[ActualDeparture].[ActualTimestamp] AS [ActualDeparture]
                    ,[ActualArrival].[ActualTimestamp] AS [ActualArrival]
                    ,[AtocCode].[AtocCode] AS [Code]
                    ,[AtocCode].[Name]
                    ,[OriginTiploc].[TiplocId]
                    ,[OriginTiploc].[Tiploc]
                    ,[OriginTiploc].[Nalco]
                    ,[OriginTiploc].[Description]
                    ,[OriginTiploc].[Stanox]
                    ,[OriginTiploc].[CRS]
                    ,[OriginStop].[Platform]
                    ,[OriginStop].[Departure]
                    ,[OriginStop].[PublicDeparture]
                    ,[DestTiploc].[TiplocId]
                    ,[DestTiploc].[Tiploc]
                    ,[DestTiploc].[Nalco]
                    ,[DestTiploc].[Description]
                    ,[DestTiploc].[Stanox]
                    ,[DestTiploc].[CRS]
                    ,[DestinationStop].[Platform]
                    ,[DestinationStop].[Arrival]
                    ,[DestinationStop].[PublicArrival]
                FROM [LiveTrain]
                LEFT JOIN [ScheduleTrain] ON [LiveTrain].[ScheduleTrain] = [ScheduleTrain].[ScheduleId]
                LEFT JOIN [AtocCode] ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
                LEFT JOIN  [Tiploc] [OriginTiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [OriginTiploc].[TiplocId]
                LEFT JOIN  [Tiploc] [DestTiploc] ON [ScheduleTrain].[DestinationStopTiplocId] = [DestTiploc].[TiplocId]
                LEFT JOIN [ScheduleTrainStop] [OriginStop] ON [ScheduleTrain].[ScheduleId] = [OriginStop].[ScheduleId]
                    AND [OriginStop].[Origin] = 1
                LEFT JOIN [ScheduleTrainStop] [DestinationStop] ON [ScheduleTrain].[ScheduleId] = [DestinationStop].[ScheduleId]
                    AND [DestinationStop].[Terminate] = 1
                LEFT JOIN [LiveTrainStop] [ActualDeparture] ON [LiveTrain].[Id] = [ActualDeparture].[TrainId] 
					AND [ActualDeparture].[ScheduleStopNumber] = [OriginStop].[StopNumber]
					AND [ActualDeparture].[EventType] = 'DEPARTURE'
                LEFT JOIN [LiveTrainStop] [ActualArrival] ON [LiveTrain].[Id] = [ActualArrival].[TrainId] 
					AND [ActualArrival].[ScheduleStopNumber] = [DestinationStop].[StopNumber]
					AND [ActualArrival].[EventType] = 'ARRIVAL'
                WHERE   [LiveTrain].[OriginStanox] = @stanox 
                    AND [LiveTrain].[OriginDepartTimestamp] >= @startDate
                    AND [LiveTrain].[OriginDepartTimestamp] < @endDate
                ORDER BY [LiveTrain].[OriginDepartTimestamp]";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                List<OriginTrainMovement> trains
                    = dbConnection.Query<OriginTrainMovement, AtocCode, ScheduleTiploc, ScheduleTiploc, OriginTrainMovement>(
                        sql,
                        (tm, ac, ot, dt) =>
                        {
                            tm.AtocCode = ac;
                            tm.Origin = ot;
                            tm.Destination = dt;
                            return tm;
                        },
                        new
                        {
                            stanox,
                            startDate,
                            endDate
                        },
                        splitOn: "Code,TiplocId,TiplocId").ToList();

                if (trains.Any())
                {
                    var trainIds = trains
                        .Select(t => t.UniqueId)
                        .Distinct()
                        .ToArray();

                    IEnumerable<ExtendedCancellation> cancellations = GetCancellations(trainIds, dbConnection);
                    foreach (var cancellation in cancellations)
                    {
                        OriginTrainMovement train = trains.FirstOrDefault(t => t.UniqueId == cancellation.TrainId);
                        train.Cancellation = cancellation;
                    }

                    IEnumerable<Reinstatement> reinstatements = GetReinstatements(trainIds, dbConnection);
                    foreach (var reinstatement in reinstatements)
                    {
                        OriginTrainMovement train = trains.FirstOrDefault(t => t.UniqueId == reinstatement.TrainId);
                        train.Reinstatement = reinstatement;
                    }

                    IEnumerable<ChangeOfOrigin> changeOfOrigins = GetChangeOfOrigins(trainIds, dbConnection);
                    foreach (var changeOfOrigin in changeOfOrigins)
                    {
                        OriginTrainMovement train = trains.FirstOrDefault(t => t.UniqueId == changeOfOrigin.TrainId);
                        train.ChangeOfOrigin = changeOfOrigin;
                    }
                }

                return trains;
            }
        }

        public IEnumerable<CallingAtTrainMovement> CallingAt(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.AddDays(1);

            // get tiploc id to improve query
            StationTiploc tiploc = _tiplocRepository.GetByStanox(stanox);

            if (tiploc == null)
                return Enumerable.Empty<CallingAtTrainMovement>();

            const string sql = @"
                SELECT
                    [LiveTrain].[Id] AS [UniqueId]
		            ,[LiveTrain].[TrainId] AS Id
                    ,[LiveTrain].[Headcode] AS HeadCode
                    ,[LiveTrain].[CreationTimestamp] AS Activated
                    ,[LiveTrain].[OriginDepartTimestamp] AS SchedOriginDeparture
                    ,[LiveTrain].[TrainServiceCode] AS ServiceCode
                    ,[LiveTrain].[Toc] AS TocId
                    ,[LiveTrain].[TrainUid] AS TrainUid
                    ,[LiveTrain].[OriginStanox] AS SchedOriginStanox
                    ,[LiveTrain].[SchedWttId] AS WorkingTTId
                    ,[LiveTrain].[ScheduleTrain] AS ScheduleId
                    ,[ActualDeparture].[ActualTimestamp] AS [ActualDeparture]
                    ,[ActualArrival].[ActualTimestamp] AS [ActualArrival]
					,[ScheduleTrainStop].[Pass]
                    ,[AtocCode].[AtocCode] AS [Code]
                    ,[AtocCode].[Name]
                    ,[OriginTiploc].[TiplocId]
                    ,[OriginTiploc].[Tiploc]
                    ,[OriginTiploc].[Nalco]
                    ,[OriginTiploc].[Description]
                    ,[OriginTiploc].[Stanox]
                    ,[OriginTiploc].[CRS]
					,[ScheduleTrainStop].[Platform]
					,[ScheduleTrainStop].[Arrival]
					,[ScheduleTrainStop].[PublicArrival]
                    ,[DestTiploc].[TiplocId]
                    ,[DestTiploc].[Tiploc]
                    ,[DestTiploc].[Nalco]
                    ,[DestTiploc].[Description]
                    ,[DestTiploc].[Stanox]
                    ,[DestTiploc].[CRS]
					,[ScheduleTrainStop].[Platform]
					,[ScheduleTrainStop].[Departure]
					,[ScheduleTrainStop].[PublicDeparture]
                FROM [ScheduleTrainStop]
                INNER JOIN [Tiploc] ON [ScheduleTrainStop].[TiplocId] = [Tiploc].[TiplocId]
                INNER JOIN [ScheduleTrain] ON [ScheduleTrainStop].[ScheduleId] = [ScheduleTrain].[ScheduleId]
                INNER JOIN [LiveTrain] ON [ScheduleTrain].[ScheduleId] = [LiveTrain].[ScheduleTrain]
                INNER JOIN [AtocCode] ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
                INNER JOIN  [Tiploc] [OriginTiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [OriginTiploc].[TiplocId]
                INNER JOIN  [Tiploc] [DestTiploc] ON [ScheduleTrain].[DestinationStopTiplocId] = [DestTiploc].[TiplocId]
                LEFT JOIN [LiveTrainStop] [ActualDeparture] ON [LiveTrain].[Id] = [ActualDeparture].[TrainId] 
					AND [ActualDeparture].[ScheduleStopNumber] = [ScheduleTrainStop].[StopNumber]
					AND [ActualDeparture].[EventType] = 'DEPARTURE'
                LEFT JOIN [LiveTrainStop] [ActualArrival] ON [LiveTrain].[Id] = [ActualArrival].[TrainId] 
					AND [ActualArrival].[ScheduleStopNumber] = [ScheduleTrainStop].[StopNumber]
					AND [ActualArrival].[EventType] = 'ARRIVAL'
                WHERE    [Tiploc].[TiplocId] = @tiplocId 
                     AND [LiveTrain].[OriginDepartTimestamp] >= @startDate
                     AND [LiveTrain].[OriginDepartTimestamp] < @endDate";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                List<CallingAtTrainMovement> trains
                    = dbConnection.Query<CallingAtTrainMovement, AtocCode, ScheduleTiploc, ScheduleTiploc, CallingAtTrainMovement>(
                    sql,
                    (tm, ac, ot, dt) =>
                    {
                        tm.AtocCode = ac;
                        tm.Origin = ot;
                        tm.Destination = dt;
                        return tm;
                    },
                    new
                    {
                        tiplocId = tiploc.TiplocId,
                        startDate,
                        endDate
                    },
                    splitOn: "Code,TiplocId,TiplocId").ToList();

                if (trains.Any())
                {
                    var trainIds = trains
                        .Select(t => t.UniqueId)
                        .Distinct()
                        .ToArray();

                    IEnumerable<ExtendedCancellation> cancellations = GetCancellations(trainIds, dbConnection);
                    foreach (var cancellation in cancellations)
                    {
                        OriginTrainMovement train = trains.FirstOrDefault(t => t.UniqueId == cancellation.TrainId);
                        train.Cancellation = cancellation;
                    }

                    IEnumerable<Reinstatement> reinstatements = GetReinstatements(trainIds, dbConnection);
                    foreach (var reinstatement in reinstatements)
                    {
                        OriginTrainMovement train = trains.FirstOrDefault(t => t.UniqueId == reinstatement.TrainId);
                        train.Reinstatement = reinstatement;
                    }

                    IEnumerable<ChangeOfOrigin> changeOfOrigins = GetChangeOfOrigins(trainIds, dbConnection);
                    foreach (var changeOfOrigin in changeOfOrigins)
                    {
                        OriginTrainMovement train = trains.FirstOrDefault(t => t.UniqueId == changeOfOrigin.TrainId);
                        train.ChangeOfOrigin = changeOfOrigin;
                    }
                }

                return trains
                    .OrderBy(t => t.Origin.Arrival ?? t.Destination.Departure ?? t.Origin.PublicArrival ?? t.Destination.PublicDeparture ?? t.Pass);
            }
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
                FROM [LiveTrainCancellation]
                LEFT JOIN [DelayAttributionCodes] ON [LiveTrainCancellation].[ReasonCode] = [DelayAttributionCodes].[ReasonCode]
                LEFT JOIN [Tiploc] AS [CancelTiploc] ON [LiveTrainCancellation].[Stanox] = [CancelTiploc].[Stanox]
                WHERE [LiveTrainCancellation].[TrainId] IN @trainIds";

            return connection.Query<ExtendedCancellation, TiplocCode, ExtendedCancellation>(sql,
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
                FROM [LiveTrainReinstatement]
                INNER JOIN [Tiploc] AS [ReinstatementTiploc] ON [LiveTrainReinstatement].[ReinstatedTiplocId] = [ReinstatementTiploc].[TiplocId]
                WHERE [LiveTrainReinstatement].[TrainId] IN @trainIds";

            return connection.Query<Reinstatement, TiplocCode, Reinstatement>(sql,
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
                FROM [LiveTrainChangeOfOrigin]
                INNER JOIN [Tiploc] AS [ChangeOfOriginTiploc] ON [LiveTrainChangeOfOrigin].[NewTiplocId] = [ChangeOfOriginTiploc].[TiplocId]
                LEFT JOIN [DelayAttributionCodes] [ChangeOfOriginDelay] ON [LiveTrainChangeOfOrigin].[ReasonCode] = [ChangeOfOriginDelay].[ReasonCode]
                WHERE [LiveTrainChangeOfOrigin].[TrainId] IN @trainIds";

            return connection.Query<ChangeOfOrigin, TiplocCode, ChangeOfOrigin>(sql,
                (o, t) =>
                {
                    o.NewOrigin = t;
                    return o;
                }, new { trainIds }, splitOn: "TiplocId");
        }
    }
}
