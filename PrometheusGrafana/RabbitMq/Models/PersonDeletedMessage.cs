using System;

namespace PrometheusGrafana.RabbitMq.Models
{
    public class PersonDeletedMessage : IMessage
    {
        public string Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public PersonDeletedMessage(string id)
        {
            Id = id;
            TimeStamp = DateTime.UtcNow;
        }
    }
}