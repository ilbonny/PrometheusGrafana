using Autofac;

namespace PrometheusGrafana.Configuration
{
    public static class ContainerBuilderConfigurator
    {
        public static void Configure(ContainerBuilder builder, RootConfiguration configuration)
        {
            builder.RegisterModule(new RabbitModule(configuration.RabbitConfiguration));
            builder.RegisterModule(new MongoModule(configuration.MongoConfiguration));       
            builder.RegisterModule(new MetricsModule(configuration.MetricsConfiguration));      
            builder.RegisterModule(new FlowModule(configuration.JobConfiguration, configuration.ApiConfiguration));   
        }        
    }
}