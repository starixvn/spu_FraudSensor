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

        public SearchAccountController()
        {
            _accountService = new AccountService();
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
       
    }
}