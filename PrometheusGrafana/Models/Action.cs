using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace PrometheusGrafana.Models
{
    public class Action
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string ReferenceId { get; set; }

        public string Type { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public Action(string referenceId, string type)
        {
            ReferenceId = referenceId;
            Type = type;
            Timestamp = DateTime.UtcNow;
        }
    }    
}