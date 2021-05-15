using PrometheusGrafana.Models;

namespace PrometheusGrafana.RabbitMq
{
    public interface IPersonPublisher 
    {
    }

    public class PersonAddPublisher : IPersonPublisher
    {
        private readonly IRabbitMqPublisher<Person> _publisher;

        public PersonAddPublisher(IRabbitMqPublisher<Person> publisher)
        {
            _publisher = publisher;
        }

        public void Publish(Person entity)
        {
            _publisher.Publish(entity);
        }
    }
}