using System;
using Microsoft.Extensions.Configuration;

namespace PrometheusGrafana.Configuration
{
    public static class ConfigurationData
    {
        private static readonly Lazy<IConfigurationRoot> _lazyRoot =
                        new Lazy<IConfigurationRoot>(LoadRoot);

        public static IConfigurationRoot Root => _lazyRoot.Value;

        private static IConfigurationRoot LoadRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}