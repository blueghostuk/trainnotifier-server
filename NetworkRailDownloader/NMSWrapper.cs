using Alchemy.Classes;
using NetworkRailDownloader.Common;
using NetworkRailDownloader.Downloader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkRailDownloader.Common
{
    internal sealed class NMSWrapper
    {
        private readonly UserManager _userManager;
        private readonly IDownloader _nmsDownloader;

        public event EventHandler<FeedEventArgs> FeedDataRecieved;

        public NMSWrapper(UserManager userManager)
        {
            _userManager = userManager;
            _nmsDownloader = new NMSConnector();
        }

        public void Start()
        {
            _nmsDownloader.FeedDataRecieved += (src, feedData) =>
            {
                dynamic evtData = JsonConvert.DeserializeObject<dynamic>(feedData.Data);
                Parallel.ForEach(_userManager.ActiveUsers
                    .Where(u => u.Value.State == UserContextState.SubscribeToFeed)
                    .Where(u => DoSendData(evtData, u.Value.StateArgs.ToString())), uc => SendData(uc, evtData as IEnumerable<dynamic>));

                var eh = FeedDataRecieved;
                if (null != eh)
                    eh(this, new FeedEventArgs(evtData));
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

        private static void SendData(KeyValuePair<UserContext, UserContextData> uc, IEnumerable<dynamic> evtData)
        {
            var data = evtData;
            if (!string.IsNullOrWhiteSpace(uc.Value.StateArgs.ToString()))
            {
                data = evtData.Where(e => e.body.loc_stanox == uc.Value);
            }
            uc.Key.Send(JsonConvert.SerializeObject(data.Select(e => e.body)));
        }
    }

    public sealed class FeedEventArgs : EventArgs
    {
        public readonly dynamic Data;

        public FeedEventArgs(dynamic data)
        {
            Data = data;
        }
    }
}
