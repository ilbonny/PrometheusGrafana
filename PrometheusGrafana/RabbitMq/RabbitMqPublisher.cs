using System;
using System.Text;
using Newtonsoft.Json;
using PrometheusGrafana.Configuration;
using RabbitMQ.Client;

namespace PrometheusGrafana.RabbitMq
{
    public interface IRabbitMqPublisher<T>
    {
        void Publish(T entity);
    }

    public class RabbitMqPublisher<T> : IRabbitMqPublisher<T>
    {
        private readonly RabbitConnectionConfiguration _configuration;
        private readonly string _exchangeName;
        private readonly string _queueName;

        public RabbitMqPublisher(RabbitConnectionConfiguration configuration,
            string exchangeName, string queueName)
        {
            _configuration = configuration;
            _exchangeName = exchangeName;
            _queueName = queueName;

            CreateExchangeAndQueue();
        }

        public void Publish(T entity)
        {
            using (var connection = OpenConnection())
            using (var channel = connection.CreateModel())
            {
                var json = JsonConvert.SerializeObject(entity);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: _exchangeName, routingKey: "",
                    basicProperties: null, body: body);
            }
        }

        private void CreateExchangeAndQueue()
        {
            using (var connection = OpenConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout);
                channel.QueueDeclare(_queueName, false, false, false, null);
                channel.QueueBind(_queueName, _exchangeName, "", null);
            }
        }

        public IConnection OpenConnection()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration.Uri),
                UserName = _configuration.Username,
                Password = _configuration.Password
            };

            return factory.CreateConnection();
        }        
    }
}