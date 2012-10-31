using Alchemy.Classes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetworkRailDownloader.Console
{
    internal sealed class UserManager
    {
        private readonly IDictionary<UserContext, UserContextData> _activeUsers = new ConcurrentDictionary<UserContext, UserContextData>();

        public IDictionary<UserContext, UserContextData> ActiveUsers
        {
            get { return _activeUsers; }
        }

        public UserManager(WebSocketServerWrapper webSocketServer)
        {
            webSocketServer.OnConnected += (s, context) =>
            {
                Trace.TraceInformation("Connection From : {0}", context.UserContext.ClientAddress);
                _activeUsers.Add(context.UserContext, new UserContextData());
            };

            webSocketServer.OnDisconnect += (s, context) =>
            {
                Trace.TraceInformation("{0} disconnected", context.UserContext.ClientAddress);
                _activeUsers.Remove(context.UserContext);
            };

            webSocketServer.OnReceive += (s, context) =>
            {
                Trace.TraceInformation("Received {0} from {1}", context.UserContext.DataFrame.ToString(), context.UserContext.ClientAddress);
                string command = context.UserContext.DataFrame.ToString();
                switch (command)
                {
                    case "subscribe":
                        _activeUsers[context.UserContext].State = UserContextState.SubscribeToFeed;
                        break;
                    case "unsubscribe":
                        _activeUsers[context.UserContext].State = UserContextState.None;
                        break;
                    default:
                        _activeUsers[context.UserContext].StateArgs = command;
                        break;
                }
                _activeUsers[context.UserContext].LastRequest = command;
            };
        }
    }

    public sealed class UserContextData
    {
        public UserContextState State { get; set; }
        public string LastRequest { get; set; }
        public string StateArgs { get; set; }

        public UserContextData()
        {
            StateArgs = string.Empty;
        }
    }

    public enum UserContextState
    {
        None,
        SubscribeToFeed
    }

}
