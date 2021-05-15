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

        private static RabbitConnectionConfiguration ReadRabbitMqConfiguration(IConfigurationRoot cfg)
        {
            var rabbitmqSection = cfg.GetSection("RabbitMq");

            return new RabbitConnectionConfiguration
            {
                Uri = rabbitmqSection["Uri"],
                Username = rabbitmqSection["Username"],
                Password = rabbitmqSection["Password"]
            };
        }
    }
}
