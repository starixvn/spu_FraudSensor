namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Business;
    using Core.Service.Impl;
    using Core.Web.Attribute;
    using Library.Criteria;
    using Library.DTO;
    using Library.Services;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Constant.Policy.Read)]
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

        private static List<Core.Models.User> GetUsers()
        {
            var siteId = int.Parse(ConfigurationManager.AppSettings["SiteId"]);
            var users = new UserService().GetUserPolicies(FSFunctionKeys.MatchMonitoring, siteId);
            var result = new List<Core.Models.User>();

            foreach (var user in users)
            {
                if (user.Policy != (decimal)Constant.Policy.Read && user.Policy != (decimal)Constant.Policy.Host)
                {
                    continue;
                }

                result.Add(user.User);
            }

            return result;
        }
    }
}