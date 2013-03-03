using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using System.Linq;

namespace TrainNotifier.Service
{
    public class TrainMovementRepository : DbRepository
    {
        public IEnumerable<OriginTrainMovement> StartingAt(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.AddDays(1);

            const string sql = @"
                SELECT
                     [LiveTrain].[TrainId] AS Id
                    ,[LiveTrain].[Headcode] AS HeadCode
                    ,[LiveTrain].[CreationTimestamp] AS Activated
                    ,[LiveTrain].[OriginDepartTimestamp] AS SchedOriginDeparture
                    ,[LiveTrain].[TrainServiceCode] AS ServiceCode
                    ,[LiveTrain].[Toc] AS TocId
                    ,[LiveTrain].[TrainUid] AS TrainUid
                    ,[LiveTrain].[OriginStanox] AS SchedOriginStanox
                    ,[LiveTrain].[SchedWttId] AS WorkingTTId
                    ,[LiveTrain].[ScheduleTrain] AS ScheduleId
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
                WHERE   [LiveTrain].[OriginStanox] = @stanox 
                    AND [LiveTrain].[OriginDepartTimestamp] >= @startDate
                    AND [LiveTrain].[OriginDepartTimestamp] < @endDate
                ORDER BY [LiveTrain].[OriginDepartTimestamp]";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                // should be SingleOrDefault - but need to work around db bugs for now
                return dbConnection.Query<OriginTrainMovement, AtocCode, ScheduleTiploc, ScheduleTiploc, OriginTrainMovement>(
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
                    splitOn: "Code,TiplocId,TiplocId");

            }
        }

        public IEnumerable<CallingAtTrainMovement> CallingAt(string stanox, DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddDays(-1);
            endDate = endDate ?? DateTime.UtcNow.AddDays(1);

            const string sql = @"
                SELECT
		            [LiveTrain].[TrainId] AS Id
                    ,[LiveTrain].[Headcode] AS HeadCode
                    ,[LiveTrain].[CreationTimestamp] AS Activated
                    ,[LiveTrain].[OriginDepartTimestamp] AS SchedOriginDeparture
                    ,[LiveTrain].[TrainServiceCode] AS ServiceCode
                    ,[LiveTrain].[Toc] AS TocId
                    ,[LiveTrain].[TrainUid] AS TrainUid
                    ,[LiveTrain].[OriginStanox] AS SchedOriginStanox
                    ,[LiveTrain].[SchedWttId] AS WorkingTTId
                    ,[LiveTrain].[ScheduleTrain] AS ScheduleId
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
                WHERE    [Tiploc].[Stanox] = @stanox 
                     AND [LiveTrain].[OriginDepartTimestamp] >= @startDate
                     AND [LiveTrain].[OriginDepartTimestamp] < @endDate";
                // ORDER BY [ScheduleTrainStop].[Arrival], [ScheduleTrainStop].[Departure], [ScheduleTrainStop].[Pass], [OriginTiploc].[Description]";

            using (DbConnection dbConnection = CreateAndOpenConnection())
            {
                // should be SingleOrDefault - but need to work around db bugs for now
                return dbConnection.Query<CallingAtTrainMovement, AtocCode, ScheduleTiploc, ScheduleTiploc, CallingAtTrainMovement>(
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
                    splitOn: "Code,TiplocId,TiplocId")
                    .ToList()
                    .OrderBy(t => t.Origin.Arrival ?? t.Destination.Departure ?? t.Origin.PublicArrival ?? t.Destination.PublicDeparture ?? t.Pass);
            }
        }
    }
}
