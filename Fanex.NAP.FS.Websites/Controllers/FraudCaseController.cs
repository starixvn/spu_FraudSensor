namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.FSMQ;
    using Fanex.NAP.FS.Websites.Models;
    using Fanex.NAP.FS.Websites.Models.FraudCase;
    using Fanex.NAP.FS.Websites.Service.FraudCase;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Read)]
    public class FraudCaseController : ConfigurationsController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int fraudCaseId)
        {
            var caseid = (FraudCaseId)fraudCaseId;

            var fraudCase = FraudCase.GetFraudCase(caseid);

            fraudCase.IsActive = FraudCaseService.CheckActiveFraudCase(caseid);

            return View(fraudCase);
        }

        [HttpPost]
        public ActionResult ChangeCaseStatus(int fraudCaseId, bool isActive)
        {
            FraudCaseService.ChangeFraudCaseStatus((FraudCaseId)fraudCaseId, CurrentSession.UserId);

            var sender = SendingMessageFactory.GetSender(isActive ? SendingType.Activating : SendingType.Deactivating);
            sender.Send(fraudCaseId);

            return Json(new object());
        }

        public ActionResult GetDelayedGoalCustomerList()
        {
            var json = new
            {
                data = FraudCaseService.GetDelayedGoalCustomers().Select((c, i) => new { rowNum = i + 1, data = c })
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDelayedGoalCustomer(int custId)
        {
            return Json(FraudCaseService.DeleteDelayedGoalCustomer(custId, CurrentSession.UserId));
        }

        [HttpPost]
        public ActionResult AddDelayedGoalCustomer(string username)
        {
            return Json(FraudCaseService.AddDelayedGoalCustomer(username, CurrentSession.UserId));
        }

        public ActionResult ExportDelayedGoalCustomer(string searchString)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/FraudCase/DelayedGoal/template.xlsx");

            var fileName = string.Format("DelayGoalCustomerList_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));

            return File(FraudCaseService.ExportDelayedCustomersToExcel(templatePath, searchString), "application/vnd.ms-excel", fileName);
        }
    }
}