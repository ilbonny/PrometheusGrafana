using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PrometheusGrafana.Metrics;
using PrometheusGrafana.RabbitMq;

namespace PrometheusGrafana
{
    public class RootService : IHostedService
    {
        private readonly MetricsService _metricsService;
        private readonly IMessageBus _messageBus;

        public RootService(MetricsService metricsService, IMessageBus messageBus)
        {
            _metricsService = metricsService;
            _messageBus = messageBus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("starting...");
    
            _messageBus.Start();
            _metricsService.Start();

            Console.WriteLine("started...");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("stopping...");
            
            _messageBus.Stop();
            _metricsService.Stop();
    
            Console.WriteLine("stopped...");

            return Task.CompletedTask;
        }        
    }
}