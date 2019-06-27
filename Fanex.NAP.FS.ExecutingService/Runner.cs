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
        private const string ProjectName = "Fraud Sensor Service";

        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                StartFraudCase().Result.Start();
                
                LogService.Info($"Executing service is STARTED at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                LogService.Info($"Executing service is START error message: { ex.ToString() } at {DateTime.Now}");
            }
        }

        protected override void OnStop()
        {
            // TO DO

            LogService.Info($"Executing service is STOP at {DateTime.Now}");
        }

        private static async Task<IScheduler> StartFraudCase()
        {
            var fraudCaseService = new FraudCaseService();

            var fraudCases = fraudCaseService.GetFraudCase<FraudCase>(null, 1, Core.AppSetting.Environment);

            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            foreach (var fraudCase in fraudCases)
            {
                if (!fraudCase.IsActive)
                {
                    continue;
                }
                    
                var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);

                if (string.IsNullOrWhiteSpace(fraudCaseName))
                {
                    continue;
                }

                AddScheduleJob(scheduler, fraudCase);
            }

            return scheduler;
        }

        private static void AddScheduleJob(IScheduler scheduler, FraudCase fraudCase)
        {
            var schedule = new ScheduleJob(scheduler);
            var job = Job.CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
            var trigger = schedule.CreateTrigger(fraudCase.FraudCaseName, fraudCase.FraudCaseInterval);

            schedule.Schedule(job, trigger);
        }
    }
}