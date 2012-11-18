using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Transactions;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;
using TrainNotifier.Service;

namespace TrainNotifier.WcfLibrary
{
    public class CacheService : ICacheService
    {
        private static readonly ArchiveRepository _cacheDb = new ArchiveRepository();

        private static CacheItemPolicy GetDefaultStanoxCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(double.Parse(ConfigurationManager.AppSettings["CacheExpiryDaysStanox"]))
            };
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

        public void CacheTrainData(IEnumerable<ITrainData> trainData)
        {
            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            //{
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
                //ts.Complete();
            //}
        }

        public void CacheTrainDescriberData(IEnumerable<TrainDescriber> trainData)
        {
            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew))
            //{
                foreach (var train in trainData)
                {
                    _cacheDb.AddTrainDescriber(train);
                }
                //ts.Complete();
            //}
        }
    }
}
