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
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://" + ConfigurationManager.AppSettings["server"] + ":82");
            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(baseAddress);

            config.MessageHandlers.Add(new CorsHeader());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var server = new HttpSelfHostServer(config);

            // Start listening 
            server.OpenAsync().Wait();
            Console.WriteLine("Listening on " + baseAddress);
            System.Console.WriteLine("Press ENTER key to disconnect"); 
            
            System.Console.ReadLine();

            server.CloseAsync().Wait();

            System.Console.WriteLine("Press ENTER key to quit");
            System.Console.ReadLine();
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
}
