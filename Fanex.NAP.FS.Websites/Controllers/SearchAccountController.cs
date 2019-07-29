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

        public ActionResult GetSubscriberAccountDetail(int accountID)
        {
            var data = _searchAccountService.GetSubscriberAccountDetail(accountID);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}