namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Business;
    using Core.Service.Impl;
    using Core.Web.Attribute;
    using Library.Core;
    using Library.Criteria;
    using Library.DTO;
    using Library.ExportExcel;
    using Library.Services;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomer, Policy = Constant.Policy.Read)]
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

        public ActionResult GetUsers()
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

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee)
        {
            var suspiciousCustomers = _suspiciousCustomerReportService.GetSuspiciousCustomerReport(criteria);

            var excelExporter = new ExportExcelSuspiciousCustomer(
                Server.MapPath("~/ExcelFiles"),
                "SuspiciousCustomerReport",
                new List<Legend>
                {
                    new Legend("From Date", criteria.FromDate.ToShortDateString()),
                    new Legend("To Date", criteria.ToDate.ToShortDateString()),
                    new Legend("Member", criteria.Username),
                    new Legend("Fraud Case", fraudCaseName),
                    new Legend("Verified", criteria.IsVerified == null ? "All" : (criteria.IsVerified.Value ? "Yes" : "No")),
                    new Legend("Classification", criteria.Classification == 0 ? "All" : (criteria.Classification == 1 ? "Delinquency" : (criteria.Classification == 2 ? "Dirty" : "Hedge"))),
                    new Legend("Assignees", assignee)
                },
                GetSuspiciousCustomerHeaders(),
                suspiciousCustomers);

            var saveAsFileName = excelExporter.GenerateExcelFile();
            ResponseStreamExcel(excelExporter.Workbook, saveAsFileName);

            return new EmptyResult();
        }

        private static List<string> GetSuspiciousCustomerHeaders()
        {
            var headers = new List<string>();
            headers.Insert(0, "#");
            headers.Insert(1, "Delinquent");
            headers.Insert(2, "Hegde");
            headers.Insert(3, "Dirty");
            headers.Insert(4, "Verified");
            headers.Insert(5, "Member");
            headers.Insert(6, "Evidence");
            headers.Insert(7, "Total Violations");
            headers.Insert(8, "Violated Cases");
            headers.Insert(9, "Bet Count");
            headers.Insert(10, "Assignee");

            return headers;
        }

        private void ResponseStreamExcel(IWorkbook workbook, string fileName)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", fileName));
            workbook.Write(Response.OutputStream);
        }
    }
}