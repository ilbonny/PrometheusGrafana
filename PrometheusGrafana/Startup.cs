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

            var queueAddedName = "Prometheus.Person.Queue.Added";
            var queueModifiedName = "Prometheus.Person.Queue.Modified";
            
            services.AddSingleton<IRabbitMqPublisher<PersonAdded>>(x =>
                new RabbitMqPublisher<PersonAdded>(config.RabbitConfiguration, "Prometheus.Person.Exchange.Added",queueAddedName));

            services.AddSingleton<IRabbitMqPublisher<PersonModified>>(x =>
                new RabbitMqPublisher<PersonModified>(config.RabbitConfiguration, "Prometheus.Person.Exchange.Modified",queueModifiedName));

            services.AddSingleton<IRabbitMqConsumer<PersonAdded>>(x =>
                new RabbitMqConsumer<PersonAdded>(config.RabbitConfiguration, queueAddedName,
                services.BuildServiceProvider().GetService<IActionGateway>()));

            services.AddSingleton<IRabbitMqConsumer<PersonModified>>(x =>
                new RabbitMqConsumer<PersonModified>(config.RabbitConfiguration, queueModifiedName,
                services.BuildServiceProvider().GetService<IActionGateway>()));

            services.AddSingleton<IMessageBus>(x=> new MessageBus(
                services.BuildServiceProvider().GetService<IRabbitMqConsumer<PersonAdded>>(),
                services.BuildServiceProvider().GetService<IRabbitMqConsumer<PersonModified>>())                
            );

        }
    }
}
