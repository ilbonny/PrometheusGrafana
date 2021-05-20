using System;

namespace PrometheusGrafana.RabbitMq.Models
{
    public class PersonDeleted : IMessage
    {
        public string Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public PersonDeleted(string id)
        {
            Id = id;
            TimeStamp = DateTime.UtcNow;
        }
    }
}