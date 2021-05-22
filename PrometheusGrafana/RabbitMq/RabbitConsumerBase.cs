using System;
using PrometheusGrafana.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PrometheusGrafana.RabbitMq
{
    public interface IRabbitMqConsumer
    {
        void Start(IConnection connection);
        void Stop();
    }

    public abstract class RabbitConsumerBase : IRabbitMqConsumer
    {
        private readonly RabbitMqConsumerConfiguration _configuration;
        private IModel _channel;

        public RabbitConsumerBase(RabbitMqConsumerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Start(IConnection connection)
        {
            _channel = Configure(connection.CreateModel());
        }

        public void Stop()
        {
            _channel?.Close();
            _channel?.Dispose();
        }

        private IModel Configure(IModel channel)
        {
            channel.QueueDeclare(_configuration.QueueName, durable: true, exclusive: false, autoDelete: false);
            channel.ExchangeDeclare(_configuration.ExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            channel.QueueBind(_configuration.QueueName, _configuration.ExchangeName, "");
            channel.BasicQos(0, 25, global: true);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ConsumerOnReceived;
            channel.BasicConsume(_configuration.QueueName, autoAck: false, consumer: consumer);
            return channel;
        }

        private void ConsumerOnReceived(object sender, BasicDeliverEventArgs evt)
        {
            try
            {
                Process(evt.Body.ToArray());
                TryToAckDelivery(evt.DeliveryTag);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TryToNAckDelivery(evt.DeliveryTag);
            }
        }

        protected abstract void Process(byte[] body);

        private void TryToAckDelivery(ulong deliveryTag)
        {
            if (_channel.IsOpen)
                _channel.BasicAck(deliveryTag, multiple: false);
        }

        private void TryToNAckDelivery(ulong deliveryTag)
        {
            if (_channel.IsOpen)
                _channel.BasicNack(deliveryTag, multiple: false, requeue: true);
        }
    }
}
