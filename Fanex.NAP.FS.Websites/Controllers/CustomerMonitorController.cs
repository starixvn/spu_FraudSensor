namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Web.Mvc;

    public class CustomerMonitorController : ReportsController
    {
        readonly ICustomerMonitorService service = new CustomerMonitorService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCustomers(CustomerGetCriteria criteria)
        {
            return Json(service.GetCustomers(criteria));
        }
    }
}