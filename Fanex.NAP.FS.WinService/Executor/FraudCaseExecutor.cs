namespace Fanex.NAP.FS.WinService.Executor
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Fanex.NAP.FS.Library.DTO;
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    public abstract class FraudCaseExecutor : IFraudCaseExecutor
    {
        protected FraudCaseExecutor(FraudCase fc)
        {
            FraudCase = fc;
        }

        protected FraudCase FraudCase { get; }

        protected abstract void ExecutePrecondition();

        protected abstract void ExecuteRule();

        protected abstract void ClearOldResult();

        protected abstract void TakeNextAction();

        public void Execute()
        {
            ExecutePrecondition();

            ExecuteRule();

            ClearOldResult();

            TakeNextAction();
        }

        protected void SendMailInform(string message, string subject = null)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var environment = GetEnvironment();
                    var newMail = new MailDelivery.Core.Models.Mail
                    {
                        Subject = string.Format("[({0}) SPU FS] ", environment) + (string.IsNullOrEmpty(subject) ? " Error" : subject),
                        Title = "SPU Fraud Sensor",
                        Body = message,
                        MailingListId = int.Parse(ConfigurationManager.AppSettings["MailingListId"])
                    };

                    MailClientHandler.SendFunctionMail(newMail);
                }
                catch (Exception exception)
                {
                    WriteLog(exception);
                }
            });
        }

        private static string GetEnvironment()
        {
            var environment = string.Empty;
            switch (int.Parse(ConfigurationManager.AppSettings["Environment"]))
            {
                case 1:
                    environment = "LOCAL";
                    break;

                case 2:
                    environment = "UAT";
                    break;

                case 3:
                    environment = "PRO";
                    break;

                default:
                    environment = "UNKNOWN";
                    break;
            }

            return environment;
        }

        protected static void WriteLog(Exception exception)
        {
            var message = "\r\nError: " + exception.Message +
                          "\r\nStackTrace: " + exception.StackTrace;

            Logger.Log.Error(message, exception);
        }
    }
}