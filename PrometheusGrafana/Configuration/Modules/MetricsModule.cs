
using System.Collections.Generic;
using System.Linq;
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
                        GetBuckets(Constants.RealPersonGateway),
                        inner)
                   , fromKey: Constants.RealPersonGateway)
                .SingleInstance();

            builder.RegisterDecorator<IActionGateway>(
                    (ctx, inner) => new ActionGatewayMetricsDecorator(
                        GetBuckets(Constants.RealActionGateway),
                        inner
                    ), fromKey: Constants.RealActionGateway)
                .SingleInstance();

            RegisterPublisherDecorator<PersonAddedMessage>(builder);
            RegisterPublisherDecorator<PersonModifiedMessage>(builder);
            RegisterPublisherDecorator<PersonDeletedMessage>(builder);

            RegisterProcessorDecorator<PersonAddedMessage>(builder);
            RegisterProcessorDecorator<PersonModifiedMessage>(builder);
            RegisterProcessorDecorator<PersonDeletedMessage>(builder);
        }

        private void RegisterPublisherDecorator<T>(ContainerBuilder builder)
        {
            builder.RegisterDecorator<IPublisher<T>>(
               (ctx, inner) => new PublisherMetricsDecorator<T>(
                       inner, Constants.RabbiMqMetricName
                     ), fromKey: Constants.RealPublisher)
                 .SingleInstance();
        }

        private void RegisterProcessorDecorator<T>(ContainerBuilder builder)
        {
            builder.RegisterDecorator<IProcessor<T>>(
                    (ctx, parameters, inner) => new ProcessorMetricsDecorator<T>(
                        inner, GetMessageMetricName<T>(), GetBuckets("processor"))
            );
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