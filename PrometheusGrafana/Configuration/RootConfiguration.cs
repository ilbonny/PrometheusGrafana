namespace PrometheusGrafana.Configuration
{
    public class RootConfiguration
    {
        public string ServiceName { get; set; }
        public MongoConnectionConfiguration MongoConfiguration { get; set; }
        public RabbitConfiguration RabbitConfiguration { get; set; }
        public MetricsConfiguration MetricsConfiguration { get; set; }
        public ApiConfiguration ApiConfiguration { get; set; }
        public JobConfiguration JobConfiguration { get; internal set; }
    }
}