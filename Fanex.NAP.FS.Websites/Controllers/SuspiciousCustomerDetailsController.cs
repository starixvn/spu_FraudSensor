#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class SuspiciousCustomerDetailsController : ReportsController
    {
        private readonly ISuspiciousCustomerReportDetailsService _detailsService = new SuspiciousCustomerReportDetailsService();

        public ActionResult Index(int custId, string custName, DateTime from, DateTime to)
        {
            return View();
        }

        public async Task<ActionResult> GetStatistics(int custId, DateTime from, DateTime to)
        {
            return Json(_detailsService.GetStatistics(custId, from, to));
        }

        public async Task<ActionResult> GetViolatedTickets(int custId, DateTime from, DateTime to)
        {
            return JData(_detailsService.GetViolatedTickets(custId, from, to));
        }
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously