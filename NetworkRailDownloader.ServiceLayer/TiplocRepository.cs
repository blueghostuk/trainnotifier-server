using System.Collections.Generic;
using System.Linq;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class TiplocRepository : DbRepository
    {
        public IEnumerable<StationTiploc> Get()
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[TiplocId],
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Tiploc].[Stanox],
                    [Station].[StationName],
                    [Station].[Location].[Lat] AS [Lat],
                    [Station].[Location].[Long] AS [Lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]";

            return Query<StationTiploc>(sql, null);
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

        public StationTiploc GetByStanox(string stanox)
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[TiplocId],
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Tiploc].[Stanox],
                    [Station].[StationName],
                    [Station].[Location].[Lat] AS [Lat],
                    [Station].[Location].[Long] AS [Lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]
                WHERE [Tiploc].[Stanox] = @stanox";

            // TODO: what if more than 1?
            return Query<StationTiploc>(sql, new { stanox }).FirstOrDefault();
        }

        public StationTiploc GetByStationName(string stationName)
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[TiplocId],
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Tiploc].[Stanox],
                    [Station].[StationName],
                    [Station].[Location].[Lat] AS [Lat],
                    [Station].[Location].[Long] AS [Lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]
                WHERE [Station].[StationName] = @stationName";

            // TODO: what if more than 1?
            return Query<StationTiploc>(sql, new { stationName }).FirstOrDefault();
        }

        public StationTiploc GetByCRSCode(string crsCode)
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[TiplocId],
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Tiploc].[Stanox],
                    [Station].[StationName],
                    [Station].[Location].[Lat] AS [Lat],
                    [Station].[Location].[Long] AS [Lon]
                FROM [Tiploc]
                LEFT JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]
                WHERE [Tiploc].[CRS] = @crsCode";

            // TODO: what if more than 1?
            return Query<StationTiploc>(sql, new { crsCode }).FirstOrDefault();
        }
    }
}
