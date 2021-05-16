using Autofac;
using PrometheusGrafana.MongoDb;
using PrometheusGrafana.MongoDb.Gateways;

namespace PrometheusGrafana.Configuration
{
    public class MongoModule : Module
    {
        private readonly MongoConnectionConfiguration _configuration;

        public MongoModule(MongoConnectionConfiguration configuration)
        {
            _configuration = configuration;            
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<MongoConnDatabase>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(MongoConnectionConfiguration),
                    (_, __) => _configuration)
                .SingleInstance()
                .AsImplementedInterfaces();

             builder
                .RegisterType<PersonGateway>() 
                .SingleInstance()
                .AsImplementedInterfaces();

            builder
                .RegisterType<ActionGateway>() 
                .SingleInstance()
                .AsImplementedInterfaces();
        }
    }
}
