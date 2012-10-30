using Alchemy.Classes;
using NetworkRailDownloader.Common;
using NetworkRailDownloader.Downloader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkRailDownloader.Console
{
    internal sealed class NMSWrapper
    {
        private readonly UserManager _userManager;
        private readonly IDownloader _nmsDownloader;

        public NMSWrapper(UserManager userManager, bool quitOnError = false)
        {
            _userManager = userManager;
            _nmsDownloader = new NMSConnector(quitOnError);
        }

        public void Start()
        {
            _nmsDownloader.FeedDataRecieved += (src, feedData) =>
            {
                dynamic dataJson = JsonConvert.DeserializeObject<dynamic>(feedData.Data);
                Parallel.ForEach(_userManager.ActiveUsers
                    .Where(u => DoSendData(dataJson, u.Value)), uc => SendData(uc, dataJson as IEnumerable<dynamic>));
            };

            Task.Run(() => _nmsDownloader.SubscribeToFeed(Feed.TrainMovement));
        }

        public void Stop()
        {
            _nmsDownloader.Quit();
        }

        private static bool DoSendData(dynamic evtData, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return true;

            foreach (dynamic evt in evtData)
            {
                if (evt.body.loc_stanox == filter)
                    return true;
            }
            return false;
        }

        private static void SendData(KeyValuePair<UserContext, string> uc, IEnumerable<dynamic> evtData)
        {
            var data = evtData;
            if (!string.IsNullOrWhiteSpace(uc.Value))
            {
                data = evtData.Where(e => e.body.loc_stanox == uc.Value);
            }
            uc.Key.Send(JsonConvert.SerializeObject(data.Select(e => e.body)));
        }
    }
}
