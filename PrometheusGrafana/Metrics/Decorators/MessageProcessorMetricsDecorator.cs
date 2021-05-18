using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prometheus;
using PrometheusGrafana.Metrics;
using PrometheusGrafana.RabbitMq;

namespace PrometheusGrafana.Configuration
{
    public class MessageProcessorMetricsDecorator : IProcessorMessage
    {
        private Histogram _durationHistogram;
        private Counter _counter;
        private IProcessorMessage _decoratee;
        private Type _type;

        public MessageProcessorMetricsDecorator(IProcessorMessage decoratee, Type type, string messageMetricName, double[] durationHistogramBuckets)
        {
            _decoratee = decoratee;
            _type = type;
            _durationHistogram = MetricsHelper.CreateHistogram(durationHistogramBuckets, GetHistogramLabelNames().ToArray(), messageMetricName,
                "duration_seconds", "process");

            _counter  = MetricsHelper.CreateCounter(GetCounterLabelNames().ToArray(), messageMetricName,
                "total", "process");            
        }

        public async Task ProcessAsync(byte[] body)
        {
            using (GetHistogramObserver().NewTimer())
            {
                try
                {
                    await _decoratee.ProcessAsync(body);
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
            yield return _type.GetType().Name;
        }

        private IEnumerable<string> GetCounterLabelNames()
        {
            yield return "outcome";
        }

        private IObserver GetHistogramObserver()
        {
            return _durationHistogram.WithLabels(_type.GetType().Name);
        }

        public Counter.Child GetCounter(bool success) =>
            _counter.WithLabels(GetCounterLabelValues(success).ToArray());

        private IEnumerable<string> GetCounterLabelValues(bool success)
        {
            yield return success ? "success" : "failure";
        }
    }
}