namespace Fanex.NAP.FS.WinService
{
    using Library.Core.Helpers.Api;
    using Library.Core.Helpers.Xml;
    using Library.Criteria;
    using Library.DTO;
    using Library.Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class FPSGetAssociationAccountExecutor : FraudCaseExecutor
    {
        private readonly string FPS_WEBAPI = ConfigurationManager.AppSettings["FPS_WEBAPI"];

        private readonly IDelayGoalAccountService _delayGoalAccountService;

        public FPSGetAssociationAccountExecutor(FraudCase fraudCase) : base(fraudCase)
        {
            _delayGoalAccountService = new DelayGoalAccountService();
        }

        protected override void ClearOldResult()
        {
        }

        protected override void ExecutePrecondition()
        {
        }

        protected override void ExecuteRule()
        {
            GetAssociationAccounts();
        }

        protected override void TakeNextAction()
        {
        }

        #region private method

        private void GetAssociationAccounts()
        {
            var custIDs = _delayGoalAccountService.GetManualAccounts().ToArray();
            var listManualAccounts = new CustomerList { CustIdList = string.Join(",", custIDs.Select(t => t)) };

            try
            {
                IEnumerable<CustomerAssociation> customerAssociation = ApiHelper.Post<string, IEnumerable<CustomerAssociation>>(FPS_WEBAPI, listManualAccounts.CustIdList).Result;
                var customerAssociationXML = customerAssociation.ToXmlString();

                _delayGoalAccountService.InsertAccounts(new DelayGoalAssociationAccountInsertCriteria { CustomerAssociationXML = customerAssociationXML });
            }
            catch (Exception exception)
            {
                var error = "<b>Error:</b> " + exception.Message +
                          "<br><b>StackTrace:</b> " + exception.StackTrace;
                var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}</b><br><br>", FraudCase.FraudCaseId, FraudCase.FraudCaseName) + error;

                if (_errorTimes++ < 10)
                {
                    SendMailInform(message);
                }
            }
        }

        #endregion private method
    }
}