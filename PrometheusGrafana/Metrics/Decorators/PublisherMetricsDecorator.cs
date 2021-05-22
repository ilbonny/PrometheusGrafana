using Prometheus;
using PrometheusGrafana.Metrics;
using PrometheusGrafana.RabbitMq;

namespace PrometheusGrafana.Configuration
{
    public class PublisherMetricsDecorator<T> : IPublisher<T>
    {
        private readonly IPublisher<T> _decoratee;
        private readonly Counter _counter;

        public PublisherMetricsDecorator(IPublisher<T> decoratee, string messageMetricsName)
        {
            _decoratee = decoratee;
            _counter = MetricsHelper.CreateCounter(new[] { "type" }, messageMetricsName, "total", "publish");
        }

        public void Publish(T message)
        {
            _decoratee.Publish(message);
             GetCounter(message).Inc();
        }

        public Counter.Child GetCounter(T message)
        {
            return _counter.WithLabels(message.GetType().Name);
        }        
    }
}