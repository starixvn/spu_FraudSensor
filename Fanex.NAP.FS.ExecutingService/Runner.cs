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

                CreateSchedulers().Result.Start();

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

        private async Task<IScheduler> CreateSchedulers()
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

                if (string.IsNullOrWhiteSpace(Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId)))
                {
                    continue;
                }
              
                scheduler = CreateSchedule(scheduler, fraudCase);
            }
            
            return scheduler;
        }

        private IScheduler CreateSchedule(IScheduler scheduler, FraudCase fraudCase)
        {
            var job = CreateJob(fraudCase);

            var schedule = new SchedulesFS(scheduler)
                                .CreateTrigger(fraudCase.FraudCaseName, fraudCase.FraudCaseInterval)                                
                                .Schedule(job);

            schedulesFS.Add(schedule);

            return scheduler;
        }

        private IJobDetail CreateJob(FraudCase fraudCase)
        {
            switch (fraudCase.FraudCaseId)
            {
                case (int)FraudCaseId.Arbitrage:
                    return new ArbitrageJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                case (int)FraudCaseId.BetsNearGoal:
                    return new BetNearGoalJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                case (int)FraudCaseId.BSNG:
                    return new BigStakeNearGoalJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                case (int)FraudCaseId.CheckBet:
                    return new CheckBetJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                case (int)FraudCaseId.DailyDataCapture:
                    return new DailyDataCaptureJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                case (int)FraudCaseId.DelayedGoal:
                    return new DelayGoalJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                case (int)FraudCaseId.FlaggedAccount:
                    return new FlaggedAccountJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                case (int)FraudCaseId.ScannedOfficeIP:
                    return new ScannedOfficeIPJob().CreateJob(fraudCase.FraudCaseName, ProjectName, fraudCase.FraudCaseDesc);
                default:
                    return null;
            }
        }
    }
}