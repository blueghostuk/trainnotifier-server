using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace TrainNotifier.Console.WebApi.MessageHandlers
{
    internal sealed class CorsHeader : MessageProcessingHandler
    {
        private static readonly HashSet<string> _allowedOrigins = new HashSet<string>();
        private static readonly string _singleCorsHeader;

        static CorsHeader()
        {
            string origins = ConfigurationManager.AppSettings["corsItems"];
            if (string.IsNullOrEmpty(origins))
            {
                _singleCorsHeader = "*";
            }
            else
            {
                foreach (string origin in origins.Split(','))
                {
                    _allowedOrigins.Add(origin);
                }
            }
        }

        private static string GetOriginAccepted(string host)
        {
            return _singleCorsHeader ?? (_allowedOrigins.Contains(host) ? host : "null");
        }

        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, System.Threading.CancellationToken cancellationToken)
        {
            response.Headers.Add("Access-Control-Allow-Origin", GetOriginAccepted(response.RequestMessage.Headers.Host));
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept");
            return response;
        }
    }
}
