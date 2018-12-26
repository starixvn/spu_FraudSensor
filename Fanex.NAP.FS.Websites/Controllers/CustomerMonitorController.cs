namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Configuration;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.CustomerMonitor, Policy = Core.Business.Constant.Policy.Read)]
    public class CustomerMonitorController : ReportsController
    {
        readonly ICustomerMonitorService service = new CustomerMonitorService();

        private readonly bool isSpecialCurrency;

        public CustomerMonitorController()
        {
            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            isSpecialCurrency = permission > 0;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCustomers(CustomerGetCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = isSpecialCurrency;
            criteria.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);

            return Json(service.GetCustomers(criteria));
        }

        [HttpPost]
        public ActionResult UpdateNote(UpdateNoteCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
    
            return Json(service.UpdateNote(criteria));
        }
    }
}