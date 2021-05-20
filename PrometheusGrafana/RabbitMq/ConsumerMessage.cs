using PrometheusGrafana.Configuration;

namespace PrometheusGrafana.RabbitMq
{
    public class ConsumerMessage<T> : RabbitMqConsumer
    {
        private readonly IProcessorMessage<T> _processor;

        public ConsumerMessage(RabbitMqConsumerConfiguration configuration,
            IProcessorMessage<T> processor)
            : base(configuration)
        {
            _processor = processor;
        }

        protected override void Process(byte[] body)
        {
            _processor.Process(body);
        }
    }
}