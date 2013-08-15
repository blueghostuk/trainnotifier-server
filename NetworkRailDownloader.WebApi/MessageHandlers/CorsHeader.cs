using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Security.Authentication;

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

        private static bool GetOriginAccepted(string host)
        {
            return _singleCorsHeader != null || _allowedOrigins.Contains(host);
        }

        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            string origin = request.Headers.Origin();
            if (string.IsNullOrEmpty(origin) && !GetOriginAccepted(origin))
                throw new AuthenticationException();
            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, System.Threading.CancellationToken cancellationToken)
        {
            string origin = response.RequestMessage.Headers.Origin();
            if (string.IsNullOrEmpty(origin) && !GetOriginAccepted(origin))
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            response.Headers.Add("Access-Control-Allow-Origin", GetOriginAccepted(origin) ? _singleCorsHeader ?? origin : "null");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept");
            return response;
        }
    }

    public static class HttpHeadersExtensions
    {
        public static string Origin(this HttpHeaders headers)
        {
            IEnumerable<string> values;
            if (headers.TryGetValues("Origin", out values))
            {
                return values.FirstOrDefault();
            }

            return null;
        }
    }
}
