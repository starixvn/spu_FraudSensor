namespace Fanex.NAP.FS.ExecutingService
{
    using Fanex.Logging;
    using Quartz;
    using Quartz.Impl; 
    using System;
    using System.Collections.Generic;    
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.ExecutingService.Schedules;
    using Fanex.NAP.FS.ExecutingService.Jobs;
    using Fanex.NAP.FS.Service;

    public partial class Runner : ServiceBase
    {
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
                //var scheduler = CreateScheduleJobTest().Result.Start();
                //var scheduler = CreateScheduleJobDemo().Result.Start();                

                var scheduler = GetFraudCase().Result.Start();

                LogService.Info($"Executing service is STARTED at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                LogService.Info($"Executing service is START error message: { ex.ToString() } at {DateTime.Now}");
            }            
        }

        protected override void OnStop()
        {

        }

        private async Task<IScheduler> GetFraudCase()
        {
            var fraudCaseService = new FraudCaseService();

            var fraudCases = fraudCaseService.GetFraudCase<FraudCase>(null, 1, Core.AppSetting.Environment);

            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            foreach (var fraudCase in fraudCases)
            {
                if (!fraudCase.IsActive)
                    continue;

                var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);

                if (string.IsNullOrWhiteSpace(fraudCaseName))
                    continue;

                AddScheduleJob(scheduler, fraudCase);
            }

            return scheduler;
        }

        private async void AddScheduleJob(IScheduler scheduler, FraudCase fraudCase)
        {
            switch (fraudCase.FraudCaseId)
            {
                case (int)FraudCaseId.FlaggedAccount:

                    var schedule = new FlaggedAcocuntSchedule();
                    var job = schedule.CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                    var trigger = schedule.CreateTrigger(fraudCase.FraudCaseName, fraudCase.FraudCaseInterval);

                    var task = await scheduler.ScheduleJob(job, trigger);
                    break;
                default:
                    break;
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
                    .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            return scheduler;
        }

        private async Task<IScheduler> CreateScheduleJobDemo()
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            var job = JobBuilder.Create<FlaggedAccountJob>()
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
                    .StartNow()
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