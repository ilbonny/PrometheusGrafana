using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PrometheusGrafana.Configuration;

namespace PrometheusGrafana.MongoDb
{
    public interface IMongoConnDatabase
    {
        IMongoDatabase Database { get; set; }
    }

    public class MongoConnDatabase : IMongoConnDatabase
    {
        public MongoDB.Driver.IMongoDatabase Database { get; set; }

        static MongoConnDatabase()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, _ => true);

            BsonSerializer.RegisterSerializer(typeof(DateTimeOffset), new DateTimeOffsetSerializer(BsonType.Document));
        }

        public MongoConnDatabase(MongoConnectionConfiguration config)
        {
            var url = new MongoUrlBuilder(config.ConnectionString).ToMongoUrl();
            var mongoClient = new MongoClient(url);
            Database = mongoClient.GetDatabase(config.DatabaseName);
        }
    }
}