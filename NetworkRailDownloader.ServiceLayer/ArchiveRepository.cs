using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainNotifier.ServiceLayer;

namespace TrainNotifier.Service
{
    public class ArchiveRepository : DbRepository
    {
        public IEnumerable<dynamic> GetBtWttId(string wttId)
        {
            const string sql = @"
                SELECT
                    `TrainId`,
                    `origin_dep_timestamp`,
                    `sched_origin_stanox`,
                    `sched_wtt_id`
                FROM `LiveTrain`
                WHERE `sched_wtt_id` LIKE @wttId";

            wttId += "%";

            return Query<dynamic>(sql, new { wttId });
        }
    }
}
