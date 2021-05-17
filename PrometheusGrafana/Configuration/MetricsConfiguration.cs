using System.Collections.Generic;

namespace PrometheusGrafana.Configuration
{
    public class MetricsConfiguration
    {
        private string _url;

        public string HostName { get; set; }

        public int Port { get; set; }

        public string Url
        {
            get => _url;
            set => _url = NormalizeUrl(value);
        }

        public bool SuppressDefaultMetrics { get; set; }

        public IEnumerable<HistogramConfiguration> Histograms { get; set; } = new List<HistogramConfiguration>();

        public bool Enabled { get; set; }

        private static string NormalizeUrl(string value)
        {
            var withEndingSlash = value.EndsWith('/')
                ? value
                : $"{value}/";

            var withoutStartingSlash = withEndingSlash.StartsWith('/')
                ? withEndingSlash.Substring(1)
                : withEndingSlash;

            return withoutStartingSlash;
        }
    }

    public class HistogramConfiguration
    {
        public string Id { get; set; }
        public double[] Buckets { get; set; }
    }
}