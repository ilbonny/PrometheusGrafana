using Autofac;
using PrometheusGrafana.RabbitMq;
using PrometheusGrafana.RabbitMq.Models;

namespace PrometheusGrafana.Configuration
{
    public class RabbitModule : Module
    {
        private readonly RabbitConfiguration _configuration;

        public RabbitModule(RabbitConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<MessageBus>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(RabbitMqConnectionConfiguration),
                    (_, __) => _configuration.RabbitConnectionConfiguration)
                .SingleInstance()
                .AsImplementedInterfaces();

            builder
                .RegisterType<RabbitMqPublisher>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(RabbitMqPublisherConfiguration),
                    (_, __) => _configuration.AddedPublisherConfiguration)
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(System.Type),
                    (_, __) => typeof(PersonAdded))
                .SingleInstance()
                .Named<IRabbitMqPublisher>("AddedPublisherConfiguration");

            builder
                .RegisterType<RabbitMqPublisher>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(RabbitMqPublisherConfiguration),
                    (_, __) => _configuration.ModifiedPublisherConfiguration)
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(System.Type),
                    (_, __) => typeof(PersonModified))
                .SingleInstance()
                .Named<IRabbitMqPublisher>("ModifiedPublisherConfiguration");
            
            builder
                .RegisterType<RabbitMqPublisher>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(RabbitMqPublisherConfiguration),
                    (_, __) => _configuration.DeletedPublisherConfiguration)
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(System.Type),
                    (_, __) => typeof(PersonDeleted))
                .SingleInstance()
                .Named<IRabbitMqPublisher>("DeletedPublisherConfiguration");

            builder
                .RegisterType<ProcessorMessageAdded>()       
                .SingleInstance()
                .Named<IProcessorMessage>("ProcessorMessageAdded");                

             builder
                .RegisterType<ProcessorMessageModified>()        
                .Named<IProcessorMessage>("ProcessorMessageModified")        
                .SingleInstance();

             builder
                .RegisterType<ProcessorMessageDeleted>()        
                .Named<IProcessorMessage>("ProcessorMessageDeleted")        
                .SingleInstance();

            builder.Register(ctx => new RabbitMqConsumer(
                    _configuration.AddedConsumerConfiguration, 
                ctx.ResolveNamed<IProcessorMessage>("ProcessorMessageAdded")))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.Register(ctx => new RabbitMqConsumer(
                    _configuration.ModifiedConsumerConfiguration, 
                ctx.ResolveNamed<IProcessorMessage>("ProcessorMessageModified")))
                .SingleInstance()
                .AsImplementedInterfaces();     

            builder.Register(ctx => new RabbitMqConsumer(
                    _configuration.DeletedConsumerConfiguration, 
                ctx.ResolveNamed<IProcessorMessage>("ProcessorMessageDeleted")))
                .SingleInstance()
                .AsImplementedInterfaces(); 

        }
    }
}