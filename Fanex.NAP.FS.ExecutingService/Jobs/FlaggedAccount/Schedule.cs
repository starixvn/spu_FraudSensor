using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ExecutingService.Jobs.FlaggedAccount
{
    public class Schedule : ISchedule
    {
        private IScheduler scheduler;
        public ISchedule Create()
        {
            NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };

            var sf = new StdSchedulerFactory(props);
            scheduler = sf.GetScheduler().Result;
            
            var job = JobBuilder.Create<Job>()
              .WithIdentity("FlaggedAccountJob", "FSGroup")
              .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("FlaggedAccountTrigger", "FSGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);

            return this;
        }

        public ISchedule Pause()
        {
            scheduler.PauseAll();

            return this;
        }

        public ISchedule Shutdown()
        {
            scheduler.Shutdown(true);

            return this;
        }

        public ISchedule Start()
        {
            scheduler.Start();

            return this;
        }
    }
}
