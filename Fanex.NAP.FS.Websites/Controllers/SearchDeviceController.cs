namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using System.Configuration;
    using System.Web.Mvc;

    public class SearchDeviceController : BaseController
    {    
        public override string BaseControllerName { get => Consts.MenuItemSearch; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchDevice(string key)
        {
            ViewBag.KeySearch = key;            
            ViewBag.ItemKeySelect = "search-account";
            ViewBag.ItemKeyNonSelect = "search-device";
            ViewBag.OptionSelect = "Search device";
            ViewBag.OptionNonSelect = "Search account";

            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);
            ViewBag.ItemKeySelect = Consts.SearchDeviceKey;
            ViewBag.ItemKeyNonSelect = Consts.SearchAccountKey;
            ViewBag.OptionSelect = Consts.SearchDeviceOption;            
            ViewBag.OptionNonSelect = Consts.SearchAccountOption;

            return View("Index");
        }
    }
}