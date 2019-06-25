using Fanex.Logging;
using Fanex.NAP.FS.ExecutingService.Jobs.Dummy;
using Quartz;
using Quartz.Impl;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ExecutingService
{
    public partial class Runner : ServiceBase
    {
        private static List<ISchedule> schedulers;

        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            CreateScheduleJobTest().Result.Start();
            //CreateScheduleJob();

            LogService.Info($"Executing service is STARTED at {DateTime.Now}");
        }

        protected override void OnStop()
        {

        }

        private void CreateScheduleJob()
        {
            schedulers = new List<ISchedule>();

            var assembly = typeof(Program).GetTypeInfo().Assembly;
            var types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (new List<Type>(type.GetInterfaces()).Contains(typeof(ISchedule)))
                {
                    var scheduler = ObjectUtils.InstantiateType<ISchedule>(type);

                    scheduler.Create().Start();

                    schedulers.Add(scheduler);
                }
            }
        }    

        private async Task<IScheduler> CreateScheduleJobTest()
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            var job = JobBuilder.Create<DummyJob>()
                   .WithIdentity("Sales", "REPORTS")
                   .WithDescription("Hello Job!")
                   .StoreDurably()
                   .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("MorningSales")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            return scheduler;
        }
    }
}