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

        private static readonly ObjectCache _stanoxCache = new MemoryCache("Stanox");

        private static CacheItemPolicy GetDefaultStanoxCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(double.Parse(ConfigurationManager.AppSettings["CacheExpiryDaysStanox"]))
            };
        }

        private void CacheTrainMovement(TrainMovement trainMovement)
        {
            CacheStation(trainMovement.SchedOriginStanox, trainMovement.Id);
            _cacheDb.AddActivation(trainMovement);
        }

        public void CacheStation(string stanoxName, string trainId)
        {
            if (string.IsNullOrWhiteSpace(stanoxName) || string.IsNullOrWhiteSpace(trainId))
                return;

            Stanox stanox;
            if (!TryGetStanox(stanoxName, out stanox))
            {
                stanox = new Stanox(stanoxName);
                _stanoxCache.Add(stanoxName, stanox, GetDefaultStanoxCacheItemPolicy());
            }
            stanox.AddTrainId(trainId);
        }

        public bool TryGetStanox(string stanoxName, out Stanox stanox)
        {
            stanox = _stanoxCache.Get(stanoxName) as Stanox;
            return stanox != null;
        }

        private void CacheTrainStep(TrainMovementStep step)
        {
            if (_cacheDb.AddMovement(step))
            {
                CacheStation(step.Stanox, step.TrainId);
            }
        }

        private void CacheTrainCancellation(CancelledTrainMovementStep step)
        {
            if (_cacheDb.AddCancellation(step))
            {
                CacheStation(step.Stanox, step.TrainId);
            }
        }

        public void CacheTrainData(IEnumerable<ITrainData> trainData)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
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
                ts.Complete();
            }
        }
    }
}
