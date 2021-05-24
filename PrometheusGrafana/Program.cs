using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using PrometheusGrafana.Configuration;
using System;
using Serilog;
using System.Threading.Tasks;
using ILogger = PrometheusGrafana.Configuration.ILogger;

namespace PrometheusGrafana
{
    public class Program
    {
        private static ILogger _logger;

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder().Build();
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(ReadConfigAndConfigureIoC))
                .ConfigureServices((hostContext, services) => { services.AddHostedService<RootService>(); })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog();

        private static void ReadConfigAndConfigureIoC(ContainerBuilder containerBuilder)
        {
            var config = ConfigurationReader.Read();
            ContainerBuilderConfigurator.Configure(containerBuilder, config);
        }

         private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) =>
            LogException(e.ExceptionObject, "UnhandledException");

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) =>
            LogException(e.Exception, "UnobservedTaskException");

        private static ILogger GetLogger(IHost host) => host.Services.GetService<ILogger>();

        private static void LogException(object exceptionObj, string from)
        {
            try
            {
                _logger.Error($"({from}) {exceptionObj}");
            }
            catch (Exception loggerException)
            {
                Console.WriteLine($"Unable to log {from}.{Environment.NewLine}{loggerException}");
            }
        }
               
    }
}
