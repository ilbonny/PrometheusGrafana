using System;

namespace PrometheusGrafana.RabbitMq.Models
{
    public class PersonAddedMessage : IMessage
    {
        public string Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public PersonAddedMessage(string id)
        {
            Id = id;
            TimeStamp = DateTime.UtcNow;
        }
    }
}