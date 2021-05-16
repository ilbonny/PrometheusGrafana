using System.Threading.Tasks;
using MongoDB.Driver;
using PrometheusGrafana.Models;

namespace PrometheusGrafana.MongoDb.Gateways
{
    public interface IActionGateway
    {
        public Task<Action> Insert(Action action);     
    }

    public class ActionGateway : IActionGateway
    {
        private readonly IMongoCollection<Action> _collection;
        private const string CollectionName = "Actions";

        public ActionGateway(IMongoConnDatabase mongoDb)
        {
            _collection = mongoDb.Database.GetCollection<Action>(CollectionName);
        }

        public async Task<Action> Insert(Action action)
        {
            await _collection.InsertOneAsync(action);
            return action;
        }        
    }
}