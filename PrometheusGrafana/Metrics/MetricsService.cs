using Prometheus;
using PrometheusGrafana.Configuration;

namespace PrometheusGrafana.Metrics
{
    public class MetricsService
    {
        private readonly MetricsConfiguration _config;
        private MetricServer _server;

        public MetricsService(MetricsConfiguration config)
        {
            _config = config;
        }

        public void Start()
        {
            if (_config.Enabled)
            {
                if (_config.SuppressDefaultMetrics)
                    Prometheus.Metrics.SuppressDefaultMetrics();

                _server = new MetricServer(hostname: _config.HostName, _config.Port, _config.Url);
                _server.Start();
            }
        }

        public void Stop()
        {
            _server?.Stop();
        }
    }

}