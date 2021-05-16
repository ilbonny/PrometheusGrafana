using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace PrometheusGrafana.RabbitMq
{
    public interface IRabbitMqPublisher
    {
        void Start(IConnection connection);
        void Stop();
    }

    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly RabbitMqPublisherConfiguration _configuration;
        private IModel _channel;

        public RabbitMqPublisher(RabbitMqPublisherConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Start(IConnection connection)
        {
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(_configuration.ExchangeName, ExchangeType.Fanout, true);
            _channel.QueueDeclare(_configuration.QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(_configuration.QueueName, _configuration.ExchangeName, "");
        }

        public void Stop()
        {
            if (_channel == null) return;
            _channel.Close();
            _channel.Dispose();
            _channel = null;
        }

        public void Publish<T>(T entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: _configuration.ExchangeName, routingKey: "",
                    basicProperties: null, body: body);
        }
    }    
}