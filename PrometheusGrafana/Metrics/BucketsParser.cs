using System.Globalization;
using System.Linq;

namespace PrometheusGrafana.Metrics
{
    public static class BucketsParser
    {
        public static double[] Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return text
                .Split(',')
                .Select(s => double.Parse(s, CultureInfo.InvariantCulture))
                .ToArray();
        }
    }
}