using System;
using Serilog;

namespace PrometheusGrafana.Configuration
{
    public interface ILogger
    {
        void Info(string message);
        void Error(string message);
        void Debug(string message);
        void Warn(string message);
    }

    public class Logger : ILogger
    {
        private readonly Serilog.Core.Logger _serilog;
        private readonly object _obj = new object();

        public static Logger Create()
        {
            return new Logger(new LoggerConfiguration()
                .ReadFrom.Configuration(ConfigurationData.Root)
                .Enrich.FromLogContext()
                .CreateLogger());
        }

        private Logger(Serilog.Core.Logger serilog)
        {
            _serilog = serilog;
            Log.Logger = serilog;
        }

        public void Info(string message)
        {
            ThreadSafeExecute(() => _serilog.Information(message));
        }

        public void Error(string message)
        {
            ThreadSafeExecute(() => { _serilog.Error(message); });
        }

        public void Debug(string message)
        {
            ThreadSafeExecute(() => _serilog.Debug(message));
        }

        public void Warn(string message)
        {
            ThreadSafeExecute(() => _serilog.Warning(message));
        }

        private void ThreadSafeExecute(Action action)
        {
            lock (_obj)
                action();
        }
    }
}