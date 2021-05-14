using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PrometheusGrafana.Configuration;
using PrometheusGrafana.Gateways;

namespace PrometheusGrafana
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                }
            );
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ReadConfigAndIoc(services);            
        }

        private static void ReadConfigAndIoc(IServiceCollection services)
        {
            var config = ConfigurationReader.Read();
            services.AddSingleton<IPersonGateway, PersonGateway>();
            services.AddSingleton<IMongoDb, MongoDb>();
        }
    }
}
