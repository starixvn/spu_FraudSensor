namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.Logging;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.ExcelConfig.CheckBetReport;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.CheckBetReport;
    using Fanex.NAP.FS.Websites.Controllers.ActionFilter;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.CheckBetRpt_Get)]
        public ActionResult GenerateReport(CheckBetReportGetCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;

            var data = Enumerable.Empty<CheckBetTicket>();

            try
            {
                data = _service.Generate(criteria);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
            
            return Json(data);
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.CheckBetRpt_UpdateNote)]
        public ActionResult UpdateNote(CheckBetReportUpdateNoteCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
         
            return Json(_service.UpdateNote(criteria));
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.CheckBetRpt_ExportToExcel)]
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