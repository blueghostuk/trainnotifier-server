using System;
using System.Configuration;
using System.Runtime.Caching;
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

        public void CacheTrainMovement(TrainMovement trainMovement)
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

        public void CacheTrainStep(string trainId, TrainMovementStep step)
        {
            CacheTrainStepLocal(trainId, step);
        }

        public void CacheTrainCancellation(string trainId, CancelledTrainMovementStep step)
        {
            CacheTrainStepLocal(trainId, step);
        }

        private void CacheTrainStepLocal(string trainId, TrainMovementStep step)
        {
            CacheStation(step.Stanox, trainId);
            if (step is CancelledTrainMovementStep)
            {
                _cacheDb.AddCancellation(trainId, (CancelledTrainMovementStep)step);
            }
            else
            {
                _cacheDb.AddMovement(trainId, step);
            }
        }
    }
}
