using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PrometheusGrafana.Models
{
    public class Person
    {
        public PersonIdentifier Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }

    public class PersonIdentifier
    {
        public string PersonId { get; set; }
        public PersonIdentifier(string personId)
        {
            PersonId = personId;
        }
    }
}