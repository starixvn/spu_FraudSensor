using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Models;
using Fanex.NAP.FS.Websites.Models.FraudCase;
using Fanex.NAP.FS.Websites.Service.FraudCase;
using System.Web.Mvc;

namespace Fanex.NAP.FS.Websites.Controllers
{
    [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Read)]
    public class FraudCaseController : Controller
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
        public ActionResult ChangeCaseStatus(int fraudCaseId)
        {
            return Json(new object());
        }
    }
}