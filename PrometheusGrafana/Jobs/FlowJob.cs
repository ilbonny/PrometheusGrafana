using PrometheusGrafana.Api;
using PrometheusGrafana.Configuration;
using System.Threading.Tasks;
using System;
using PrometheusGrafana.Models;

namespace PrometheusGrafana.Jobs
{
    public class FlowJob : ISchedulableJob
    {
        private readonly IPersonGateway _gateway;
        private readonly JobConfiguration _configuration;

        public FlowJob(IPersonGateway gateway, JobConfiguration configuration)
        {
            _gateway = gateway;
            _configuration = configuration;
        }

        public string Name => "FlowJob";

        public TimeSpan RunInterval => _configuration.Interval;

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            Task.Run(()=>InternalExecute());
        }

        private async Task InternalExecute()
        {
            var newPerson = await _gateway.Post(new Person
            {
                Name = $"Name {Guid.NewGuid().ToString()}",
                Surname = $"Surname {Guid.NewGuid().ToString()}",
                Age = 20
            });

            var getPerson = await _gateway.Get(newPerson.Id);
            getPerson.Age++;

            await _gateway.Put(getPerson);

            await Task.Delay(1000);

            //await _gateway.Delete(getPerson.Id);
        }
    }
}