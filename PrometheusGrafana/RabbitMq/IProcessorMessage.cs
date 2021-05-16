using System.Threading.Tasks;

namespace PrometheusGrafana.RabbitMq
{
    public interface IProcessorMessage
    {
        Task ProcessAsync(byte[] body);
    }
}