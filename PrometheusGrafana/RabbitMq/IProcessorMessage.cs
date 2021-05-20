using System.Threading.Tasks;

namespace PrometheusGrafana.RabbitMq
{
    public interface IProcessorMessage<T>
    {
        Task Process(byte[] body);
    }
}