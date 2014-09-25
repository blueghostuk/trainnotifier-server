using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using TrainNotifier.Common.Model.Api;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class CancellationRepository : DbRepository
    {
        private static readonly TiplocRepository _tiplocRepository = new TiplocRepository();

        public async Task<IEnumerable<Cancellation>> GetCancellations(string fromCrs, string toCrs, DateTime startDate, DateTime endDate)
        {
            // get tiploc id to improve query
            var tiplocsFrom = _tiplocRepository.GetAllByCRSCode(fromCrs)
                .Select(t => t.TiplocId)
                .ToList();
            var tiplocsTo = _tiplocRepository.GetAllByCRSCode(toCrs)
                .Select(t => t.TiplocId)
                .ToList();

            if (!tiplocsFrom.Any() || !tiplocsTo.Any())
                return Enumerable.Empty<Cancellation>();

            return await GetCancellations(tiplocsFrom, tiplocsTo, startDate, endDate);
        }

        private async Task<IEnumerable<Cancellation>> GetCancellations(List<short> tiplocsFrom, List<short> tiplocsTo, DateTime startDate, DateTime endDate)
        {
            TimeSpan startTime = startDate.TimeOfDay;
            TimeSpan endTime = endDate.TimeOfDay;

            const string sql = @"
                SELECT
                    [ScheduleTrain].[TrainUid] AS [Uid],
                    [LiveTrain].[Headcode],
                    [OriginTiploc].[Stanox] AS [OriginStanox],
                    [DestinationTiploc].[Stanox] AS [DestStanox],
                    [FromS].[PublicDeparture] AS [FromExpected],
                    [ToS].[PublicArrival] AS [ToExpected],
                    [AtocCode].[AtocCode] AS [Code],
                    [AtocCode].[Name]
                FROM [LiveTrainCancellation]
                INNER JOIN [LiveTrain] ON [LiveTrainCancellation].[TrainId] = [LiveTrain].[Id]
                INNER JOIN [ScheduleTrain] ON [LiveTrain].[ScheduleTrain] = [ScheduleTrain].[ScheduleId]
                INNER JOIN [ScheduleTrainStop] [FromS]  ON [ScheduleTrain].[ScheduleId] = [FromS].[ScheduleId]
                INNER JOIN [ScheduleTrainStop] [ToS]  ON [ScheduleTrain].[ScheduleId] = [ToS].[ScheduleId]
                INNER JOIN [Tiploc] [OriginTiploc] ON [ScheduleTrain].[OriginStopTiplocId] = [OriginTiploc].[TiplocId]
                INNER JOIN [Tiploc] [DestinationTiploc] ON [ScheduleTrain].[DestinationStopTiplocId] = [DestinationTiploc].[TiplocId]
                LEFT JOIN [AtocCode] ON [ScheduleTrain].[AtocCode] = [AtocCode].[AtocCode]
                WHERE
                    [FromS].[TiplocId] IN @tiplocsFrom
                    AND 
                    [FromS].[PublicDeparture] IS NOT NULL
                    AND	
                    [ToS].[TiplocId] IN @tiplocsTo
                    AND 
                    [ToS].[PublicArrival] IS NOT NULL
                    AND
                    [FromS].[StopNumber] < [ToS].[StopNumber]
                    AND
                    [LiveTrain].[OriginDepartTimeStamp] >= @startDate
                    AND
                    [LiveTrain].[OriginDepartTimeStamp] < @endDate
                    AND
                    [ScheduleTrain].[Runs{0}] = 1
                    AND
                    [FromS].[PublicDeparture] >= @startTime
                    AND
                    [ToS].[PublicArrival] <= @endTime
                    AND
                    [ScheduleTrain].[CategoryTypeId] IN (1,2,3,10,11,12)
                    AND 
                    [ScheduleTrain].[STPIndicatorId] IN (2,3,4)
                ORDER BY [FromS].[PublicDeparture]";

            using (DbConnection connection = CreateAndOpenConnection())
            {
                return await connection.QueryAsync<Cancellation,  AtocCode, Cancellation>(
                    string.Format(sql, startDate.DayOfWeek.ToString()),
                    (c, a) =>
                    {
                        c.Operator = a;
                        return c;
                    },
                    new { tiplocsFrom, tiplocsTo, startDate, endDate, startTime, endTime },
                    splitOn: "Code");
            }
        }
    }
}
