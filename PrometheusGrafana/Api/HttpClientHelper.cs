using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PrometheusGrafana.Api
{
    public interface IHttpClientHelper
    {
        Task<T> Get<T>(string url);
        Task<TOut> Post<TIn, TOut>(string url, TIn payload, params HttpStatusCode[] acceptedStatusCodes);
        Task<bool> Delete(string url);
        Task<bool> Put<T>(string url, T payload, params HttpStatusCode[] acceptedStatusCodes);
    }

    public class HttpClientHelper : IHttpClientHelper
    {
        HttpClientHandler _clientHandler;

        public HttpClientHelper()
        {
            _clientHandler = new HttpClientHandler();
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };            
        }

        public async Task<T> Get<T>(string url)
        {
            var requestUri = new Uri(url);

            var httpClient = new HttpClient(_clientHandler);
            using var responseMessage = await httpClient.GetAsync(requestUri);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
                throw new WrongStatusCodeException(responseMessage);

            var responseStream = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseStream);
        }

        public async Task<TOut> Post<TIn, TOut>(string url, TIn payload, params HttpStatusCode[] acceptedStatusCodes)
        {         
            var payloadSerialized = JsonConvert.SerializeObject(payload);

            HttpContent httpContent = new StringContent(payloadSerialized, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient(_clientHandler);
            var responseMessage = await httpClient.PostAsync(url, httpContent);

            if (!acceptedStatusCodes.Contains(responseMessage.StatusCode))
                throw new WrongStatusCodeException(responseMessage);

            var responseStream = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TOut>(responseStream);
        }

        public async Task<bool> Put<T>(string url, T payload, params HttpStatusCode[] acceptedStatusCodes)
        {
            var payloadSerialized = JsonConvert.SerializeObject(payload);

            HttpContent httpContent = new StringContent(payloadSerialized, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient(_clientHandler);
            var responseMessage = await httpClient.PutAsync(url, httpContent);

            if (!acceptedStatusCodes.Contains(responseMessage.StatusCode))
                throw new WrongStatusCodeException(responseMessage);

            return true;
        }

        public async Task<bool> Delete(string url)
        {
            var requestUri = new Uri(url);

            var httpClient = new HttpClient(_clientHandler);
            var responseMessage = await httpClient.DeleteAsync(requestUri);

            if (responseMessage.StatusCode != HttpStatusCode.NoContent)
                throw new WrongStatusCodeException(responseMessage);

            return true;
        }        
    }
}