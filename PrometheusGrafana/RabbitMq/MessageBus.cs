using System;
using System.Collections.Generic;
using PrometheusGrafana.Configuration;
using RabbitMQ.Client;

namespace PrometheusGrafana.RabbitMq
{
    public interface IMessageBus
    {
        void Start();
        void Stop();
    }

    public class MessageBus : IMessageBus
    {
        private readonly IEnumerable<IRabbitMqConsumer> _consumers;
        private readonly IEnumerable<IRabbitMqPublisher> _publishers;
        private IConnection _consumersConnection;
        private IConnection _publishersConnection;
        private IConnectionFactory _connectionFactory;

        public MessageBus(RabbitConnectionConfiguration configuration, 
                IEnumerable<IRabbitMqConsumer> consumers, 
                IEnumerable<IRabbitMqPublisher> publishers)
        {
            _consumers = consumers;
            _publishers = publishers;
            _connectionFactory = CreateConnectionFactory(configuration);
        }

        public void Start()
        {
            _consumersConnection = CloseAndOpenNew(_consumersConnection);
            foreach (var consumer in _consumers)
                 consumer.Start(_consumersConnection);            

            _publishersConnection = CloseAndOpenNew(_publishersConnection);
            foreach (var publisher in _publishers)
                 publisher.Start(_publishersConnection);            
        }

        public void Stop()
        {
            foreach (var consumer in _consumers)
                 consumer.Stop();            

            foreach (var publisher in _publishers)
                 publisher.Stop(); 
        }

        private IConnection CloseAndOpenNew(IConnection connection)
        {
            Close(connection);
            var newConnection = _connectionFactory.CreateConnection();
            return newConnection;
        }

        private void Close(IConnection connection)
        {
            if (connection != null)
              if (connection.IsOpen)
                   connection.Close();
        }

        private IConnectionFactory CreateConnectionFactory(RabbitConnectionConfiguration configuration)
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(configuration.Uri),
                UserName = configuration.Username,
                Password = configuration.Password
            };

            return connectionFactory;
        }
    }
}