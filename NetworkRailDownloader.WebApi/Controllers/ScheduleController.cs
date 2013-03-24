using System.Web.Http;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class ScheduleController : ApiController
    {
        // cache for 12 hrs
        [CachingActionFilterAttribute(60 * 60 * 12)]
        public ScheduleTrain GetByUid(string trainId, string trainUid)
        {
            ScheduleRepository sr = new ScheduleRepository();

            return sr.GetTrainByUid(trainId, trainUid);
        }
    }
}