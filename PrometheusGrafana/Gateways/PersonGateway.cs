using System.Threading.Tasks;
using MongoDB.Driver;
using LanguageExt;
using PrometheusGrafana.Models;

namespace PrometheusGrafana.Gateways
{
    public interface IPersonGateway
    {
        public Task<Option<Person>> Get(string id);
    }

    public class PersonGateway : IPersonGateway
    {
        private readonly IMongoCollection<Person> _collection;

        public PersonGateway(MongoConnectionConfiguration mongoDb)
        {
            _collection = mongoDb.Database.GetCollection<Person>(collectionName);
        }

        public Task<Option<Person>> Get(string id)
        {
            return Task.FromResult(Option<Person>.None);
        }
    }
}