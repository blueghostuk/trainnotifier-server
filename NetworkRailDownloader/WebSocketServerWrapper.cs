﻿using Alchemy;
using System;
using System.Net;

namespace TrainNotifier.Console.WebSocketServer
{
    internal sealed class WebSocketServerWrapper
    {
        private readonly Alchemy.WebSocketServer _webSocketServer;

        public event EventHandler<UserContextEventArgs> OnConnect;
        public event EventHandler<UserContextEventArgs> OnConnected;
        public event EventHandler<UserContextEventArgs> OnDisconnect;
        public event EventHandler<UserContextEventArgs> OnReceive;
        public event EventHandler<UserContextEventArgs> OnSend;

        public WebSocketServerWrapper(int port = 81, IPAddress ipAddress = null)
        {
            _webSocketServer = new Alchemy.WebSocketServer(port, (ipAddress ?? IPAddress.Any))
            {
                TimeOut = TimeSpan.FromMilliseconds(-1)
            };
            _webSocketServer.OnConnect = new OnEventDelegate((uctx) =>
            {
                var oc = OnConnect;
                if (null != oc)
                    oc(this, new UserContextEventArgs(uctx));
            });
            _webSocketServer.OnConnected = new OnEventDelegate((uctx) =>
            {
                var oc = OnConnected;
                if (null != oc)
                    oc(this, new UserContextEventArgs(uctx));
            });
            _webSocketServer.OnDisconnect = new OnEventDelegate((uctx) =>
            {
                var oc = OnDisconnect;
                if (null != oc)
                    oc(this, new UserContextEventArgs(uctx));
            });
            _webSocketServer.OnReceive = new OnEventDelegate((uctx) =>
            {
                var or = OnReceive;
                if (null != or)
                    or(this, new UserContextEventArgs(uctx));
            });
            _webSocketServer.OnSend = new OnEventDelegate((uctx) =>
            {
                var os = OnSend;
                if (null != os)
                    os(this, new UserContextEventArgs(uctx));
            });            
        }

        public void Start()
        {
            _webSocketServer.Start();
        }

        public void Stop()
        {
            _webSocketServer.Stop();
        }
    }
}
