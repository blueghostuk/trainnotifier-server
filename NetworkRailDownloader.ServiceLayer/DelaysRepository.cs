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
    public class DelaysRepository : DbRepository
    {
        private static readonly TiplocRepository _tiplocRepository = new TiplocRepository();

        public async Task<IEnumerable<Delay>> GetDelays(string fromCrs, string toCrs, DateTime startDate, DateTime endDate)
        {
            // get tiploc id to improve query
            var tiplocsFrom = _tiplocRepository.GetAllByCRSCode(fromCrs)
                .Select(t => t.TiplocId)
                .ToList();
            var tiplocsTo = _tiplocRepository.GetAllByCRSCode(toCrs)
                .Select(t => t.TiplocId)
                .ToList();

            if (!tiplocsFrom.Any() || !tiplocsTo.Any())
                return Enumerable.Empty<Delay>();

            return await GetDelays(tiplocsFrom, tiplocsTo, startDate, endDate);
        }

        private async Task<IEnumerable<Delay>> GetDelays(IEnumerable<short> tiplocsFrom, IEnumerable<short> tiplocsTo, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT
					 [LiveTrain].[ScheduleTrainUid] AS [Uid]
                    ,[LiveTrain].[Headcode]
					,[OriginTiploc].[Stanox] AS [OriginStanox]
					,[DestTiploc].[Stanox] AS [DestStanox]
					,DATEDIFF(mi, [ToStop].[PlannedTimestamp], [ToStop].[ActualTimestamp]) AS [DelayTime]
					,[FromStop].[PlannedTimestamp] AS [Expected]
					,[FromStop].[ActualTimestamp] AS [Actual]
					,[FromStop].[Platform]
					,[ToStop].[PlannedTimestamp] AS [Expected]
					,[ToStop].[ActualTimestamp] AS [Actual]
					,[ToStop].[Platform]
					,[AtocCode].[AtocCode] AS [Code]
					,[AtocCode].[Name]
				FROM [LiveTrain]
			    INNER JOIN [ScheduleTrain]				ON [ScheduleTrain].[ScheduleId] = [LiveTrain].[ScheduleTrain]
				INNER JOIN [LiveTrainStop] [FromStop]	ON [FromStop].[TrainId] = [LiveTrain].[Id]
				INNER JOIN [LiveTrainStop] [ToStop]		ON [ToStop].[TrainId] = [LiveTrain].[Id]
				INNER JOIN [Tiploc] [OriginTiploc]		ON [OriginTiploc].[TiplocId] = [ScheduleTrain].[OriginStopTiplocId]
				INNER JOIN [Tiploc] [DestTiploc]		ON [DestTiploc].[TiplocId] = [ScheduleTrain].[DestinationStopTiplocId]
				LEFT JOIN [AtocCode]					ON [LiveTrain].[ScheduleTrainAtocCode] = [AtocCode].[AtocCode]
				WHERE
                    [FromStop].[ReportingTiplocId] IN @tiplocsFrom 
                    AND 
                    [FromStop].[EventTypeId] = 1
                    AND 
                    [FromStop].[Public] = 1
					AND	
					[ToStop].[ReportingTiplocId] IN @tiplocsTo 
                    AND 
                    [ToStop].[EventTypeId] = 2
                    AND 
                    [ToStop].[Public] = 1
                    AND
					([ToStop].[PlannedTimestamp] > [FromStop].[PlannedTimestamp])
					AND
					([FromStop].[ActualTimestamp] >= @startDate)
					AND
					([FromStop].[ActualTimestamp] < @endDate)
					AND
					[LiveTrain].[ScheduleTrainCategoryTypeId] IN (1,2,3,10,11,12)
				ORDER BY [FromStop].[PlannedTimestamp]";

            using (DbConnection connection = CreateAndOpenConnection())
            {
                return await connection.QueryAsync<Delay, StationStop, StationStop, AtocCode, Delay>(
                    sql,
                    (d,sf,st,a) =>
                    {
                        d.From = sf;
                        d.To = st;
                        d.Operator = a;
                        return d;
                    },
                    new { tiplocsFrom, tiplocsTo, startDate, endDate },
                    splitOn: "Expected,Expected,Code");
            }
        }
    }
}
