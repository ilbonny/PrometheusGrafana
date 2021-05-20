using System;

namespace PrometheusGrafana.RabbitMq.Models
{
    public interface IMessage 
    {
        string Id { get; set; }
        DateTimeOffset TimeStamp { get; set; }
    }
}