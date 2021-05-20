namespace PrometheusGrafana.RabbitMq
{
    public interface IPublisherMessage<T>
    {
        void Publish(T message);
    }
}