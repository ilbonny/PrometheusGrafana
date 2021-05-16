using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using PrometheusGrafana.Configuration;

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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(ReadConfigAndConfigureIoC))
                .ConfigureServices((hostContext, services) => { services.AddHostedService<RootService>(); })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static void ReadConfigAndConfigureIoC(ContainerBuilder containerBuilder)
        {
            var config = ConfigurationReader.Read();
            ContainerBuilderConfigurator.Configure(containerBuilder, config);
        }
               
    }
}
