namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.ExcelConfig.CheckBetReport;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Web.Mvc;

    public class CheckBetReportController : ReportsController
    {
        private readonly ICheckBetReportService _service = new CheckBetReportService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateReport(CheckBetReportGetCriteria criteria)
        {
            return Json(_service.Generate(criteria));
        }

        public ActionResult GenerateExcel(CheckBetReportGetCriteria criteria)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/Check Bet Report/template.xlsx");

            var fileName = GetExcelReportName("CheckBetReport");

            var data = _service.Generate(criteria);

            var content = RenderExcelFile(template, wb => new CheckBetReportExcelConfig().RenderExcel(wb, criteria, data, null));

            return ExcelFile(content, fileName);
        }
    }
}