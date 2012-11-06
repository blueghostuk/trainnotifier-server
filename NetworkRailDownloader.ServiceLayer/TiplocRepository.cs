using System.Linq;

namespace TrainNotifier.ServiceLayer
{
    public class TiplocRepository : DbRepository
    {
        public dynamic Get()
        {
            const string sql = @"select 
                 `Tiploc`.Tiploc,
                 `Tiploc`.Nalco,
                 `Tiploc`.Description,
                 `Tiploc`.CRS,
                 `Station`.StationName,
                 `Tiploc`.`Stanox`,
                 Y(Station.Location) AS `lat`,
                 X(Station.Location) AS `lon`
                from `natrail`.`Tiploc`
                left join Station ON Tiploc.Id = Station.TiplocId";

            return Query<dynamic>(sql, null);
        }

        public dynamic GetByStanox(string stanox)
        {
            const string sql = @"select 
                 `Tiploc`.Tiploc,
                 `Tiploc`.Nalco,
                 `Tiploc`.Description,
                 `Tiploc`.CRS,
                 `Station`.StationName,
                 `Tiploc`.`Stanox`,
                 Y(Station.Location) AS `lat`,
                 X(Station.Location) AS `lon`
                from `natrail`.`Tiploc`
                left join Station ON Tiploc.Id = Station.TiplocId
                where Tiploc.Stanox = @stanox";

            // TODO: what if more than 1?
            return Query<dynamic>(sql, new { stanox }).FirstOrDefault();
        }

        public dynamic GetByStationName(string stationName)
        {
            const string sql = @"select 
                 `Tiploc`.Tiploc,
                 `Tiploc`.Nalco,
                 `Tiploc`.Description,
                 `Tiploc`.CRS,
                 `Station`.StationName,
                 `Tiploc`.`Stanox`,
                 Y(Station.Location) AS `lat`,
                 X(Station.Location) AS `lon`
                from `natrail`.`Tiploc`
                left join Station ON Tiploc.Id = Station.TiplocId
                where Station.StationName = @stationName";

            // TODO: what if more than 1?
            return Query<dynamic>(sql, new { stationName }).FirstOrDefault();
        }
    }
}
