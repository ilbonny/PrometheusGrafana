using System.Threading.Tasks;
using MongoDB.Driver;
using LanguageExt;
using PrometheusGrafana.Models;
using System;

namespace PrometheusGrafana.Gateways
{
    public interface IPersonGateway
    {
        public Task<Option<Person>> Get(string id);
        public Task<Person> Insert(Person person);
        public Task Save(Person person);
    }

    public class PersonGateway : IPersonGateway
    {
        private readonly IMongoCollection<Person> _collection;
        private const string CollectionName = "Persons";

        public PersonGateway(IMongoDb mongoDb)
        {
            _collection = mongoDb.Database.GetCollection<Person>(CollectionName);
        }

        public Task<Option<Person>> Get(string id)
        {
            var person = _collection.Find<Person>(GenerateIdFilterDefinition(ObjectId(id))).FirstOrDefault();
            return person ?? Option<Person>.None;            
        }

        public async Task<Person> Insert(Person person)
        {
            await _collection.InsertOneAsync(person);
            return person;
        }

        public Task Save(Person person)
        {
            return _collection.ReplaceOneAsync(GenerateIdFilterDefinition(person.Id), person, new ReplaceOptions
            {
                IsUpsert = true
            });
        }

        private static FilterDefinition<Person> GenerateIdFilterDefinition(Object id)
        {
            return Builders<Person>.Filter.Eq(doc => doc.Id, id);
        }
    }
}