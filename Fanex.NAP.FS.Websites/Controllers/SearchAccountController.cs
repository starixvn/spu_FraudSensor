namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Services;
    using Fanex.NAP.FS.Library.ViewModels.Account.Search;
    using Fanex.NAP.FS.Service.Utilities;
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

        public override string BaseControllerName { get => Consts.MenuItemSearch;  }

        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);
            ViewBag.ItemKeySelect = Consts.SearchAccountKey;
            ViewBag.ItemKeyNonSelect = Consts.SearchDeviceKey;
            ViewBag.OptionSelect = Consts.SearchAccountOption;
            ViewBag.OptionNonSelect = Consts.SearchDeviceOption;

            return View();
        }

        public ActionResult GetSubscribers()
        {
            var userId = GetIdentityUserId();

            if (userId > 0)
            {
                var data = _accountService.GetSubscribers(userId);

                AppSession.CurrentSubscriberAssignments = data;

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
        public ActionResult SearchSubscriberAccount(SearchAccountFilter filter)
        {
            filter.UserId = GetIdentityUserId();

            var data = _accountService.SearchAccounts(filter);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private int GetIdentityUserId()
        {
            var user = _userService.Get(this.User.Identity.Name);

            if (user != null && _userService.HasAnyPermission(user.Id, SiteId, Environment))
            {
                return user.Id;
            }

            return 0;
        }
    }
}