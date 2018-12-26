namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.CustomerMonitor, Policy = Core.Business.Constant.Policy.Read)]
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

        [HttpPost]
        public ActionResult UpdateNote(UpdateNoteCriteria criteria)
        {
            return Json(service.UpdateNote(criteria));
        }
    }
}