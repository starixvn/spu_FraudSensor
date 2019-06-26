namespace Fanex.NAP.FS.ExecutingService
{
    using Fanex.Logging;
    using Quartz;
    using Quartz.Impl; 
    using System;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.ExecutingService.Jobs;
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.FS.ExecutingService.Models;

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

        private void AddScheduleJob(IScheduler scheduler, FraudCase fraudCase)
        {
            switch (fraudCase.FraudCaseId)
            {
                case (int)FraudCaseId.FlaggedAccount:

                    var schedule = new FlaggedAcocuntSchedule(scheduler);
                    var job = schedule.CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                    var trigger = schedule.CreateTrigger(fraudCase.FraudCaseName, fraudCase.FraudCaseInterval);

                    schedule.Schedule(job, trigger);
                    break;
                default:
                    break;
            }
        }

        private async Task<IScheduler> CreateScheduleJobDemo()
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