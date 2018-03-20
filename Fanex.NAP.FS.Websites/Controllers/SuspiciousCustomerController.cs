namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Web.Attribute;
    using Library.Criteria;
    using Library.Services;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
    public class SuspiciousCustomerController : ReportsController
    {
        private readonly ISuspiciousCustomerReportService _suspiciousCustomerReportService;

        public SuspiciousCustomerController()
        {
            _suspiciousCustomerReportService = new SuspiciousCustomerReportService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generate(SuspiciousCustomerReportGetCriteria criteria)
        {
            var suspiciousCustomers = _suspiciousCustomerReportService.GetSuspiciousCustomerReport(criteria);

            return Json(suspiciousCustomers, JsonRequestBehavior.AllowGet);
        }
    }
}