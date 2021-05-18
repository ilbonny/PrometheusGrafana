using System;
using Prometheus;
using PrometheusGrafana.Metrics;
using PrometheusGrafana.RabbitMq;
using RabbitMQ.Client;

namespace PrometheusGrafana.Configuration
{
    public class MessagePublisherMetricsDecorator : IRabbitMqPublisher
    {
        private readonly IRabbitMqPublisher _decoratee;
        private readonly Counter _counter;

        public MessagePublisherMetricsDecorator(string messageMetricsName, IRabbitMqPublisher decoratee)
        {
            _decoratee = decoratee;
            _counter = MetricsHelper.CreateCounter(new[] { "type" }, messageMetricsName, "total", "publish");
        }

        public Type Type => _decoratee.Type;

        public void Publish<T>(T entity)
        {
            _decoratee.Publish(entity);
            GetCounter<T>(entity).Inc();
        }

        public Counter.Child GetCounter<T>(T entity)
        {
            return _counter.WithLabels(typeof(T).Name);
        }

        public void Start(IConnection connection)
        {
            _decoratee.Start(connection);
        }

        public void Stop()
        {
            _decoratee.Stop();
        }
    }
}