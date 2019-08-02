namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Services;
    using System.Configuration;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SearchAccount, Policy = Core.Business.Constant.Policy.Read)]
    public class SearchAccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;       

        public SearchAccountController()
        {
            _accountService = new AccountService();
            _evidenceService = new EvidenceService();
        }

        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);
            ViewBag.ItemKeySelect = "search-account";
            ViewBag.ItemKeyNonSelect = "search-device";
            ViewBag.OptionSelect = "Search account";
            ViewBag.OptionNonSelect = "Search device";

            return View();
        }

        public ActionResult GetSubscribers()
        {
            var data = _accountService.GetSubscribers();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchAccount(string key)
        {
            ViewBag.KeySearch = key;

            return View("Index");
        }       

        public ActionResult SearchSubscriberAccount(string key, string subscribers)
        {
            var data = _accountService.SearchSubscriberAccounts(key, subscribers);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region Detail

        public ActionResult DetailAccount(int id)
        {
            var data = _accountService.GetSubscriberAccountDetail(id);

            ViewBag.AccountId = id;

            return View("Detail", data);
        }

        [HttpPost]
        public ActionResult GetChainsToEvidence(int accountId)
        {
            var data = _evidenceService.GetChainsToEvidence(accountId);

            return PartialView("Detail/ChainsToEvidence", data);
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

            return PartialView("Detail/RetractAndException", data);
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountByAccount(int accountId)
        {
            var data = _accountService.GetAssociatedAccount(accountId);

            return PartialView("Detail/AssociatedAccount", data);
        }

        [HttpPost]
        public ActionResult GetAssociatedDeviceByAccount(int accountId)
        {
            var data = _accountService.GetAssociatedAccount(accountId);

            return PartialView("Detail/AssociatedDevice", data);
        }
       
        #endregion
    }
}