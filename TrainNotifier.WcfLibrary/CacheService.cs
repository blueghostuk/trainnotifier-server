using System.Collections.Generic;
using System.Threading.Tasks;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;
using TrainNotifier.Service;

namespace TrainNotifier.WcfLibrary
{
    public class CacheService : ICacheService
    {
        private static readonly ArchiveRepository _cacheDb = new ArchiveRepository();

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
    }
}
