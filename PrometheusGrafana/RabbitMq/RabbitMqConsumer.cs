using PrometheusGrafana.Configuration;

namespace PrometheusGrafana.RabbitMq
{
    public class RabbitMqConsumer<T> : RabbitConsumerBase
    {
        private readonly IProcessorMessage<T> _processor;

        public RabbitMqConsumer(RabbitMqConsumerConfiguration configuration,
                        IProcessorMessage<T> processor) : base(configuration)
        {
            _processor = processor;
        }

        protected override void Process(byte[] body)
        {
            _processor.Process(body);
        }
    }    
}