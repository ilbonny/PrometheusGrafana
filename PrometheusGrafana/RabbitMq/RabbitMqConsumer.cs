using System;
using System.Text;
using Newtonsoft.Json;
using PrometheusGrafana.Configuration;
using PrometheusGrafana.MongoDb.Gateways;
using PrometheusGrafana.RabbitMq.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PrometheusGrafana.RabbitMq
{
    public interface IRabbitMqConsumer<T> where T : IPersonId
    {
        void Consume();
    }

    public class RabbitMqConsumer<T> : IRabbitMqConsumer<T> where T : IPersonId
    {
        private readonly string _queueName;
        private readonly RabbitConnectionConfiguration _configuration;
        private readonly IActionGateway _actionGateway;

        public RabbitMqConsumer(RabbitConnectionConfiguration configuration,
            string queueName, IActionGateway actionGateway)
        {
            _configuration = configuration;
            _queueName = queueName;
            _actionGateway = actionGateway;

            CreateQueue();
        }

        public void Consume()
        {
            using (var connection = OpenConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    var entity = JsonConvert.DeserializeObject<T>(message);
                    _actionGateway.Insert(new PrometheusGrafana.Models.Action(entity.Id, nameof(T)));
                    
                };

                channel.BasicConsume(queue: _queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
        }


        private void CreateQueue()
        {
            using (var connection = OpenConnection())
            using (var channel = connection.CreateModel())
                channel.QueueDeclare(_queueName, false, false, false, null);
        }

        private IConnection OpenConnection()
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