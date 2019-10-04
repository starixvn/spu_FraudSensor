namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor;
    using System.Web.Mvc;

    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;

        public AccountController()
        {
            accountService = new AccountService();
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult AssociatedAccountsMonitor()
        {
            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Evidences = accountService.GetAllEvidences();

            return View("AssociatedToMonitor/AssociatedAccountsToMonitor");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetAssociatedAccountsToMonitor(AssociatedAccountsToMonitorFilter filter)
        {
            var data = accountService.GetAssociatedAccountsToMonitor(filter);

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        public ActionResult UpdateAssociatedAccountToMonitorOfVerify(AssociatedAccountToMonitorOfVerify accountOfVerify)
        {
            accountOfVerify.UserId = CurrentSession.UserId;

            var data = accountService.UpdateAssociatedAccountToMonitorOfVerify(accountOfVerify);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAssociatedAccountToMonitorOfNote(AssociatedAccountToMonitorOfNote accountOfNote)
        {
            var data = accountService.UpdateAssociatedAccountToMonitorOfNote(accountOfNote);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}