using System.Threading.Tasks;
using PrometheusGrafana.Models;
using PrometheusGrafana.MongoDb.Gateways;

namespace PrometheusGrafana.Configuration
{
    public class ActionGatewayMetricsDecorator : GatewayDecorator, IActionGateway
    {
        private IActionGateway _decoratee;

        public ActionGatewayMetricsDecorator(double[] durationHistogramBuckets, IActionGateway decoratee) : base(durationHistogramBuckets)
        {
            _decoratee = decoratee;
        }

        public Task<Action> Insert(Action action) =>
            ExecuteFunction(() => _decoratee.Insert(action), "mongo_action_insert");
    }
}