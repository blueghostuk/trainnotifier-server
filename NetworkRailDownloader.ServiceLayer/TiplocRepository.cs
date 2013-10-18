using System;
using System.Collections.Generic;
using System.Linq;
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
                 INSERT INTO [Tiploc]
                       ([Tiploc])
                 OUTPUT [inserted].[TiplocId]
                 VALUES
                       (@tiploc)";

            return ExecuteScalar<short>(sql, new { tiploc });
        }

        public void InsertTiploc(TiplocCode tiploc)
        {
            const string sql = @"
                 INSERT INTO [Tiploc]
                       ([Tiploc]
                       ,[Nalco]
                       ,[Description]
                       ,[Stanox]
                       ,[CRS])
                 VALUES
                       (@tiploc
                       ,@nalco
                       ,@description
                       ,@stanox
                       ,@crs)";

            Query<short>(sql, new
            {
                tiploc = tiploc.Tiploc,
                nalco = tiploc.Nalco,
                description = tiploc.Description,
                stanox = tiploc.Stanox,
                crs = tiploc.CRS
            });
        }

        public TiplocCode GetTiplocByStanox(string stanox)
        {
            const string sql = @"
                SELECT 
                    [Tiploc].[TiplocId],
                    [Tiploc].[Tiploc],
                    [Tiploc].[Nalco],
                    [Tiploc].[Description],
                    [Tiploc].[CRS],
                    [Tiploc].[Stanox]
                FROM [Tiploc]
                WHERE [Tiploc].[Stanox] = @stanox";

            return Query<TiplocCode>(sql, new { stanox }).FirstOrDefault();
        }

        [Obsolete("Update code to use GetAllByStanox")]
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

        /// <summary>
        /// There can be more than one location per stanox
        /// </summary>
        public IEnumerable<StationTiploc> GetAllByStanox(string stanox)
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

            return Query<StationTiploc>(sql, new { stanox });
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

        [Obsolete("Update code to use GetAllByCRSCode")]
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
        
        public IEnumerable<StationTiploc> GetAllByCRSCode(string crsCode)
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
                WHERE [Tiploc].[CRS] = @crsCode OR [Station].[SubsiduaryAlphaCode] = @crsCode";

            // TODO: what if more than 1?
            return Query<StationTiploc>(sql, new { crsCode });
        }

        public IEnumerable<StationTiploc> GetByLocation(double lat, double lon, int limit)
        {
            const string sql = @"
                DECLARE @g geography = 'POINT({0} {1})';
                SELECT TOP({2})
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
                INNER JOIN [Station] ON [Tiploc].[TiplocId] = [Station].[TiplocId]
                WHERE [Station].[Location].STDistance(@g) IS NOT NULL
                ORDER BY [Station].[Location].STDistance(@g);";

            return Query<StationTiploc>(string.Format(sql, lon, lat, limit));
        }
    }
}
