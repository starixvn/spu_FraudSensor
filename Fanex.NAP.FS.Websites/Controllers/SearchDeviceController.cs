namespace Fanex.NAP.FS.Websites.Controllers
{
    using System.Web.Mvc;

    public class SearchDeviceController : BaseController
    {
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

            return View("Index");
        }
    }
}