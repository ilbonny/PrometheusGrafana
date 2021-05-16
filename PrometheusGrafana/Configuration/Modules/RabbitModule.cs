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
                .AsImplementedInterfaces();

            builder
                .RegisterType<RabbitMqPublisher>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(RabbitMqPublisherConfiguration),
                    (_, __) => _configuration.ModifiedPublisherConfiguration)
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(System.Type),
                    (_, __) => typeof(PersonModified))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder
                .RegisterType<ProcessorMessageAdded>()        
                .Named<IProcessorMessage>("ProcessorMessageAdded")        
                .SingleInstance()
                .AsImplementedInterfaces();

             builder
                .RegisterType<ProcessorMessageModified>()        
                .Named<IProcessorMessage>("ProcessorMessageModified")        
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.Register(ctx => new RabbitMqConsumer(
                    _configuration.AddedConsumerConfiguration, 
                ctx.ResolveNamed<IProcessorMessage>("ProcessorMessageAdded")))
                .SingleInstance();

            builder.Register(ctx => new RabbitMqConsumer(
                    _configuration.ModifiedConsumerConfiguration, 
                ctx.ResolveNamed<IProcessorMessage>("ProcessorMessageModified")))
                .SingleInstance();         

        }
    }
}