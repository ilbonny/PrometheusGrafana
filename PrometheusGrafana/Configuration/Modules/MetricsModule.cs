
using System.Collections.Generic;
using System.Linq;
using Autofac;
using PrometheusGrafana.Metrics;
using PrometheusGrafana.MongoDb.Gateways;

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