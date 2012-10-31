using NetworkRailDownloader.Console.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRailDownloader.Console
{
    internal sealed class CacheController : IDisposable
    {
        private static readonly ObjectCache _cache = MemoryCache.Default;
        private readonly UserManager _userManager;

        private static CacheItemPolicy GetDefaultCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
            };
        }

        public CacheController(NMSWrapper nmsWrapper, WebSocketServerWrapper wssWrapper, UserManager userManager)
        {
            _userManager = userManager;
            nmsWrapper.FeedDataRecieved += (s, f) =>
            {
                foreach (var message in f.Data)
                {
                    switch (byte.Parse((string)message.header.msg_type))
                    {
                            // activation
                        case 1:
                            CacheActivation(message.body);
                            break;

                            // cancellation
                        case 2:
                            break;

                            // train movement
                        case 3:
                            CacheTrainMovement(message.body.train_id, message.body);
                            break;

                        // unidentified train
                        case 4:
                            break;

                        // train reinstatement
                        case 5:
                            break;

                        // train change of origin
                        case 6:
                            break;

                        // train change of identity
                        case 7:
                            break;

                        // train change of location
                        case 8:
                            break;
                    }
                }
            };

            wssWrapper.OnReceive += (s, context) =>
            {
                string command = context.UserContext.DataFrame.ToString();
                switch (new string(command.Take(10).ToArray()))
                {
                    case "gettrain--":
                        string trainId = new string(command.Skip(11).ToArray());
                        TrainMovement tm = TryGetTrainMovement(trainId);
                        if (tm != null)
                        {
                            string response = JsonConvert.SerializeObject(tm);
                            context.UserContext.Send(response);
                        }
                        break;
                    /*case "listtrains":
                        IEnumerable<TrainMovement> tms = _cache.Get
                            .Select(v => v.Value as TrainMovement)
                            .Where(v => v != null);

                        if (tms.Any())
                        {
                            string response = JsonConvert.SerializeObject(tms);
                            context.UserContext.Send(response);
                        }
                        break;*/
                }
            };
        }

        private void CacheActivation(dynamic body)
        {
            TrainMovement tm = new TrainMovement
            {
                Activated = UnixTsToDateTime(double.Parse((string)body.creation_timestamp)),
                Id = (string)body.train_id,
                SchedOriginDeparture = UnixTsToDateTime(double.Parse((string)body.origin_dep_timestamp)),
                SchedOriginStanox = (string)body.sched_origin_stanox,
                ServiceCode = (string)body.train_service_code                
            };
            CacheTrainMovement(tm);
        }

        private static void CacheTrainMovement(TrainMovement tm)
        {
            _cache.Add(tm.Id, tm, GetDefaultCacheItemPolicy());
        }

        private void CacheTrainMovement(dynamic trainId, dynamic body)
        {
            TrainMovement tm = TryGetTrainMovement((string)trainId);
            if (tm == null)
            {
                tm = new TrainMovement
                {
                    Id = (string)body.train_id,
                    ServiceCode = (string)body.train_service_code
                };
                CacheTrainMovement(tm);
            }
            if (tm != null)
            {
                DateTime? plannedTime = null;
                if (!string.IsNullOrEmpty((string)body.planned_timestamp))
                {
                    plannedTime = UnixTsToDateTime(double.Parse((string)body.planned_timestamp));
                }
                tm.AddTrainMovementStep(new TrainMovementStep
                {
                    ActualTimeStamp = UnixTsToDateTime(double.Parse((string)body.actual_timestamp)),
                    EventType = (string)body.event_type,
                    Line = (string)body.line_ind,
                    PlannedTime = plannedTime,
                    Platform = (string)body.platform,
                    Stanox = (string)body.loc_stanox,
                    Terminated = ((string)body.train_teminated) == "true"
                });
            }
        }

        private static TrainMovement TryGetTrainMovement(string trainId)
        {
            TrainMovement tm = _cache.Get(trainId) as TrainMovement;
            return tm;
        }

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private static DateTime UnixTsToDateTime(double timeStamp)
        {
            return _epoch.AddMilliseconds(timeStamp);
        }

        public void Dispose()
        {
        }
    }
}
