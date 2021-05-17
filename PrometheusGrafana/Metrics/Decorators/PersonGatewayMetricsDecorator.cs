using System.Threading.Tasks;
using LanguageExt;
using PrometheusGrafana.Models;
using PrometheusGrafana.MongoDb.Gateways;

namespace PrometheusGrafana.Configuration
{
    public class PersonGatewayMetricsDecorator : GatewayDecorator, IPersonGateway
    {
        private IPersonGateway _decoratee;

        public PersonGatewayMetricsDecorator(double[] durationHistogramBuckets, IPersonGateway decoratee) : base(durationHistogramBuckets)
        {
            _decoratee = decoratee;
        }

        public Task<Option<Person>> Get(string id) =>
            ExecuteFunction(() => _decoratee.Get(id), "mongo_person_get");

        public Task<Person> Insert(Person person) =>
            ExecuteFunction(() => _decoratee.Insert(person), "mongo_person_insert");

        public Task Save(Person person) =>
            ExecuteAction(() => _decoratee.Save(person), "mongo_person_save");

        public Task Delete(string id) =>
            ExecuteAction(() => _decoratee.Delete(id), "mongo_person_delete");
    }
}