using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrometheusGrafana.MongoDb.Gateways;
using PrometheusGrafana.RabbitMq.Models;

namespace PrometheusGrafana.RabbitMq
{
    public interface IProcessorMessage<T>
    {
        Task Process(byte[] body);
    }
    
    public class ProcessorMessage<T> : IProcessorMessage<T> 
    {
        private readonly IActionGateway _actionGateway;

        public ProcessorMessage(IActionGateway actionGateway)
        {
             _actionGateway = actionGateway;
        }

        public Task Process(byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);
            var entity = JsonConvert.DeserializeObject<T>(message);

            return _actionGateway.Insert(Map((IMessage)entity));
        }

         public PrometheusGrafana.Models.Action Map(IMessage entity)
         {
             return new PrometheusGrafana.Models.Action(entity.Id
                 , typeof(T).Name);
         }
    }
}