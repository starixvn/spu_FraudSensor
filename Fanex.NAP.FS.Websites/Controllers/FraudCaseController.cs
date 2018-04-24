namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.FSMQ;
    using Fanex.NAP.FS.Library.Models;
    using Library.Core.Helpers.Xml;
    using Library.Criteria.FraudCase.DelayedGoal;
    using Library.DTO;
    using Library.Services;
    using ViewModels.FraudCase;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Read)]
    public class FraudCaseController : ConfigurationsController
    {
        private readonly string FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT = ConfigurationManager.AppSettings["FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT"];

        private readonly IDelayGoalAccountService _delayGoalAccountService;

        private readonly IFraudCaseService _fraudCaseService;

        public FraudCaseController()
        {
            _delayGoalAccountService = new DelayGoalAccountService();
            _fraudCaseService = new FraudCaseService();
        }

        public ActionResult Index()
        {
            var fraudCaseViewModel = new FraudCaseViewModel();
            fraudCaseViewModel.FraudCaseInfo = Library.Models.FraudCase.FraudCase.GetFraudCase(FraudCaseId.BSNG);
            fraudCaseViewModel.FraudCaseList = _fraudCaseService.Get(null, null, int.Parse(ConfigurationManager.AppSettings["Environment"]));
            fraudCaseViewModel.FraudCaseInfo.IsActive = _fraudCaseService.CheckActiveFraudCase(FraudCaseId.BSNG);

            return View(fraudCaseViewModel);
        }

        [HttpPost]
        public ActionResult Index(int fraudCaseId)
        {
            var caseid = (FraudCaseId)fraudCaseId;
            var fraudCaseViewModel = new FraudCaseViewModel();
            fraudCaseViewModel.FraudCaseInfo = Library.Models.FraudCase.FraudCase.GetFraudCase(caseid);
            fraudCaseViewModel.FraudCaseList = _fraudCaseService.Get(null, null, int.Parse(ConfigurationManager.AppSettings["Environment"]));
            fraudCaseViewModel.FraudCaseInfo.IsActive = _fraudCaseService.CheckActiveFraudCase(caseid);

            return View(fraudCaseViewModel);
        }

        [HttpPost]
        public ActionResult ChangeCaseStatus(int fraudCaseId, bool isActive)
        {
            _fraudCaseService.ChangeFraudCaseStatus((FraudCaseId)fraudCaseId, CurrentSession.UserId);

            var sender = SendingMessageFactory.GetSender(isActive ? SendingType.Activating : SendingType.Deactivating);
            sender.Send(fraudCaseId);

            return Json(new object());
        }

        public ActionResult GetDelayedGoalCustomerList()
        {
            var json = new
            {
                data = _fraudCaseService.GetDelayedGoalCustomers().Select((c, i) => new { rowNum = i + 1, data = c })
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDelayedGoalCustomer(int custId)
        {
            return Json(_fraudCaseService.DeleteDelayedGoalCustomer(custId, CurrentSession.UserId));
        }

        [HttpPost]
        public ActionResult AddDelayedGoalCustomer(string username)
        {
            var dbresult = _fraudCaseService.AddDelayedGoalCustomer(username, CurrentSession.UserId);

            if (dbresult.ErrorCode == 0)
            {
                var manualAccount = dbresult.OutParam["addedcustid"].ToString();
                IEnumerable<CustomerAssociation> customerAssociation = ApiHelper.Post<string, IEnumerable<CustomerAssociation>>(FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT, manualAccount).Result;

                if (customerAssociation.Any())
                {
                    var customerAssociationXML = customerAssociation.ToXmlString();
                    dbresult = _delayGoalAccountService.InsertAccounts(new DelayGoalAssociationAccountInsertCriteria { CustomerAssociationXML = customerAssociationXML });
                }
            }

            return Json(dbresult);
        }

        public ActionResult ExportDelayedGoalCustomer(string searchString)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/FraudCase/DelayedGoal/template.xlsx");

            var fileName = string.Format("DelayGoalCustomerList_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));

            return File(_fraudCaseService.ExportDelayedCustomersToExcel(templatePath, searchString), "application/vnd.ms-excel", fileName);
        }
    }
}