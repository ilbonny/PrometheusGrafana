using System;
using FluentScheduler;

namespace PrometheusGrafana.Jobs
{
    public interface IJobService
    {
        void Start();
        void Stop();
    }

    public class JobService : IJobService
    {
        private readonly JobRegistry _jobRegistry;

        public JobService(JobRegistry jobRegistry)
        {
            _jobRegistry = jobRegistry;
        }

        public void Start()
        {
            JobManager.Initialize(_jobRegistry);            
        }

        public void Stop()
        {
            JobManager.StopAndBlock();
        }
    }
}