namespace PrometheusGrafana.RabbitMq
{
    public class PublisherMessage<T> : IPublisherMessage<T>
    {
        private readonly IRabbitMqPublisher _publisher;

        public PublisherMessage(IRabbitMqPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Publish(T message)
        {
            _publisher.Publish(message);
        }
    }
}