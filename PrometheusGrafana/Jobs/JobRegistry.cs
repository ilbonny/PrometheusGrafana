using System;
using System.Collections.Generic;
using System.Linq;
using FluentScheduler;

namespace PrometheusGrafana.Jobs
{
     public interface ISchedulableJob : IJob
    {
        string Name { get; }

        TimeSpan RunInterval { get; }

        void Cancel();
    }
    
    public class JobRegistry : Registry
    {
        private readonly List<ISchedulableJob> _registeredJobs;

        public JobRegistry(IEnumerable<ISchedulableJob> registeredJobs)
        {
            NonReentrantAsDefault();
            _registeredJobs = registeredJobs.ToList();

            _registeredJobs.ForEach(job => Schedule(job)
                           .WithName(job.Name)
                           .ToRunEvery((int)job.RunInterval.TotalMilliseconds)
                           .Milliseconds());
        }

        public void CancelAllJobs() => _registeredJobs.ForEach(job => job.Cancel());
    }
}