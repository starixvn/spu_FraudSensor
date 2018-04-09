namespace Fanex.NAP.FS.WinService
{
    using Library.DTO;
    using System;
    using System.Configuration;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Library.Services;
    using System.Linq;
    using Library.Core.Helpers.Api;
    using System.Collections.Generic;
    using Library.Criteria;
    using Library.Core.Helpers.Xml;

    public class FPSGetAssociationAccountExecutor : FraudCaseExecutor
    {
        private readonly string FPS_WEBAPI = ConfigurationManager.AppSettings["FPS_WEBAPI"];

        private IHttpWebService _webService;

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
                IEnumerable<CustomerAssociation> customerAssociation =  ApiHelper.Post<CustomerList, IEnumerable<CustomerAssociation>>(FPS_WEBAPI, listManualAccounts).Result;
                var customerAssociationXML = "<root>" + string.Join("", customerAssociation.Select(l => l.ToXmlString())) + "</root>";
                
                var result = _delayGoalAccountService.InsertAccounts(new DelayGoalAssociationAccountInsertCriteria { CustomerAssociationXML = customerAssociationXML });
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
        #endregion
    }
}
