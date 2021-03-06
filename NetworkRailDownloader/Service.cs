﻿using System;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using TrainNotifier.Common;

namespace TrainNotifier.Console.WebSocketServer
{
    partial class Service : ServiceBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private WebSocketServerWrapper _wsServerWrapper;
        private UserManager _userManager;
        private NMSWrapper _nmsWrapper;
        private CacheController _cacheController;

        private Task _nmsTask;
        private Timer _nmsTimer;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "install")
                {
                    ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                }
                else if (args[0] == "uninstall")
                {
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
                else if (args[0] == "console")
                {
                    // allow cache service to start
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    Service service = new Service();
                    service.OnStart(null);

                    System.Console.WriteLine("Press any key to disconnect");
                    System.Console.ReadLine();

                    service.OnStop();
                    System.Console.WriteLine("Press any key to quit");
                    System.Console.ReadLine();
                }
            }
            else
            {
                ServiceBase.Run(new Service());
            }
        }

        public Service()
        {
            InitializeComponent();

            _cancellationTokenSource = new CancellationTokenSource();

            TraceHelper.SetupTrace();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Trace.TraceError("Unhandled Exception: {0}", e.ExceptionObject);
                TraceHelper.FlushLog();
                ExitCode = -1;
            };
        }

        protected override void OnStart(string[] args)
        {
            int port = int.Parse(ConfigurationManager.AppSettings["WebSocketServerPort"]);
            _wsServerWrapper = new WebSocketServerWrapper(port: port);
            _userManager = new UserManager(_wsServerWrapper);
            _wsServerWrapper.Start();
            Trace.TraceInformation("Started server on {0}:{1}", IPAddress.Any, port);

            _nmsWrapper = new NMSWrapper(_userManager, _cancellationTokenSource);
            _cacheController = new CacheController(_nmsWrapper, _wsServerWrapper, _userManager);
            _nmsTask = _nmsWrapper.Start();

            _nmsTimer = new Timer((s) =>
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    Trace.TraceError("Cancellation Requested");
                    ExitCode = -1;
                    throw new OperationCanceledException(_cancellationTokenSource.Token);
                }
                else if (_nmsTask.IsFaulted)
                {
                    Trace.TraceError("NMS Task Faulted: {0}", _nmsTask.Exception);
                    ExitCode = -1;
                    throw _nmsTask.Exception;
                }
                else if (_nmsTask.IsCompleted)
                {
                    Trace.TraceInformation("NMS Task Finished");

                    Stop();
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        protected override void OnStop()
        {
            Trace.TraceInformation("Stopping Service");
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    _cancellationTokenSource.Cancel();
                }
                catch { }
            }
            if (_nmsTimer != null)
            {
                _nmsTimer.Change(-1, -1);
                _nmsTimer.Dispose();
            }
            if (_nmsWrapper != null)
            {
                _nmsWrapper.Stop();
            }
            if (_wsServerWrapper != null)
            {
                _wsServerWrapper.Stop();
            }
            if (_cacheController != null)
            {
                _cacheController.Dispose();
            }
            try
            {
                Trace.Flush();
            }
            catch { }
        }
    }

    [RunInstaller(true)]
    public class WebSocketServerServiceInstaller : Installer
    {
        public WebSocketServerServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "TrainNotifier Web Socket Server";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServicesDependedOn = new[] { "TrainNotifierWcfService", "TrainNotifierTDService" };

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "TrainNotiferWsServer";
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
