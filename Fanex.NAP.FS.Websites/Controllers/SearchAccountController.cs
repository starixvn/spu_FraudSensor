namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Services;
    using System.Configuration;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SearchAccount, Policy = Core.Business.Constant.Policy.Read)]
    public class SearchAccountController : BaseController
    {
        private readonly ISearchAccountService _searchAccountService;
        public SearchAccountController()
        {
            _searchAccountService = new SearchAccountService();
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
            var data = _searchAccountService.GetSubscribers();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchAccount(string key)
        {
            ViewBag.KeySearch = key;

            return View("Index");
        }       

        public ActionResult SearchSubscriberAccount(string key, string subscribers)
        {
            var data = _searchAccountService.SearchSubscriberAccounts(key, subscribers);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region Detail

        public ActionResult DetailAccount(int id)
        {
            var data = _searchAccountService.GetSubscriberAccountDetail(id);

            return View("Detail", data);
        }

        public ActionResult ChainsToEvidence(string keySearch)
        {
            var model = "ChainsToEvidence Content";

            return PartialView("Detail/ChainsToEvidence", model);
        }

        public ActionResult RetractAndException(string keySearch)
        {
            var model = "RetractAndException Content";

            return PartialView("Detail/RetractAndException", model);
        }

        public ActionResult AssociatedAccount(string keySearch)
        {
            var model = "RetractAndException Content";

            return PartialView("Detail/AssociatedAccount", model);
        }

        public ActionResult AssociatedDevice(string keySearch)
        {
            var model = "RetractAndException Content";

            return PartialView("Detail/AssociatedDevice", model);
        }

        public ActionResult TransactionHistory(string keySearch)
        {
            var model = "RetractAndException Content";

            return PartialView("Detail/TransactionHistory", model);
        }

        #endregion
    }
}