using System;
using System.Net.Http;

namespace PrometheusGrafana.Api
{
    public class WrongStatusCodeException : Exception
    {
        public WrongStatusCodeException(HttpResponseMessage responseMessage) : base(ComposeMessage(responseMessage))
        {
        }

        private static string ComposeMessage(HttpResponseMessage response)
        {
            return
                $@"StatusCode: {response.StatusCode} 
Response Content: '{response.Content?.ReadAsStringAsync().Result}'
Request Uri: '{response.RequestMessage?.RequestUri}'
Request Content: '{response.RequestMessage?.Content?.ReadAsStringAsync().Result}'";
        }
    }
}