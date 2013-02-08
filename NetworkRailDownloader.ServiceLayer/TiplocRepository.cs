using System.Linq;

namespace TrainNotifier.ServiceLayer
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
