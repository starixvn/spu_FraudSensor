namespace Fanex.NAP.FS.ExecutingService
{
    using Fanex.Logging;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Util;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.ExecutingService.Schedules;
    using Fanex.NAP.FS.ExecutingService.Jobs;
    using Fanex.NAP.FS.Service;

    public partial class Runner : ServiceBase
    {
        private static List<ISchedule> schedulers;
        //private readonly LetstalkService _letstalkService;
        private const string ProjectName = "Fraud Sensor Service";

        public Runner()
        {
            InitializeComponent();
           
            //_letstalkService = new LetstalkService();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                CreateScheduleJobTest().Result.Start();
                //CreateScheduleJob();
                //GetFraudCase();
            }
            catch (Exception ex)
            {
            }

            LogService.Info($"Executing service is STARTED at {DateTime.Now}");
        }

        protected override void OnStop()
        {

        }

        private async void GetFraudCase()
        {
            var fraudCaseService = new FraudCaseService();

            var fraudCases = fraudCaseService.GetFraudCase<FraudCase>(null, 1, Core.AppSetting.Environment);

            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            foreach (var fraudCase in fraudCases)
            {
                if (!fraudCase.IsActive)
                    continue;

                //var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);

                //if (string.IsNullOrWhiteSpace(fraudCaseName))
                //    continue;

                AddScheduleJob(scheduler, fraudCase);
            }
        }

        private async void AddScheduleJob(IScheduler scheduler, FraudCase fraudCase)
        {
            switch (fraudCase.FraudCaseId)
            {
                case (int)FraudCaseId.FlaggedAccount:

                    var schedule = new FlaggedAcocuntSchedule();
                    var job = schedule.CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                    var trigger = schedule.CreateTrigger(fraudCase.FraudCaseName, fraudCase.FraudCaseInterval);

                    await scheduler.ScheduleJob(job, trigger);

                    break;
                default:
                    break;
            }
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

    public class FraudCase
    {
        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public string FraudCaseDesc { get; set; }
      
        public int FraudCaseInterval { get; set; }

        public bool IsActive { get; set; }

        public bool IsTest { get; set; }
    }

    public enum FraudCaseId
    {
        BSNG = 1,
        FlaggedAccount = 6,
        DelayedGoal = 8,
        ScannedOfficeIP = 2,
        DailyDataCapture = 4,
        CheckBet = 3,
        Arbitrage = 10,
        BetsNearGoal = 11
    }
}