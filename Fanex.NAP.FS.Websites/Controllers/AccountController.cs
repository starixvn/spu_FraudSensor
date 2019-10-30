namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor;
    using System.Text;
    using System.Web.Mvc;

    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;

        public AccountController()
        {
            accountService = new AccountService();
        }

        #region Associated Account Monitor
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult AssociatedAccountsMonitor()
        {
            var evidencesCache = new StringBuilder();
            var evidences = accountService.GetAllEvidences();
            
            foreach (var evidence in evidences)
            {
                evidencesCache.Append(evidence.EvidenceCode + ":" + evidence.EvidenceName + ";");
            }
            
            ViewBag.Evidences = evidences;            
            ViewBag.EvidencesCache = evidencesCache.ToString();
            ViewBag.Subscribers = accountService.GetAllSubscribers();
            ViewBag.Username = this.User.Identity.Name;

            return View("AssociatedToMonitor/AssociatedAccountsToMonitor");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetAssociatedAccountsToMonitor(AssociatedAccountsToMonitorFilter filter)
        {
            filter.AssociatedDateFrom = DateTimeExtension.ConvertToFirstTimeInDay(filter.AssociatedDateFrom);
            filter.AssociatedDateTo = DateTimeExtension.ConvertToLastTimeInDay(filter.AssociatedDateTo);

            var data = accountService.GetAssociatedAccountsToMonitor(filter);

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetAssociatedAccountsDetailToMonitor(AssociatedAccountsDetailToMonitorFilter filter)
        {
            filter.AssociatedDateFrom = DateTimeExtension.ConvertToFirstTimeInDay(filter.AssociatedDateFrom);
            filter.AssociatedDateTo = DateTimeExtension.ConvertToLastTimeInDay(filter.AssociatedDateTo);

            var data = accountService.GetAssociatedAccountsDetailToMonitor(filter);

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult UpdateAssociatedAccountToMonitorOfVerify(AssociatedAccountToMonitorOfVerify accountOfVerify)
        {
            accountOfVerify.UserId = CurrentSession.UserId;

            var data = accountService.UpdateAssociatedAccountToMonitorOfVerify(accountOfVerify);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult UpdateAssociatedAccountToMonitorOfNote(AssociatedAccountToMonitorOfNote accountOfNote)
        {
            var data = accountService.UpdateAssociatedAccountToMonitorOfNote(accountOfNote);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}