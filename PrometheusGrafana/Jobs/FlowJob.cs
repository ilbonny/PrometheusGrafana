using PrometheusGrafana.Api;
using PrometheusGrafana.Configuration;
using System.Threading.Tasks;
using System;
using PrometheusGrafana.Models;
using System.Collections.Generic;
using System.Linq;

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
            Task.Run(() => InternalExecute());
        }

        private async Task InternalExecute()
        {
            var countCalls = 100;
            var persons = new List<Person>();

            for(var count=0; count<countCalls; count++ ){
                persons.Add(await ExecutePost());
            }

            foreach(var person in persons.Take(countCalls/2)){
                await _gateway.Get(person.Id);
            }

            foreach(var person in persons.Take(countCalls/3)){
                await ExecutePut(person);
            }

            foreach(var person in persons){
                await _gateway.Delete(person.Id);
            }            
        }

        private async Task<Person> ExecutePost()
        {
            var newPerson = await _gateway.Post(new Person
            {
                Name = $"Name {Guid.NewGuid().ToString()}",
                Surname = $"Surname {Guid.NewGuid().ToString()}",
                Age = 20
            });

            return newPerson;
        }

        private Task<bool> ExecutePut(Person person)
        {
            person.Age++;
            return _gateway.Put(person);
        }
    }
}