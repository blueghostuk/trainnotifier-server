using System;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Web.Http;
using System.Web.Http.SelfHost;
using TrainNotifier.Common;
using TrainNotifier.Console.WebApi.Config;
using TrainNotifier.Console.WebApi.MessageHandlers;

namespace TrainNotifier.Console.WebApi
{
    partial class Service : ServiceBase
    {
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

        private HttpSelfHostServer _server;

        public Service()
        {
            InitializeComponent();

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
            Uri baseAddress = new Uri("http://" + ConfigurationManager.AppSettings["server"] + ":82");
            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(baseAddress);

            config.MessageHandlers.Add(new CorsHeader());

            RouteConfig.RegisterRoutes(config.Routes);

            _server = new HttpSelfHostServer(config);
            // Start listening 
            _server.OpenAsync().Wait();
            System.Console.WriteLine("Listening on " + baseAddress);
        }

        protected override void OnStop()
        {
            if (_server != null)
            {
                _server.CloseAsync().Wait();
            }
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

            serviceInstaller.DisplayName = "TrainNotifier Web API Server";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "TrainNotiferWebApiServer";
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
