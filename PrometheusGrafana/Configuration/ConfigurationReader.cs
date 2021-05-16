using Microsoft.Extensions.Configuration;

namespace PrometheusGrafana.Configuration
{
    public class ConfigurationReader
    {
        public static RootConfiguration Read()
        {
            var cfg = ConfigurationData.Root;

            return new RootConfiguration
            {
                ServiceName = cfg["ServiceName"],
                MongoConfigurationDb = ReadMongoConfigurationDb(cfg),
                RabbitConfiguration = ReadRabbitMqConfiguration(cfg)
            };
        }

        private static MongoConnectionConfiguration ReadMongoConfigurationDb(IConfigurationRoot cfg)
        {
            var mongoDbSection = cfg.GetSection("MongoDb");

            return new MongoConnectionConfiguration
            {
                ConnectionString = mongoDbSection["ConnectionString"],
                DatabaseName = mongoDbSection["DatabaseName"]
            };
        }

        private static RabbitConfiguration ReadRabbitMqConfiguration(IConfigurationRoot cfg)
        {
            var rabbitmqSection = cfg.GetSection("RabbitMq");
            var publishersSection = rabbitmqSection.GetSection("Publishers");
            var consumersSection = rabbitmqSection.GetSection("Consumers");

            return new RabbitConfiguration
            {
                RabbitConnectionConfiguration = new RabbitMqConnectionConfiguration
                {
                    Uri = rabbitmqSection["Uri"],
                    Username = rabbitmqSection["Username"],
                    Password = rabbitmqSection["Password"]
                },
                AddedConsumerConfiguration = new RabbitMqConsumerConfiguration
                {
                    ExchangeName = consumersSection.GetSection("AddedPublisherConfiguration")["ExchangeName"],
                    QueueName = consumersSection.GetSection("AddedConsumerConfiguration")["QueueName"]
                },
                ModifiedConsumerConfiguration = new RabbitMqConsumerConfiguration
                {
                    ExchangeName = consumersSection.GetSection("ModifiedPublisherConfiguration")["ExchangeName"],
                    QueueName = consumersSection.GetSection("ModifiedConsumerConfiguration")["QueueName"]
                },
                AddedPublisherConfiguration = new RabbitMqPublisherConfiguration
                {
                    ExchangeName = consumersSection.GetSection("AddedPublisherConfiguration")["ExchangeName"]
                },
                ModifiedPublisherConfiguration = new RabbitMqPublisherConfiguration
                {
                    ExchangeName = consumersSection.GetSection("ModifiedPublisherConfiguration")["ExchangeName"]
                }
            };
        }
    }
}
