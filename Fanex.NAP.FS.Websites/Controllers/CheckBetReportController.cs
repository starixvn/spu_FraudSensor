namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.ExcelConfig.CheckBetReport;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.CheckBetReport, Policy = Core.Business.Constant.Policy.Read)]
    public class CheckBetReportController : ReportsController
    {
        private readonly ICheckBetReportService _service = new CheckBetReportService();
        private bool IsShowSpecialCurrencies;

        public CheckBetReportController()
        {
            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            IsShowSpecialCurrencies = permission > 0;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateReport(CheckBetReportGetCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            return Json(_service.Generate(criteria));
        }

        public ActionResult UpdateNote(CheckBetReportUpdateNoteCriteria criteria)
        {
            //criteria.UserId = CurrentSession.UserId;

            //return Json(_service.UpdateNote(criteria));

            // result for test
            return Json(new Library.Models.DBResult { ErrorCode = 0 });
        }

        public ActionResult GenerateExcel(CheckBetReportGetCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var template = Server.MapPath("~/App_Data/Excel Template/Check Bet Report/template.xlsx");
            var fileName = GetExcelReportName("CheckBetReport");
            var data = _service.Generate(criteria);
            var content = RenderExcelFile(template, wb => new CheckBetReportExcelConfig().RenderExcel(wb, criteria, data, null));

            return ExcelFile(content, fileName);
        }
    }
}