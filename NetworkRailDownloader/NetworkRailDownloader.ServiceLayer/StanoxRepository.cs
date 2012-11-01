using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRailDownloader.ServiceLayer
{
    public class StanoxRepository : DbRepository
    {
        public string GetStanoxByCrs(string crs)
        {
            const string sql = "SELECT Stanox FROM Tiploc WHERE CRS = @crs";

            return ExecuteScalar<string>(sql, new { crs });
        }
    }
}
