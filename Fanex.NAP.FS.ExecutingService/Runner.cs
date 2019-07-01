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
    using Fanex.NAP.FS.ExecutingService.Schedules;
    using System.Collections.Generic;

    public partial class Runner : ServiceBase
    {
        private const string ProjectName = "Fraud Sensor Service";
        private const string GroupName = "FraudCaseGroup";
        private List<ISchedulesFS> schedulesFS;

        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                schedulesFS = new List<ISchedulesFS>();

                ExecuteJobs();

                LogService.Info($"Executing service is STARTED");
            }
            catch (Exception ex)
            {
                LogService.Info($"Executing service is START error message: { ex.ToString() } at {DateTime.Now}");
            }
        }

        protected override void OnStop()
        {
            try
            {
                schedulesFS.ForEach(x => x.Shutdown());

                LogService.Info($"Executing service is STOP at {DateTime.Now}");
            }
            catch (Exception ex)
            {

                LogService.Info($"Executing service is STOP error message: { ex.ToString() } at {DateTime.Now}");
            }
        }

        private void ExecuteJobs()
        {
            var fraudCaseService = new FraudCaseService();

            var fraudCases = fraudCaseService.GetFraudCase<FraudCase>(null, 1, Core.AppSetting.Environment);

            foreach (var fraudCase in fraudCases)
            {
                if (!fraudCase.IsActive)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId)))
                {
                    continue;
                }

                var job = new JobFS();
                var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);
                var className = string.Format("{0}Job", fraudCaseName);
                var jobName = className;
                var triggerName = string.Format("{0}Trigger", fraudCaseName);

                job.CreateJob(className, jobName, GroupName)
                    .CreateTrigger(triggerName, fraudCase.FraudCaseInterval)
                    .CreateSchedule()
                    .Run();
            }
        }
    }
}