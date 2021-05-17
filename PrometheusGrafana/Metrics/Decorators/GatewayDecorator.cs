using System;
using System.Threading.Tasks;
using Prometheus;
using PrometheusGrafana.Metrics;

namespace PrometheusGrafana.Configuration
{
    public class GatewayDecorator 
    {
        private readonly Histogram _durationHistogram;

        public GatewayDecorator(double[] durationHistogramBuckets)
        {
            _durationHistogram = MetricsHelper.CreateHistogram(durationHistogramBuckets, new[] { "operation" }, "operation",
                "duration_seconds");
        }

        protected async Task<T> ExecuteFunction<T>(Func<Task<T>> func, string operationType)
        {
            using (GetHistogramObserver(operationType).NewTimer())
            {
                return await func.Invoke();
            }
        }

        protected async Task ExecuteAction(Func<Task> action, string operationType)
        {
            using (GetHistogramObserver(operationType).NewTimer())
            {
                await action.Invoke();
            }
        }

        private IObserver GetHistogramObserver(string operationType)
        {
            return _durationHistogram.WithLabels(operationType);
        }
    }
}