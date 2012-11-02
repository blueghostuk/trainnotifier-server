using System;
using System.Runtime.Caching;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Services;

namespace TrainNotifier.WcfLibrary
{
    public class CacheService : ICacheService
    {
        private static readonly ObjectCache _tmCache = new MemoryCache("TrainMovements");
        private static readonly ObjectCache _stanoxCache = new MemoryCache("Stanox");

        private static CacheItemPolicy GetDefaultCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
            };
        }

        public void CacheTrainMovement(TrainMovement trainMovement)
        {
            _tmCache.Add(trainMovement.Id, trainMovement, GetDefaultCacheItemPolicy());
            CacheStation(trainMovement.SchedOriginStanox, trainMovement.Id);
        }

        public void CacheStation(string stanoxName, string trainId)
        {
            if (string.IsNullOrWhiteSpace(stanoxName) || string.IsNullOrWhiteSpace(trainId))
                return;

            Stanox stanox;
            if (!TryGetStanox(stanoxName, out stanox))
            {
                stanox = new Stanox(stanoxName);
                _stanoxCache.Add(stanoxName, stanox, GetDefaultCacheItemPolicy());
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
        }

        public bool TryGetTrainMovement(string trainId, out TrainMovement trainMovement)
        {
            trainMovement = _tmCache.Get(trainId) as TrainMovement;
            return trainMovement != null;
        }
    }
}
