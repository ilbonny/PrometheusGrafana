namespace PrometheusGrafana.RabbitMq
{
    public interface IPublisher<T>
    {
        void Publish(T message);
    }
    
    public class Publisher<T> : IPublisher<T>
    {
        private readonly IRabbitMqPublisher _publisher;

        public Publisher(IRabbitMqPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Publish(T message)
        {
            _publisher.Publish(message);
        }
    }
}