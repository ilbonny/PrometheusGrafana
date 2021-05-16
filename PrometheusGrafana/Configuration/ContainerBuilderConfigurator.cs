using Autofac;

namespace PrometheusGrafana.Configuration
{
    public static class ContainerBuilderConfigurator
    {
        public static void Configure(ContainerBuilder builder, RootConfiguration configuration)
        {
            builder.RegisterModule(new RabbitModule(configuration.RabbitConfiguration));
            builder.RegisterModule(new MongoModule(configuration.MongoConfigurationDb));           
        }        
    }
}