using Prometheus;

namespace PrometheusGrafana.Metrics
{
    public static class MetricsHelper
    {
        private const string ApplicationName = "prometheus_grafana";

        public static Histogram CreateHistogram(double[] buckets, string[] labels, string metricName, string suffix, string operationType = "") =>
             Prometheus.Metrics.CreateHistogram(
                BuildMetricName(metricName, suffix, operationType), "",
                new HistogramConfiguration
                {
                    LabelNames = labels,
                    Buckets = buckets
                });

        public static Counter CreateCounter(string[] labels, string metricName, string suffix, string operationType = "") =>
            Prometheus.Metrics.CreateCounter(
                BuildMetricName(metricName, suffix, operationType), "",
                new CounterConfiguration
                {
                    LabelNames = labels
                });

        private static string BuildMetricName(string metricName, string suffix, string operationType = "") =>
            string.IsNullOrEmpty(operationType)
                ? $"{ApplicationName}_{metricName}_{suffix}"
                : $"{ApplicationName}_{metricName}_{operationType}_{suffix}";
    }
}