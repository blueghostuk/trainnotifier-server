using Alchemy.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrainNotifier.Common;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.NMS;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class NMSWrapper
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly UserManager _userManager;
        private readonly INMSConnector _nmsDownloader;

        public event EventHandler<FeedEventArgs> FeedDataRecieved;

        public NMSWrapper(UserManager userManager, CancellationTokenSource cancellationTokenSource)
        {
            _userManager = userManager;
            _cancellationTokenSource = cancellationTokenSource;
            _nmsDownloader = new NMSConnector(_cancellationTokenSource);
        }

        public Task Start()
        {
            _nmsDownloader.TrainDataRecieved += (src, feedData) =>
            {
                // run this as a task to return to callee quicker
                Task.Run(() =>
                {
                    dynamic evtData = JsonConvert.DeserializeObject<dynamic>(feedData.Data);

                    switch (feedData.FeedSource)
                    {
                        case Feed.TrainMovement:

                            Parallel.ForEach(_userManager.ActiveUsers
                                .Where(u => u.Value.State == UserContextState.SubscribeToFeed), uc => SendData(uc, evtData as IEnumerable<dynamic>));

                            Parallel.ForEach(_userManager.ActiveUsers
                                .Where(u => u.Value.State == UserContextState.SubscribeToStanox)
                                .Where(u => DataContainsStanox(evtData, u.Value.StateArgs)), uc => SendStanoxData(uc, evtData as IEnumerable<dynamic>));

                            break;
                    }

                    var eh = FeedDataRecieved;
                    if (null != eh)
                        eh(this, new FeedEventArgs(feedData.FeedSource, evtData));
                }, _cancellationTokenSource.Token);
            };

            return Task.Run(() => _nmsDownloader.SubscribeToFeeds(), _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _nmsDownloader.Quit();
            _cancellationTokenSource.Cancel();
        }

        private bool DataContainsStanox(dynamic evtData, string stanox)
        {
            if (string.IsNullOrWhiteSpace(stanox))
                return false;
            foreach (dynamic evt in evtData)
            {
                if (evt.body.loc_stanox == stanox)
                    return true;
            }
            return false;
        }
        private static void SendStanoxData(KeyValuePair<UserContext, UserContextData> uc, IEnumerable<dynamic> evtData)
        {
            var data = evtData
                .Where(e => e.body.loc_stanox == uc.Value.StateArgs)
                .Select(e => e);

            uc.Key.Send(JsonConvert.SerializeObject(new CommandResponse<IEnumerable<dynamic>>
            {
                Command = "substanoxupdate",
                Args = uc.Value.StateArgs,
                Response = data
            }));
        }

        private static void SendData(KeyValuePair<UserContext, UserContextData> uc, IEnumerable<dynamic> evtData)
        {
            uc.Key.Send(JsonConvert.SerializeObject(evtData.Select(e => e.body)));
        }
    }

    public sealed class FeedEventArgs : EventArgs
    {
        public readonly Feed Source;
        public readonly dynamic Data;

        public FeedEventArgs(Feed source, dynamic data)
        {
            Source = source;
            Data = data;
        }
    }
}
