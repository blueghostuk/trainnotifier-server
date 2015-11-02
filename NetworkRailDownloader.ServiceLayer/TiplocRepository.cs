using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TrainNotifier.Common.Model.Schedule;

namespace TrainNotifier.Service
{
    public class TiplocRepository : DbRepository
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<TiplocCode>> _stanoxCache
             = new ConcurrentDictionary<string, IEnumerable<TiplocCode>>();

        public void PreloadCache()
        {
            var all = GetTiplocs();

            foreach (var tiploc in all.Where(t=> !string.IsNullOrWhiteSpace(t.Stanox)).ToLookup(t => t.Stanox))
            {
                AddToCache(tiploc.Key, tiploc);
            }
        }

        private void AddToCache(string stanox, IEnumerable<TiplocCode> tiplocs)
        {
            _stanoxCache.AddOrUpdate(stanox, tiplocs, (key, existing) => tiplocs);
        }

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
            if (_stanoxCache.ContainsKey(stanox))
            {
                return _stanoxCache[stanox].FirstOrDefault();
            }
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

            var results = Query<TiplocCode>(sql, new { stanox }); 
            if (results.Any())
            {
                AddToCache(stanox, results);
            }
            return results.FirstOrDefault();
        }

        public IEnumerable<TiplocCode> GetTiplocsByStanox(string stanox)
        {
            if (_stanoxCache.ContainsKey(stanox))
            {
                return _stanoxCache[stanox];
            }
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

            var results = Query<TiplocCode>(sql, new { stanox });
            if (results.Any())
            {
                AddToCache(stanox, results);
            }
            return results;
        }

        [Obsolete("Update code to use GetAllByStanox")]
        public StationTiploc GetByStanox(string stanox)
        {
            return GetAllByStanox(stanox).FirstOrDefault();
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

            return Query<StationTiploc>(sql, new { stationName }).FirstOrDefault();
        }

        [Obsolete("Update code to use GetAllByCRSCode")]
        public StationTiploc GetByCRSCode(string crsCode)
        {
            return GetAllByCRSCode(crsCode).FirstOrDefault();
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
