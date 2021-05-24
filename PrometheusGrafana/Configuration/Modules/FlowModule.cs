using Autofac;
using PrometheusGrafana.Api;
using PrometheusGrafana.Jobs;

namespace PrometheusGrafana.Configuration
{
    public class FlowModule : Module
    {
        private JobConfiguration _retryConfig;
        private readonly ApiConfiguration _apiConfig;

        public FlowModule(JobConfiguration retryConfig, ApiConfiguration apiConfig)
        {
            _retryConfig = retryConfig;
            _apiConfig = apiConfig;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClientHelper>()
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder.RegisterType<PersonGateway>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(ApiConfiguration),
                    (_, __) => _apiConfig)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<JobRegistry>()
                 .AsSelf()
                 .SingleInstance();

            builder.RegisterType<FlowJob>()
                .WithParameter(
                    (pi, _) => pi.ParameterType == typeof(JobConfiguration),
                    (_, __) => _retryConfig)
                .AsImplementedInterfaces()
                .SingleInstance(); 

            builder.RegisterType<JobService>()
                 .AsImplementedInterfaces()
                 .SingleInstance();           
        }
    }
}