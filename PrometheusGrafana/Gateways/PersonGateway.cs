using System.Threading.Tasks;
using MongoDB.Driver;
using LanguageExt;
using PrometheusGrafana.Models;

namespace PrometheusGrafana.Gateways
{
    public interface IPersonGateway
    {
        public Task<Option<Person>> Get(string id);
        public Task<Person> Insert(Person person);
        public Task Save(Person person);
        public Task Delete(string id);
    }

    public class PersonGateway : IPersonGateway
    {
        private readonly IMongoCollection<Person> _collection;
        private const string CollectionName = "Persons";

        public PersonGateway(IMongoDb mongoDb)
        {
            _collection = mongoDb.Database.GetCollection<Person>(CollectionName);
        }

        public async Task<Option<Person>> Get(string id)
        {
            var filter = Builders<Person>.Filter.Eq(doc => doc.Id, id);
            var person = (await _collection.FindAsync<Person>(filter)).FirstOrDefault();
            return person ?? Option<Person>.None;            
        }

        public async Task<Person> Insert(Person person)
        {
            await _collection.InsertOneAsync(person);
            return person;
        }

        public Task Save(Person person)
        {
            var filter = Builders<Person>.Filter.Eq(doc => doc.Id, person.Id);
            return _collection.ReplaceOneAsync(filter, person, new ReplaceOptions
            {
                IsUpsert = true
            });
        }

        public Task Delete(string id)
        {
            var filter = Builders<Person>.Filter.Eq(doc => doc.Id, id);
            return _collection.DeleteOneAsync(filter);
        }
    }
}