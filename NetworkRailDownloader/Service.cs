using NetworkRailDownloader.Common;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.ServiceProcess;

namespace NetworkRailDownloader.Console
{
    partial class Service : ServiceBase
    {
        private WebSocketServerWrapper _wsServerWrapper;
        private UserManager _userManager;
        private NMSWrapper _nmsWrapper;
        private CacheController _cacheController;

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
                    Service service = new Service(false);
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

        private readonly bool _asService = true;

        public Service(bool asService = true)
        {
            InitializeComponent();

            _asService = asService;

            TraceHelper.SetupTrace();
        }

        protected override void OnStart(string[] args)
        {
            _wsServerWrapper = new WebSocketServerWrapper();
            _userManager = new UserManager(_wsServerWrapper);
            _wsServerWrapper.Start();
            Trace.TraceInformation("Started server on {0}:{1}", IPAddress.Any, 81);

            _nmsWrapper = new NMSWrapper(_userManager, _asService ? true : false);
            _cacheController = new CacheController(_nmsWrapper, _wsServerWrapper, _userManager);
            _nmsWrapper.Start();
        }

        protected override void OnStop()
        {
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

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "TrainNotiferWsServer";
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
