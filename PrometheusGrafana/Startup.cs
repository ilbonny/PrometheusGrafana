using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PrometheusGrafana.Configuration;
using PrometheusGrafana.Models;
using PrometheusGrafana.MongoDb;
using PrometheusGrafana.MongoDb.Gateways;
using PrometheusGrafana.RabbitMq;
using PrometheusGrafana.RabbitMq.Models;

namespace PrometheusGrafana
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                }
            );
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ReadConfigAndIoc(services);
        }

        private static void ReadConfigAndIoc(IServiceCollection services)
        {
            var config = ConfigurationReader.Read();
            services.AddSingleton<IPersonGateway, PersonGateway>();
            services.AddSingleton<IActionGateway, ActionGateway>();

            services.AddSingleton<IMongoConnDatabase>(x =>
                new MongoConnDatabase(config.MongoConfigurationDb));

            var exchangeAddedName = "Prometheus.Person.Exchange.Added";
            var queueAddedName = "Prometheus.Person.Queue.Added";
            var exchangeModifiedName = "Prometheus.Person.Exchange.Modified";
            var queueModifiedName = "Prometheus.Person.Queue.Modified";

            services.AddSingleton<IRabbitMqPublisher>(x =>
                new RabbitMqPublisher(
                    new RabbitMqPublisherConfiguration
                    {
                        ExchangeName = exchangeAddedName,
                        QueueName = queueAddedName
                    })
                );

            services.AddSingleton<IRabbitMqPublisher>(x =>
                new RabbitMqPublisher(
                    new RabbitMqPublisherConfiguration
                    {
                        ExchangeName = exchangeModifiedName,
                        QueueName = queueModifiedName
                    })
                );

            services.AddSingleton<IRabbitMqConsumer>(x =>
                new RabbitMqConsumer(
                    new RabbitMqConsumerConfiguration
                    {
                        ExchangeName = exchangeAddedName,
                        QueueName = queueAddedName
                    },
                    new ProcessorMessageAdded(services.BuildServiceProvider().GetService<IActionGateway>())
                    ));
            
            services.AddSingleton<IRabbitMqConsumer>(x =>
                new RabbitMqConsumer(
                    new RabbitMqConsumerConfiguration
                    {
                        ExchangeName = exchangeModifiedName,
                        QueueName = queueModifiedName
                    },
                    new ProcessorMessageModified(services.BuildServiceProvider().GetService<IActionGateway>())
                    ));

            services.AddSingleton<IMessageBus, MessageBus>();

        }
    }
}
