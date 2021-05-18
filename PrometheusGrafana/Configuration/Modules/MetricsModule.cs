
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using PrometheusGrafana.Metrics;
using PrometheusGrafana.MongoDb.Gateways;
using PrometheusGrafana.RabbitMq;
using PrometheusGrafana.RabbitMq.Models;

namespace PrometheusGrafana.Configuration
{
    public class MetricsModule : Module
    {
        private readonly MetricsConfiguration _config;

        public MetricsModule(MetricsConfiguration config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MetricsService>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(MetricsConfiguration),
                    (_, __) => _config);

            builder.RegisterDecorator<IPersonGateway>(
                    (ctx, inner) => new PersonGatewayMetricsDecorator(
                        GetBuckets("personGateway"),
                        inner)
                   , fromKey: "RealPersonGateway")

                .SingleInstance();

            builder.RegisterDecorator<IActionGateway>(
                    (ctx, inner) => new ActionGatewayMetricsDecorator(
                        GetBuckets("actionGateway"),
                        inner
                    ), fromKey: "RealActionGateway")
                .SingleInstance();

            builder.RegisterDecorator<IRabbitMqPublisher>(
                    (ctx, inner) => new MessagePublisherMetricsDecorator(
                        "rabbit",
                        inner
                    ), fromKey: "AddedPublisherConfiguration")
                .SingleInstance();
            
            builder.RegisterDecorator<IRabbitMqPublisher>(
                    (ctx, inner) => new MessagePublisherMetricsDecorator(
                        "rabbit",
                        inner
                    ), fromKey: "ModifiedPublisherConfiguration")
                .SingleInstance();
            
            builder.RegisterDecorator<IRabbitMqPublisher>(
                    (ctx, inner) => new MessagePublisherMetricsDecorator(
                        "rabbit",
                        inner
                    ), fromKey: "DeletedPublisherConfiguration")
                .SingleInstance();

            builder.RegisterDecorator<IProcessorMessage>(
                    (ctx, inner) => new Decorator(
                        inner
                    ), fromKey: "ProcessorMessageAdded")
                .SingleInstance();

            // builder.RegisterDecorator<IProcessorMessage>(
            //         (ctx, inner) => new MessageProcessorMetricsDecorator(
            //             inner,
            //             typeof(PersonAdded),
            //             "processor",
            //             GetBuckets("rabbit_processor")
            //         ), fromKey: "ProcessorMessageAdded")
            //     .SingleInstance();

            // builder.RegisterDecorator<IProcessorMessage>(
            //         (ctx, inner) => new MessageProcessorMetricsDecorator(
            //             inner,
            //             typeof(PersonModified),
            //             "processor",
            //             GetBuckets("rabbit_processor")
            //         ), fromKey: "ProcessorMessageModified")
            //     .SingleInstance();

            // builder.RegisterDecorator<IProcessorMessage>(
            //         (ctx, inner) => new MessageProcessorMetricsDecorator(
            //             inner,
            //             typeof(PersonDeleted),
            //             "processor",
            //             GetBuckets("rabbit_processor")
            //         ), fromKey: "ProcessorMessageDeleted")
            //     .SingleInstance();
        }

        private static string GetMessageMetricName<TMessage>() =>
           typeof(TMessage).Name.ToLowerInvariant();

        private double[] GetBuckets(string id) =>
            HistogramHelper.GetBuckets(_config.Histograms, id);
    }
}

public static class HistogramHelper
{
    public static double[] GetBuckets(IEnumerable<PrometheusGrafana.Configuration.HistogramConfiguration> configs, string histogramConfigId)
    {
        var match = configs.FirstOrDefault(h => h.Id == histogramConfigId);

        return match?.Buckets; // if null is provided, the default buckets will be used by Prometheus
    }
}

public class Decorator : IProcessorMessage
{
        private IProcessorMessage _decoratee;

        public Decorator(IProcessorMessage decoratee)
        {
            _decoratee = decoratee;            
        }

    public Task ProcessAsync(byte[] body)
    {
        return _decoratee.ProcessAsync(body);
    }
}