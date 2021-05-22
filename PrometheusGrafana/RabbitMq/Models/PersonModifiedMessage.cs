using System;

namespace PrometheusGrafana.RabbitMq.Models
{
    public class PersonModifiedMessage : IMessage
    {
        public string Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public PersonModifiedMessage(string id)
        {
            Id = id;
            TimeStamp = DateTime.UtcNow;
        }
    }
}