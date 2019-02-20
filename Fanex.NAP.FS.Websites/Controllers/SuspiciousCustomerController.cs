#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Business;
    using Core.Service.Impl;
    using Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomer;
    using Library.Criteria.SuspiciousCustomer;
    using Library.Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomer, Policy = Constant.Policy.Read)]
    public class SuspiciousCustomerController : ReportsController
    {
        private readonly ISuspiciousCustomerReportService _suspiciousCustomerReportService;

        private readonly bool IsShowSpecialCurrencies;

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

        [HttpPost]
        public ActionResult Generate(DataTableAjaxPostModel criteria)
        {
            int filteredResultsCount = 0;
            int totalResultsCount = 0;

            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            

            var result = FilterData(criteria, out filteredResultsCount, out totalResultsCount);

            return Json(new
            {
                draw = criteria.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public IList<SuspiciousCustomerReportModel> FilterData(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount)
        {
            var take = model.length;
            var skip = model.start;
            var pageIndex = (skip / take) + 1;

            var searchBy = new SuspiciousCustomerReportGetCriteria
            {
                AssigneeId = model.AssigneeId,
                Classification = model.Classification,
                FraudCaseId = model.FraudCaseId,
                FromDate = model.FromDate,
                IsShowSpecialCurrencies = model.IsShowSpecialCurrencies,
                IsVerified = model.IsVerified,
                ToDate = model.ToDate,
                Username = model.Username,
                PageSize = take,
                PageIndex = pageIndex,
                TotalItems = 0
            };
           
            // search the dbase taking into consideration table sorting and paging
            var result = GetDataFromDbase(searchBy, take, skip, out filteredResultsCount, out totalResultsCount);

            if (result == null)
            {
                // empty collection...
                return new List<SuspiciousCustomerReportModel>();
            }

            return result;
        }

        public List<SuspiciousCustomerReportModel> GetDataFromDbase(SuspiciousCustomerReportGetCriteria searchBy, int take, int skip, out int filteredResultsCount, out int totalResultsCount)
        {
            var suspiciousCustomers = _suspiciousCustomerReportService.GetSuspiciousCustomerReport(searchBy);
            var counter = skip + 1;           

            var result = suspiciousCustomers.Select(x => {
                x.Counter = counter++;
                return x;
            }).ToList();

            filteredResultsCount = searchBy.TotalItems;
            totalResultsCount = searchBy.TotalItems;

            return result;
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
            criteria.IsExportExcel = true;
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