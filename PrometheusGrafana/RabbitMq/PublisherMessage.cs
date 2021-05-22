namespace PrometheusGrafana.RabbitMq
{
    public interface IPublisherMessage<T>
    {
        void Publish(T message);
    }
    
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