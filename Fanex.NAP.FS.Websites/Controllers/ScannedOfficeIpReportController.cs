namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
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

        private bool IsShowSpecialCurrencies;

        public ScannedOfficeIpReportController()
        {
            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            IsShowSpecialCurrencies = permission > 0;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTickets(DateTime fromDate, DateTime toDate)
        {
            return Json(_service.GetTickets(fromDate, toDate, IsShowSpecialCurrencies));
        }

        public ActionResult ExportExcel(DateTime fromDate, DateTime toDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/Scanned Office Ip/template.xlsx");

            var file = RenderExcelFile(
                template,
                wb => new OfficeTicketConfig().RenderExcel(
                    wb,
                    new TicketFromOfficeIpGetCriteria { FromDate = fromDate, ToDate = toDate }, _service.GetRawTickets(fromDate, toDate, IsShowSpecialCurrencies), null));

            return ExcelFile(file, $"OfficeIPResult_{DateTime.Today.ToString("yyyyMMdd")}");
        }
    }
}