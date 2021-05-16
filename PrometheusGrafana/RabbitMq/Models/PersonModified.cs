using System;

namespace PrometheusGrafana.RabbitMq.Models
{
    public class PersonModified : IPersonId
    {
        public string Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public PersonModified(string id)
        {
            Id = id;
            TimeStamp = DateTime.UtcNow;
        }
    }
}