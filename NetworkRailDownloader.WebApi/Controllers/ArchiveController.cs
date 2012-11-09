using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TrainNotifier.Console.WebApi.ViewModels;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class ArchiveController : ApiController
    {
        public IEnumerable<WttIdSearchResult> GetByWttId(string wttId)
        {
            return new ArchiveRepository().GetBtWttId(wttId)
                .Select(r => new WttIdSearchResult
                {
                    WttId = r.sched_wtt_id,
                    TrainId = r.TrainId,
                    From = r.sched_origin_stanox,
                    Depart = r.origin_dep_timestamp
                });
        }
    }
}
