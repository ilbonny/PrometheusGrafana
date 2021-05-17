namespace PrometheusGrafana.Configuration
{
    public class RabbitConfiguration
    {
        public RabbitMqConnectionConfiguration RabbitConnectionConfiguration { get; set; }

        public RabbitMqConsumerConfiguration AddedConsumerConfiguration { get; set; }
        public RabbitMqConsumerConfiguration ModifiedConsumerConfiguration { get; set; }
        public RabbitMqConsumerConfiguration DeletedConsumerConfiguration { get; set; }
        
        public RabbitMqPublisherConfiguration AddedPublisherConfiguration { get; set; }
        public RabbitMqPublisherConfiguration ModifiedPublisherConfiguration { get; set; }
        public RabbitMqPublisherConfiguration DeletedPublisherConfiguration { get; set; }
        
    }

    public class RabbitMqConnectionConfiguration
    {
        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RabbitMqConsumerConfiguration
    {
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
    } 
    
    public class RabbitMqPublisherConfiguration
    {
        public string ExchangeName { get; set; }
    }
}