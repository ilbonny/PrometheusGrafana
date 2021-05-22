using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PrometheusGrafana.Configuration;
using PrometheusGrafana.Models;

namespace PrometheusGrafana.Api
{
    public interface IPersonGateway
    {
        Task<Person> Get(string id);
        Task<Person> Post(Person person);
        Task<bool> Put(Person person);
        Task<bool> Delete(string id);
    }

    public class PersonGateway : IPersonGateway
    {
        private ApiConfiguration _configuration;
        private IHttpClientHelper _httpClientHelper;
        private Dictionary<string, string> EmptyDictionary = new Dictionary<string, string> { };

        public PersonGateway(ApiConfiguration configuration, IHttpClientHelper httpClientHelper)
        {
            _configuration = configuration;
            _httpClientHelper = httpClientHelper;
        }

        public Task<Person> Get(string id)
        {
            var url = ComposeUrl(_configuration.Url, _configuration.GetPath,
                new Dictionary<string, string> { { "id", id } });

            return _httpClientHelper.Get<Person>(url);
        }

        public Task<Person> Post(Person person)
        {
            var url = ComposeUrl(_configuration.Url, _configuration.AddPath,
                        EmptyDictionary);

            return _httpClientHelper.Post<Person, Person>(url, person, HttpStatusCode.Created);
        }

        public Task<bool> Put(Person person)
        {
            var url = ComposeUrl(_configuration.Url, _configuration.ModifyPath,
                        EmptyDictionary);

            return _httpClientHelper.Put<Person>(url, person, HttpStatusCode.OK);
        }

        public Task<bool> Delete(string id)
        {
            var url = ComposeUrl(_configuration.Url, _configuration.DeletePath,
                new Dictionary<string, string> { { "id", id } });

            return _httpClientHelper.Delete(url);
        }

        public static string ComposeUrl(string baseUrl, string path, IDictionary<string, string> replacements = null)
        {
            foreach (var (key, value) in replacements)
                path = path.Replace("{{" + key + "}}", value);

            return new UriBuilder($"{baseUrl}/{path}").Uri.ToString();
        }
    }



}