namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Web.Attribute;
    using Newtonsoft.Json;
    using System.Web.Mvc;

    [NapSuperAuth]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu(string activeMenu)
        {
            ViewBag.ActiveMenu = activeMenu;

            LogService.Info($"Test Login FS CurrentSession.MenuItemList: {JsonConvert.SerializeObject(CurrentSession.MenuItemList)}");

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