using System.Web.Http;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class ScheduleController : ApiController
    {
        public ScheduleTrain GetByUid(string trainId, string trainUid)
        {
            ScheduleRepository sr = new ScheduleRepository();

            return sr.GetTrainByUid(trainId, trainUid);
        }
    }
}