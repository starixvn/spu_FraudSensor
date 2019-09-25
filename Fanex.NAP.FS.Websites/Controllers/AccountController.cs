namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using System.Web.Mvc;

    public class AccountController : Controller
    {
        private readonly IAccountService accountService;

        public AccountController()
        {
            accountService = new AccountService();
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult AssociatedAccounts()
        {
            ViewBag.Evidences = accountService.GetAllEvidences();

            return View("Associated/AssociatedAccounts");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetAssociatedAccounts(AssociatedAccountsFilter filter)
        {
            var data = accountService.GetAssociatedAccounts(filter);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}