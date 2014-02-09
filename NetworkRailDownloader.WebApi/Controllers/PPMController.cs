using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
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
        public IEnumerable<PPMRecord> Get(byte? operatorCode, string name, DateTime? startDate = null, DateTime? endDate = null)
        {
            PPMDataRepository repo = new PPMDataRepository();
            if (startDate.HasValue && endDate.HasValue)
            {
                return repo.GetLatestRecords(operatorCode, name, startDate.Value, endDate.Value);
            }
            else
            {
                return new[] { repo.GetLatestRecord(operatorCode, name) };
            }
        }
    }
}
