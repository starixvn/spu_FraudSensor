namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Business;
    using Core.Web.Attribute;
    using Library.Criteria;
    using Library.DTO;
    using Library.Services;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NAP.Core.Service.Impl;

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

        [HttpGet]
        public ActionResult GetFraudCases()
        {
            var fraudCaseService = new FraudCaseService();
            var fraudCases = fraudCaseService.GetAll();
            if (fraudCases != null && fraudCases.Any())
            {
                fraudCases = fraudCases.Where(t => !t.IsTest);
            }

            fraudCases = fraudCases ?? new List<FraudCase>();

            return Json(fraudCases, JsonRequestBehavior.AllowGet);
        }
    }
}