using PrometheusGrafana.RabbitMq.Models;

namespace PrometheusGrafana.RabbitMq
{
    public interface IMessageBus
    {
        void Start();
    }

    public class MessageBus : IMessageBus
    {
        private readonly IRabbitMqConsumer<PersonAdded> _addConsumer;
        private readonly IRabbitMqConsumer<PersonModified> _modifiedConsumer;

        public MessageBus(IRabbitMqConsumer<PersonAdded> addConsumer, 
                IRabbitMqConsumer<PersonModified> modifiedConsumer)
        {
            _addConsumer = addConsumer;
            _modifiedConsumer = modifiedConsumer;
        }

        public void Start()
        {
            _addConsumer.Consume();
            _modifiedConsumer.Consume();
        }
    }
}