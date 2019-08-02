namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using System.Web.Mvc;

    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;

        public AccountController()
        {
            _accountService = new AccountService();
            _evidenceService = new EvidenceService();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Detail
        public ActionResult DetailAccount(int id)
        {
            var data = _accountService.GetAccountDetail(id);

            return View("DetailAccount", data);
        }

        [HttpPost]
        public ActionResult GetChainsToEvidence(int accountId)
        {
            var data = _evidenceService.GetChainsToEvidence(accountId);

            return PartialView("DetailInfoAccount/ChainsToEvidence", data);
        }

        [HttpPost]
        public ActionResult AddException(int accountId, int exceptionAccountId, string comment)
        {
            var data = "";

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRetractAndException(int accountId)
        {
            var data = _evidenceService.GetRetractedEvidenceAndException(accountId);

            return PartialView("DetailInfoAccount/RetractAndException", data);
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountByAccount(int accountId)
        {
            return PartialView("Associated/AccountByAccount");
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountByAccountFilter(AssociatedAccountByAccountFilter filter)
        {
            var data = "";

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAssociatedDeviceByAccount(int accountId)
        {
            return PartialView("Associated/AccountByDevice");
        }
        #endregion
    }
}