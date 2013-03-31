using System.Collections.Generic;
using System.Threading.Tasks;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.Schedule;
using TrainNotifier.Common.PPM;
using TrainNotifier.Common.Services;
using TrainNotifier.Service;

namespace TrainNotifier.WcfLibrary
{
    public class CacheService : ICacheService
    {
        private static readonly LiveTrainRepository _cacheDb = new LiveTrainRepository();
        private static readonly ScheduleRepository _scheduleRepository = new ScheduleRepository();

        static CacheService()
        {
            _cacheDb.PreLoadActivations();
        }

        public void CacheTrainData(IEnumerable<ITrainData> trainData)
        {
            Task.Run(() =>
            {
                _cacheDb.BatchInsertTrainData(trainData);
            });
        }

        public void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData)
        {
            Task.Run(() =>
            {
                _cacheDb.BatchInsertTDData(trainData);
            });
        }

        public void CacheVSTPSchedule(ScheduleTrain train)
        {
            Task.Run(() =>
            {
                _scheduleRepository.InsertSchedule(train, ScheduleSource.VSTP);
            });
        }

        public void CachePPMData(RtppmData data)
        {
            Task.Run(() =>
            {

            });
        }
    }
}
