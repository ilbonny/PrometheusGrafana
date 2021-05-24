using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using PrometheusGrafana.Metrics;

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
                MongoConfiguration = ReadMongoConfigurationDb(cfg),
                RabbitConfiguration = ReadRabbitMqConfiguration(cfg),
                MetricsConfiguration = ReadMetricsConfiguration(cfg),
                ApiConfiguration = ReadApiConfiguration(cfg),
                JobConfiguration = ReadJobConfigutation(cfg)
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
                    ExchangeName = publishersSection.GetSection("AddedPublisherConfiguration")["ExchangeName"],
                    QueueName = consumersSection.GetSection("AddedConsumerConfiguration")["QueueName"]
                },
                ModifiedConsumerConfiguration = new RabbitMqConsumerConfiguration
                {
                    ExchangeName = publishersSection.GetSection("ModifiedPublisherConfiguration")["ExchangeName"],
                    QueueName = consumersSection.GetSection("ModifiedConsumerConfiguration")["QueueName"]
                },
                DeletedConsumerConfiguration = new RabbitMqConsumerConfiguration
                {
                    ExchangeName = publishersSection.GetSection("DeletedPublisherConfiguration")["ExchangeName"],
                    QueueName = consumersSection.GetSection("DeletedConsumerConfiguration")["QueueName"]
                },
                AddedPublisherConfiguration = new RabbitMqPublisherConfiguration
                {
                    ExchangeName = publishersSection.GetSection("AddedPublisherConfiguration")["ExchangeName"]
                },
                ModifiedPublisherConfiguration = new RabbitMqPublisherConfiguration
                {
                    ExchangeName = publishersSection.GetSection("ModifiedPublisherConfiguration")["ExchangeName"]
                },
                DeletedPublisherConfiguration = new RabbitMqPublisherConfiguration
                {
                    ExchangeName = publishersSection.GetSection("DeletedPublisherConfiguration")["ExchangeName"]
                }
            };
        }

        private static MetricsConfiguration ReadMetricsConfiguration(IConfigurationRoot cfg)
        {
            var metricsSection = cfg.GetSection("Metrics");

            return new MetricsConfiguration
            {
                HostName = metricsSection["HostName"],
                Port = Int32.Parse(metricsSection["Port"]),
                Url = metricsSection["Url"],
                SuppressDefaultMetrics = bool.Parse(metricsSection["SuppressDefaultMetrics"]),
                Enabled = bool.Parse(metricsSection["Enabled"]),
                Histograms = ReadHistogramConfigurations(metricsSection)
            };
        }

        private static ApiConfiguration ReadApiConfiguration(IConfigurationRoot cfg)
        {
            var apiSection = cfg.GetSection("Api");
            var personSection = apiSection.GetSection("Person");

            return new ApiConfiguration
            {
                Url = apiSection["Url"],
                AddPath = personSection["AddPath"],
                GetPath = personSection["GetPath"],
                ModifyPath = personSection["ModifyPath"],
                DeletePath = personSection["DeletePath"]
            };
        }

        private static IEnumerable<HistogramConfiguration> ReadHistogramConfigurations(IConfigurationSection cfg)
        {
            return cfg.GetSection("Histograms").GetChildren()
                .Select(c => new HistogramConfiguration
                {
                    Id = c["Id"],
                    Buckets = BucketsParser.Parse(c["Buckets"])
                });
        }

        private static JobConfiguration ReadJobConfigutation(IConfigurationRoot cfg)
        {
            var jobSection = cfg.GetSection("Job");

            return new JobConfiguration
            {
                Interval = TimeSpan.FromSeconds(int.Parse(jobSection["Interval"]))
            };
        }
    }    
}
