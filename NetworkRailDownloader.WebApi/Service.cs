using NetworkRailDownloader.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace NetworkRailDownloader.WebApi
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
        }

        protected override void OnStart(string[] args)
        {
            Uri baseAddress = new Uri("http://" + ConfigurationManager.AppSettings["server"] + ":82");
            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(baseAddress);

            config.MessageHandlers.Add(new CorsHeader());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            _server = new HttpSelfHostServer(config);
            // Start listening 
            _server.OpenAsync().Wait();
            Console.WriteLine("Listening on " + baseAddress);
        }

        protected override void OnStop()
        {
            if (_server != null)
            {
                _server.CloseAsync().Wait();
            }
        }

        private sealed class CorsHeader : MessageProcessingHandler
        {
            protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                request.Headers.Add("Access-Control-Allow-Origin", "*");
                request.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept");
                return request;
            }

            protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, System.Threading.CancellationToken cancellationToken)
            {
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept");
                return response;
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
