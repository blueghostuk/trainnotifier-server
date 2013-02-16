using System.Collections.Generic;
using System.Linq;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class TiplocRepository : DbRepository
    {
        public dynamic Get()
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Station].[StationName],
                    [Tiploc].[Stanox],
                    [Station].[Location].[Lat] AS [lat],
                    [Station].[Location].[Long] AS [lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]";

            return Query<dynamic>(sql, null);
        }

        public IEnumerable<TiplocCode> GetTiplocs()
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[TiplocId],
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Tiploc].[Stanox]
                FROM [Tiploc]";

            return Query<TiplocCode>(sql, null);
        }

        public short InsertTiploc(string tiploc)
        {
            const string sql = @"
                 INSERT INTO [natrail].[dbo].[Tiploc]
                       ([Tiploc])
                 OUTPUT [inserted].[TiplocId]
                 VALUES
                       (@tiploc)";

            return ExecuteScalar<short>(sql, new { tiploc });
        }

        public dynamic GetByStanox(string stanox)
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Station].[StationName],
                    [Tiploc].[Stanox],
                    [Station].[Location].[Lat] AS [lat],
                    [Station].[Location].[Long] AS [lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]
                WHERE [Tiploc].[Stanox] = @stanox";

            // TODO: what if more than 1?
            return Query<dynamic>(sql, new { stanox }).FirstOrDefault();
        }

        public dynamic GetByStationName(string stationName)
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Station].[StationName],
                    [Tiploc].[Stanox],
                    [Station].[Location].[Lat] AS [lat],
                    [Station].[Location].[Long] AS [lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]
                WHERE [Station].[StationName] = @stationName";

            // TODO: what if more than 1?
            return Query<dynamic>(sql, new { stationName }).FirstOrDefault();
        }

        public dynamic GetByCRSCode(string crsCode)
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Station].[StationName],
                    [Tiploc].[Stanox],
                    [Station].[Location].[Lat] AS [lat],
                    [Station].[Location].[Long] AS [lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]
                WHERE [Tiploc].[CRS] = @crsCode";

            // TODO: what if more than 1?
            return Query<dynamic>(sql, new { crsCode }).FirstOrDefault();
        }
    }
}
