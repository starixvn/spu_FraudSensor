namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.ExcelConfig.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.ScannedOfficeIP, Policy = Core.Business.Constant.Policy.Read)]
    public class ScannedOfficeIpReportController : ReportsController
    {
        private readonly IScannedOfficeIpReportService _service = new ScannedOfficeIpReport();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTickets(DateTime fromDate, DateTime toDate)
        {
            return Json(_service.GetTickets(fromDate, toDate));
        }

        public ActionResult ExportExcel(DateTime fromDate, DateTime toDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/Scanned Office Ip/template.xlsx");

            var file = RenderExcelFile(
                template,
                wb => new OfficeTicketConfig().RenderExcel(
                    wb,
                    new TicketFromOfficeIpGetCriteria { FromDate = fromDate, ToDate = toDate }, _service.GetRawTickets(fromDate, toDate), null));

            return ExcelFile(file, $"OfficeIP_{DateTime.Today.ToString("yyyyMMdd")}");
        }
    }
}