namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Services;
    using System.Configuration;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SearchAccount, Policy = Core.Business.Constant.Policy.Read)]
    public class SearchAccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public SearchAccountController()
        {
            _accountService = new AccountService();
            _userService = new UserService();
        }

        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);
            ViewBag.ItemKeySelect = Consts.SearchAccountItemKeySelect;
            ViewBag.ItemKeyNonSelect = Consts.SearchAccountItemKeyNonSelect;
            ViewBag.OptionSelect = Consts.SearchAccountOptionSelect;
            ViewBag.OptionNonSelect = Consts.SearchAccountOptionNonSelect;

            return View();
        }

        public ActionResult GetSubscribers()
        {
            var user = _userService.Get(this.User.Identity.Name);

            if (user != null && _userService.HasAnyPermission(user.Id, SiteId, Environment))
            {
                var data = _accountService.GetSubscribers(user.Id);

                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchAccount(string key)
        {
            ViewBag.KeySearch = key;

            return View("Index");
        }

        [HttpPost]
        public ActionResult SearchSubscriberAccount(string key, string subscribers)
        {
            var data = _accountService.SearchAccounts(key, subscribers);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}