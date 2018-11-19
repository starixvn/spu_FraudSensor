#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Business;
    using Core.Service.Impl;
    using Core.Web.Attribute;
    using Library.Criteria.SuspiciousCustomer;
    using Library.Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomer, Policy = Constant.Policy.Read)]
    public class SuspiciousCustomerController : ReportsController
    {
        private readonly ISuspiciousCustomerReportService _suspiciousCustomerReportService;

        private bool IsShowSpecialCurrencies;

        public SuspiciousCustomerController()
        {
            _suspiciousCustomerReportService = new SuspiciousCustomerReportService();
            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            IsShowSpecialCurrencies = permission > 0;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generate(SuspiciousCustomerReportGetCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var suspiciousCustomers = _suspiciousCustomerReportService.GetSuspiciousCustomerReport(criteria);

            return Json(suspiciousCustomers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFraudCases()
        {
            var fraudCaseService = new FraudCaseService();
            var fraudCases = fraudCaseService.Get(0, null, int.Parse(ConfigurationManager.AppSettings["Environment"]));

            return Json(fraudCases, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetUsers()
        {
            var siteId = int.Parse(ConfigurationManager.AppSettings["SiteId"]);
            var users = new UserService().GetUserPolicies(FSFunctionKeys.SuspiciousCustomer, siteId);
            var result = new List<Core.Models.User>();

            foreach (var user in users)
            {
                if (user.Policy != (decimal)Constant.Policy.Read && user.Policy != (decimal)Constant.Policy.Host)
                {
                    continue;
                }

                result.Add(user.User);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var file = _suspiciousCustomerReportService.ExportExcel(criteria, fraudCaseName, assignee, Server.MapPath("~/App_Data/Excel Template/SuspiciousCustomer/template.xlsx"));
            var fileName = string.Format("SuspiciousCustomer_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd"));

            return File(file, "application/vnd.ms-excel", fileName);
        }

        public ActionResult GetSuspiciousCustomerNote(int custId)
        {
            return Json(_suspiciousCustomerReportService.GetNoteViews(custId), JsonRequestBehavior.AllowGet);
        }
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously