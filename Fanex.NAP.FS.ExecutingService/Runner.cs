namespace Fanex.NAP.FS.ExecutingService
{
    using Fanex.Logging;
    using System;
    using System.ServiceProcess;    
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.ExecutingService.Models;
    using Fanex.NAP.FS.Core;    

    public partial class Runner : ServiceBase
    {
        private const string GroupName = "FraudCaseGroup";
        private JobsMap jobsMap;

        public Runner()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                jobsMap = GetJobsMap();

                if (jobsMap == null)
                {
                    LogService.Info($"Executing service is error message: JobsMap null");

                    return;
                }

                ExecuteJobs();

                LogService.Info($"Executing service is STARTED");
            }
            catch (Exception ex)
            {
                LogService.Error($"Executing service is START error message: { ex.ToString() } at {DateTime.Now}");
            }
        }

        protected override void OnStop()
        {
            LogService.Info($"Executing service is STOP at {DateTime.Now}");
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
                var mapping = GetJobData(fraudCase.FraudCaseId.ToString());

                job.CreateJob(className, jobName, GroupName, mapping)
                    .CreateTrigger(triggerName, fraudCase.FraudCaseInterval)
                    .CreateSchedule()
                    .CreateListenerJob()
                    .Run();
            }
        }

        private JobsMap GetJobsMap()
        {
            try
            {
                var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + @"App_Data\JobDataExecuting.xml";
                return Serializer.Deserialize<JobsMap>(xmlPath);
            }
            catch (Exception ex)
            {
                LogService.Info($"Executing service - GetDataMap error message: { ex.ToString() } at {DateTime.Now}");
            }

            return new JobsMap();
        }

        private string GetJobData(string id)
        {
            var item = jobsMap.JobDataExecutingModel.Find(c => c.Id.Equals(id));

            if (item != null)
            {
                return item.Path;
            }

            return null;
        }
    }
}