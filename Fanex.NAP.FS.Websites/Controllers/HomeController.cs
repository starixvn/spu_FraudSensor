namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Web;
    using Fanex.NAP.Core.Web.Session;
    using Newtonsoft.Json;
    using System.Web.Mvc;
    using System.Web.Security;

    //[NapSuperAuth]
    public class HomeController : BaseController
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext != null && !CheckSessionAuthorization())
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new HttpUnauthorizedResult("<center><b>" + ViewResources.UnauthorizeMsg + "</b></center>");
                }
                else
                {
                    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                }
            }           
        }

        protected bool CheckSessionAuthorization()
        {
            ICurrentSession currentSession = new CurrentSession();
            if (currentSession == null || string.IsNullOrEmpty(currentSession.Username) || currentSession.UserId <= 0)
            {
                return false;
            }

            return true;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu(string activeMenu)
        {
            ViewBag.ActiveMenu = activeMenu;
            
            return PartialView("~/Views/Shared/_Menu.cshtml", CurrentSession.MenuItemList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}