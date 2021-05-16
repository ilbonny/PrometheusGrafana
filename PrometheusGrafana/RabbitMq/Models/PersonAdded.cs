using System;

namespace PrometheusGrafana.RabbitMq.Models
{
    public class PersonAdded : IPersonId
    {
        public string Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public PersonAdded(string id)
        {
            Id = id;
            TimeStamp = DateTime.UtcNow;
        }
    }
}