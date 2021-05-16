using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PrometheusGrafana.RabbitMq;

namespace PrometheusGrafana
{
    public class RootService : IHostedService
    {
        private readonly IMessageBus _messageBus;

        public RootService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("starting...");
    
            _messageBus.Start();
            
            Console.WriteLine("started...");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("stopping...");
    
            Console.WriteLine("stopped...");

            return Task.CompletedTask;
        }        
    }
}