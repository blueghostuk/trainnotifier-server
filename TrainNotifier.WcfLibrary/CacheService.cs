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
                foreach (var train in trainData)
                {
                    TrainMovement tm = train as TrainMovement;
                    if (tm != null)
                    {
                        CacheTrainMovement(tm);
                    }
                    else
                    {
                        CancelledTrainMovementStep ctms = train as CancelledTrainMovementStep;
                        if (ctms != null)
                        {
                            CacheTrainCancellation(ctms);
                        }
                        else
                        {
                            TrainMovementStep tms = train as TrainMovementStep;
                            if (tms != null)
                            {
                                CacheTrainStep(tms);
                            }
                        }
                    }
                }
            });
        }

        public void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData)
        {
            Task.Run(() =>
            {
                foreach (var train in trainData)
                {
                    _cacheDb.AddTrainDescriber(train);
                }
            });
        }

        private void CacheTrainMovement(TrainMovement trainMovement)
        {
            _cacheDb.AddActivation(trainMovement);
        }

        private void CacheTrainStep(TrainMovementStep step)
        {
            _cacheDb.AddMovement(step);
        }

        private void CacheTrainCancellation(CancelledTrainMovementStep step)
        {
            _cacheDb.AddCancellation(step);
        }
    }
}
