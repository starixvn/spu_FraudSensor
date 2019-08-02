namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
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
            var data = _accountService.GetAssociatedAccount(accountId);

            return PartialView("DetailInfoAccount/AssociatedAccount", data);
        }

        [HttpPost]
        public ActionResult GetAssociatedDeviceByAccount(int accountId)
        {
            var data = _accountService.GetAssociatedAccount(accountId);

            return PartialView("DetailInfoAccount/AssociatedDevice", data);
        }
        #endregion
    }
}