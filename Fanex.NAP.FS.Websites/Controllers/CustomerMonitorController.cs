namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.FS.Library.ExcelConfig.CustomerMonitor;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.FS.Websites.Controllers.ActionFilter;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.CustomerMonitor, Policy = Core.Business.Constant.Policy.Read)]
    public class CustomerMonitorController : ReportsController
    {
        readonly ICustomerMonitorService _customerMonitorService = new CustomerMonitorService();

        private readonly bool isSpecialCurrency;

        public CustomerMonitorController()
        {
            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            isSpecialCurrency = permission > 0;
        }

        public ActionResult Index()
        {
            return View(_customerMonitorService.GetBalanceUpDate());
        }        

        [HttpPost]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.CustomerMonitor_Get)]
        public ActionResult GetCustomers(CustomerGetCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = isSpecialCurrency;
            criteria.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);            

            return Json(_customerMonitorService.GetCustomers(criteria));
        }

        [HttpPost]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.CustomerMonitor_UpdateNote)]
        public ActionResult UpdateNote(UpdateNoteCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
    
            return Json(_customerMonitorService.UpdateNote(criteria));
        }

        public ActionResult GetCustomerTicketDetails(CustomerDetailsInfo model)
        {
            return View("CustomerDetails/Index", model);
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.CustomerMonitor_Detail_GetTicket)]
        public ActionResult GetTicketDetails(GetCustomerTicketsCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = isSpecialCurrency;
            criteria.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);

            var data =
                _customerMonitorService.GetTicketDetails(criteria, true)
                .GroupBy(t => t.LeagueMatch)
                .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ExportExcel(CustomerGetCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = isSpecialCurrency;
            criteria.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);

            var template = Server.MapPath("~/App_data/Excel Template/CustomerMonitor/template.xlsx");

            var fileName = GetExcelReportName("CustomerMonitor");

            var data = _customerMonitorService.GetCustomers(criteria);

            var content = RenderExcelFile(template, wb => new CustomerMonitorConfig().RenderExcel(wb, criteria, data, null));

            return ExcelFile(content, fileName);
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.CustomerMonitor_Detail_ExportToExcel)]
        public ActionResult ExportExcelForDetails(GetCustomerTicketsCriteria criteria, CustomerDetailsInfo detailsInfo)
        {
            /*criteria.IsShowSpecialCurrencies = isSpecialCurrency;
            criteria.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);

            var template = Server.MapPath("~/App_Data/Excel Template/CustomerMonitor/CustomerDetails/template.xlsx");

            var fileName = string.Format("CustomerMonitorDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));

            var data = _customerMonitorService.GetTicketDetails(criteria, true);

            var content = RenderExcelFile(template, wb => new CustomerMonitorTicketDetailConfig(data.Count()).RenderExcel(wb, detailsInfo, data, null));

            return ExcelFile(content, fileName);
            */
            criteria.IsShowSpecialCurrencies = isSpecialCurrency;
            criteria.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);
            var template = Server.MapPath("~/App_Data/Excel Template/CustomerMonitor/CustomerDetails/template.xlsx");

            var fileName = string.Format("CustomerMonitorDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));
            return File(_customerMonitorService.ExportExcel(criteria, detailsInfo, template), "application/vnd.ms-excel", fileName);
        }
    }
}