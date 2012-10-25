using Alchemy;
using Alchemy.Classes;
using Essential.Diagnostics;
using NetworkRailDownloader.Common;
using NetworkRailDownloader.Downloader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NetworkRailDownloader.Console
{
    class Program
    {
        private static readonly IDictionary<UserContext, string> _activeUsers = new Dictionary<UserContext, string>();

        static void Main(string[] args)
        {
            SetupTrace();

            System.Console.WriteLine("Press ENTER key to disconnect");

            WebSocketServer webSocketServer = new WebSocketServer(81, IPAddress.Any)
            {
                OnConnected = new OnEventDelegate(Program.OnWsConnected),
                OnDisconnect = new OnEventDelegate(Program.OnWsDisconnect),
                OnReceive = new OnEventDelegate(Program.OnWsReceive),
                TimeOut = TimeSpan.FromMinutes(5)
            };
            webSocketServer.Start();

            Trace.TraceInformation("Started server on {0}:{1}", IPAddress.Any, 81);

            IDownloader downloader = (IDownloader)new NMSConnector();

            downloader.FeedDataRecieved += (src, feedData) =>
            {
                dynamic dataJson = JsonConvert.DeserializeObject<dynamic>(feedData.Data);
                Parallel.ForEach(_activeUsers
                    .Where(u => DoSendData(dataJson, u.Value)), uc => SendData(uc, dataJson as IEnumerable<dynamic>));
            };

            Task.Run(() => downloader.SubscribeToFeed(Feed.TrainMovement));

            System.Console.ReadLine();

            downloader.Quit();
            webSocketServer.Stop();

            System.Console.WriteLine("Press ENTER key to quit");
            System.Console.ReadLine();
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

        private static void OnWsReceive(UserContext context)
        {
            System.Console.WriteLine("Received {0} from {1}", context.DataFrame.ToString(), context.ClientAddress);
            _activeUsers[context] = context.DataFrame.ToString();
        }

        private static void OnWsConnected(UserContext context)
        {
            System.Console.WriteLine("Connection From : {0}", context.ClientAddress);
            _activeUsers.Add(context, string.Empty);
        }

        private static void OnWsDisconnect(UserContext context)
        {
            System.Console.WriteLine("{0} disconnected", context.ClientAddress);
            _activeUsers.Remove(context);
        }

        private static void SetupTrace()
        {
            Trace.Listeners.Add(new ColoredConsoleTraceListener
            {
                Template = "{DateTime:HH':'mm':'ssZ} [{Thread}] {EventType} {Id}: {Message}{Data}"
            });
        }
    }
}
