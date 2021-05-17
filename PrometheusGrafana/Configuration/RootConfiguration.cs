namespace PrometheusGrafana.Configuration
{
    public class RootConfiguration
    {
        public string ServiceName { get; set; }
        public MongoConnectionConfiguration MongoConfigurationDb { get; set; }
        public RabbitConfiguration RabbitConfiguration { get; set; }
        public MetricsConfiguration MetricsConfiguration { get; set; }
    }
}