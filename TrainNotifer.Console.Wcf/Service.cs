using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using TrainNotifier.Common;
using TrainNotifier.WcfLibrary;

namespace TrainNotifer.Console.Wcf
{
    partial class Service : ServiceBase
    {
        private ServiceHost _serviceHost;

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

        public Service()
        {
            InitializeComponent();

            TraceHelper.SetupTrace();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Trace.TraceError("Unhandled Exception: {0}", e.ExceptionObject);
                TraceHelper.FlushLog();
            };
        }

        protected override void OnStart(string[] args)
        {
            _serviceHost = new ServiceHost(typeof(CacheService));
            _serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
            }
        }
    }

    [RunInstaller(true)]
    public class WcfServerServiceInstaller : Installer
    {
        public WcfServerServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "TrainNotifier WCF Server";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "TrainNotifierWcfService";
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
