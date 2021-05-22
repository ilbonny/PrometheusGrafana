using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prometheus;
using PrometheusGrafana.Metrics;
using PrometheusGrafana.RabbitMq;

namespace PrometheusGrafana.Configuration
{
    public class ProcessorMetricsDecorator<T> : IProcessor<T>
    {
        private readonly IProcessor<T> _decoratee;
        private Histogram _durationHistogram;
        private Counter _counter;

        public ProcessorMetricsDecorator(IProcessor<T> decoratee, string messageMetricName, double[] durationHistogramBuckets)
        {
            _decoratee = decoratee;
            _durationHistogram = MetricsHelper.CreateHistogram(durationHistogramBuckets, GetHistogramLabelNames().ToArray(), messageMetricName,
                "duration_seconds", "process");

            _counter = MetricsHelper.CreateCounter(GetCounterLabelNames().ToArray(), messageMetricName,
                "total", "process");
        }

        public async Task Process(byte[] body)
        {
            using (GetHistogramObserver().NewTimer())
            {
                try
                {
                    await _decoratee.Process(body);
                    GetCounter(success: true).Inc();
                }
                catch (Exception)
                {
                    GetCounter(success: false).Inc();
                    throw;
                }
            }
        }

        private IEnumerable<string> GetHistogramLabelNames()
        {
            yield return typeof(T).Name;
        }

        private IEnumerable<string> GetCounterLabelNames()
        {
            yield return "outcome";
        }

        private IObserver GetHistogramObserver()
        {
            return _durationHistogram.WithLabels(typeof(T).Name);
        }

        public Counter.Child GetCounter(bool success) =>
            _counter.WithLabels(GetCounterLabelValues(success).ToArray());

        private IEnumerable<string> GetCounterLabelValues(bool success)
        {
            yield return success ? "success" : "failure";
        }
    }
}