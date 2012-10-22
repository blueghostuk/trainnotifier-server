using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRailDownloader.ServiceLayer
{
    public class TiplocRepository : DbRepository
    {
        public dynamic GetByStanox(string stanox)
        {
            const string sql = @"select 
                 `Tiploc`.Tiploc,
                 `Tiploc`.Nalco,
                 `Tiploc`.Description,
                 `Tiploc`.CRS,
                 `Station`.StationName,
                 Y(Station.Location) AS `lat`,
                 X(Station.Location) AS `lon`
                from `natrail`.`Tiploc`
                left join Station ON Tiploc.Id = Station.TiplocId
                where Tiploc.Stanox = @stanox";

            // TODO: what if more than 1?
            return Query<dynamic>(sql, new { stanox }).FirstOrDefault();

        }
    }
}
