namespace Fanex.NAP.FS.WinService
{
    using Library.DTO;
    using Library.Services;
    using Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FraudCaseManager : IFraudCaseManager
    {
        private readonly IFraudCaseService _fraudCaseService;

        public FraudCaseManager()
        {
            FraudCaseExecutors = new List<FraudCaseExecutor>();
            _fraudCaseService = new FraudCaseService();
        }

        public IList<FraudCaseExecutor> FraudCaseExecutors { get; set; }

        public void AddCase(int fraudCaseId)
        {
            var fraudCase = _fraudCaseService.GetByFraudCaseId(fraudCaseId);

            AddCase(fraudCase);
        }

        public void ClearCases()
        {
            foreach (var executor in FraudCaseExecutors)
            {
                executor.Stop();
            }

            FraudCaseExecutors.Clear();
        }

        public void RemoveCase(int fraudCaseId)
        {
            try
            {
                var executor = FraudCaseExecutors.SingleOrDefault(x => x.FraudCase.FraudCaseId == fraudCaseId);

                if (executor == null)
                {
                    return;
                }

                executor.Stop();

                FraudCaseExecutors.Remove(executor);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
        }

        public void UpdateCase(int fraudCaseId)
        {
            RemoveCase(fraudCaseId);

            AddCase(fraudCaseId);
        }

        private void AddCase(FraudCase fraudCase)
        {
            Logger.Log.Info("SPU Fraud Sensor Service - " + fraudCase.FraudCaseName + " is " + fraudCase.IsActive + " and Existing(" + Exist(fraudCase) + ")");

            if (!fraudCase.IsActive || Exist(fraudCase))
            {
                return;
            }

            var executor = FraudCaseFactory.GetExecutor(fraudCase);

            FraudCaseExecutors.Add(executor);

            executor.Start();
        }

        #region private method

        private bool Exist(FraudCase fraudCase)
        {
            return FraudCaseExecutors.Any(x => x.FraudCase.FraudCaseId == fraudCase.FraudCaseId);
        }

        private static void WriteLog(Exception exception)
        {
            var message = "\r\nError:" + exception.Message +
                          "\r\nStackTrace: " + exception.StackTrace;

            Logger.Log.Error(message);
        }

        #endregion private method
    }
}