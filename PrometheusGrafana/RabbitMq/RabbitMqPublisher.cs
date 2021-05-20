using System.Text;
using Newtonsoft.Json;
using PrometheusGrafana.Configuration;
using RabbitMQ.Client;

namespace PrometheusGrafana.RabbitMq
{
    public interface IRabbitMqPublisher
    {
        void Start(IConnection connection);
        void Stop();
        void Publish<T>(T massage);
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