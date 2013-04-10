using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.PPM;
using TrainNotifier.Console.WebApi.ActionFilters;
using TrainNotifier.Service;

namespace TrainNotifier.Console.WebApi.Controllers
{
    public class PPMController : ApiController
    {
        [CachingActionFilterAttribute(604800)]
        public IEnumerable<PPMSector> Get()
        {
            PPMDataRepository repo = new PPMDataRepository();
            return repo.GetSectors()
                .Where(s => !(s.OperatorCode != null && s.SectorCode != null));
        }

        [CachingActionFilterAttribute(604800)]
        public IEnumerable<PPMSector> Get(byte id)
        {
            PPMDataRepository repo = new PPMDataRepository();
            return repo.GetSectors(id);
        }

        [HttpGet]
        [CachingActionFilterAttribute(60)]
        public PPMRecord Get(byte? operatorCode, string name)
        {
            PPMDataRepository repo = new PPMDataRepository();
            return repo.GetLatestRecord(operatorCode, name);
        }
    }
}
