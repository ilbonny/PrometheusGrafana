using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PrometheusGrafana
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder().Build();
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) => { services.AddHostedService<RootService>(); })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
               
    }
}
