namespace Fanex.NAP.FS.Websites.Controllers
{
    using System;
    using System.Web.Mvc;

    public class SuspiciousCustomerDetailsController : Controller
    {
        public ActionResult Index(int custId, DateTime from, DateTime to)
        {
            return View();
        }
    }
}