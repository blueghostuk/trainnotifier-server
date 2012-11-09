using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;

namespace TrainNotifier.WcfLibrary
{
    public class CacheService : ICacheService
    {
        private static readonly ObjectCache _tmCache = new MemoryCache("TrainMovements");
        private static readonly ObjectCache _headCodeCache = new MemoryCache("TrainServices");
        private static readonly ObjectCache _stanoxCache = new MemoryCache("Stanox");

        private static CacheItemPolicy GetDefaultTMCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(double.Parse(ConfigurationManager.AppSettings["CacheExpiryDaysTM"]))
            };
        }

        private static CacheItemPolicy GetDefaultTrainServiceCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(double.Parse(ConfigurationManager.AppSettings["CacheExpiryDaysHeadCode"]))
            };
        }

        private static CacheItemPolicy GetDefaultStanoxCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(double.Parse(ConfigurationManager.AppSettings["CacheExpiryDaysStanox"]))
            };
        }

        private static readonly CacheDatabase _cacheDb = new CacheDatabase();

        public void CacheTrainMovement(TrainMovement trainMovement)
        {
            _tmCache.Add(trainMovement.Id, trainMovement, GetDefaultTMCacheItemPolicy());
            CacheStation(trainMovement.SchedOriginStanox, trainMovement.Id);
            _cacheDb.AddActivation(trainMovement);
            if (!string.IsNullOrWhiteSpace(trainMovement.WorkingTTId))
            {
                string wttid = trainMovement.WorkingTTId.Substring(0, trainMovement.WorkingTTId.Length -1);
                ICollection<string> existing = _headCodeCache.Get(wttid) as ICollection<string>;
                if (existing == null)
                {
                    existing = new HashSet<string>();
                    _headCodeCache.Add(wttid, existing, GetDefaultTrainServiceCacheItemPolicy());
                }

                existing.Add(trainMovement.Id);
            }
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

        public void CacheTrainStep(string trainId, string serviceCode, TrainMovementStep step)
        {
            CacheTrainStepLocal(trainId, serviceCode, step);
        }

        public void CacheTrainCancellation(string trainId, string serviceCode, CancelledTrainMovementStep step)
        {
            CacheTrainStepLocal(trainId, serviceCode, step);
        }

        private void CacheTrainStepLocal(string trainId, string serviceCode, TrainMovementStep step)
        {
            TrainMovement trainMovement;
            if (!TryGetTrainMovement(trainId, out trainMovement))
            {
                trainMovement = new TrainMovement
                {
                    Id = trainId,
                    ServiceCode = serviceCode
                };
                CacheTrainMovement(trainMovement);
            }
            trainMovement.AddTrainMovementStep(step);
            CacheStation(step.Stanox, trainId);
            if (step is CancelledTrainMovementStep)
            {
                _cacheDb.AddCancellation(trainMovement, (CancelledTrainMovementStep)step);
            }
            else
            {
                _cacheDb.AddMovement(trainMovement, step);
            }
        }

        public bool TryGetTrainMovement(string trainId, out TrainMovement trainMovement)
        {
            trainMovement = _tmCache.Get(trainId) as TrainMovement;
            return trainMovement != null;
        }

        public bool TryGetService(string headCode, out IEnumerable<string> trainIds)
        {
            trainIds = _headCodeCache.Get(headCode) as IEnumerable<string>;

            return trainIds != null;
        }
    }
}
