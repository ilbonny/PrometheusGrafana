using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PrometheusGrafana.RabbitMq
{
    public interface IRabbitMqConsumer
    {
        void Start(IConnection connection);
        void Stop();
    }

    public class RabbitMqConsumer : IRabbitMqConsumer
    {
        private readonly RabbitMqConsumerConfiguration _configuration;
        private readonly IProcessorMessage _processorMessage;
        private IModel _channel;

        public RabbitMqConsumer(RabbitMqConsumerConfiguration configuration, IProcessorMessage processorMessage)
        {
            _configuration = configuration;
            _processorMessage = processorMessage;
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

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += ConsumerOnReceived;
            channel.BasicConsume(_configuration.QueueName, autoAck: false, consumer: consumer);
            return channel;
        }

        private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs evt)
        {
            var body = evt.Body.ToArray();
            await _processorMessage.ProcessAsync(body);

             if (_channel.IsOpen)
                _channel.BasicAck(evt.DeliveryTag, multiple: false);
        }
    }
}
