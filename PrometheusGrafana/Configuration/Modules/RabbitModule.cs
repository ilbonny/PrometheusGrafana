using System.Threading.Tasks;
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
            builder.RegisterType<MessageBus>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(RabbitMqConnectionConfiguration),
                    (_, __) => _configuration.RabbitConnectionConfiguration)
                .SingleInstance()
                .AsImplementedInterfaces();            

            RegisterConsumerMessage<PersonAdded>(builder, _configuration.AddedConsumerConfiguration);
            RegisterConsumerMessage<PersonModified>(builder, _configuration.ModifiedConsumerConfiguration);
            RegisterConsumerMessage<PersonDeleted>(builder, _configuration.DeletedConsumerConfiguration);

            RegisterPublisherMessage<PersonAdded>(builder, _configuration.AddedPublisherConfiguration);
            RegisterPublisherMessage<PersonModified>(builder, _configuration.ModifiedPublisherConfiguration);
            RegisterPublisherMessage<PersonDeleted>(builder, _configuration.DeletedPublisherConfiguration);
        }

        private void RegisterConsumerMessage<T>(ContainerBuilder builder, RabbitMqConsumerConfiguration configuration)
        {
            builder.RegisterType<Processor<T>>()
              .Named<IProcessor<T>>(Constants.RealProcessor)     
              .SingleInstance();            

           builder.Register(ctx => new RabbitMqConsumer<T>(
                configuration,
                ctx.ResolveNamed<IProcessor<T>>(Constants.RealProcessor)))
              .AsImplementedInterfaces()   
              .SingleInstance();   
        }

        private void RegisterPublisherMessage<T>(ContainerBuilder builder, RabbitMqPublisherConfiguration configuration)
        {
            var rabbitMqPublisherName = $"{typeof(T).Name}_publisher";
            builder.RegisterType<RabbitMqPublisher>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(RabbitMqPublisherConfiguration),
                    (_, __) => configuration)
                .Named<IRabbitMqPublisher>(rabbitMqPublisherName)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(ctx => new Publisher<T>(
                ctx.ResolveNamed<IRabbitMqPublisher>(rabbitMqPublisherName)))
              .Named<IPublisher<T>>(Constants.RealPublisher)
              .SingleInstance();
        }
    }

    
}