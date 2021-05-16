using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrometheusGrafana.MongoDb.Gateways;
using PrometheusGrafana.RabbitMq.Models;

namespace PrometheusGrafana.RabbitMq
{
    public class ProcessorMessageModified : IProcessorMessage
    {
        private readonly IActionGateway _actionGateway;

        public ProcessorMessageModified(IActionGateway actionGateway)
        {
             _actionGateway = actionGateway;
        }

        public Task ProcessAsync(byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);

            var entity = JsonConvert.DeserializeObject<PersonModified>(message);
            return _actionGateway.Insert(
                new PrometheusGrafana.Models.Action(entity.Id, nameof(PersonAdded)));
        }
    }
}