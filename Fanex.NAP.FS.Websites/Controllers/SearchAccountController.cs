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
    }
}