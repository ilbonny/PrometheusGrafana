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
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("starting...");
    
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