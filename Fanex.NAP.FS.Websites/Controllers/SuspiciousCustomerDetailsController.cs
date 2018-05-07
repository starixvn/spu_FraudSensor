namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using Fanex.NAP.FS.Library.Services;
    using System;
    using System.Web.Mvc;

    public class SuspiciousCustomerDetailsController : ReportsController
    {
        private readonly ISuspiciousCustomerReportDetailsService _detailsService = new SuspiciousCustomerReportDetailsService();

        public ActionResult Index(int custId, string custName, DateTime from, DateTime to)
        {
            return View();
        }

        public ActionResult GetSummary(int custid, DateTime from, DateTime to)
        {
            var json = new { data = _detailsService.GetSummaries(custid, from, to) };

            return Json(json);
        }

        public ActionResult GetChoice(int custid, DateTime from, DateTime to)
        {
            var json = new
            {
                data = _detailsService.GetChoices(custid, from, to)
            };

            return Json(json);
        }

        public ActionResult GetTop5Sports(int custid, DateTime from, DateTime to)
        {
            var json = new
            {
                data = _detailsService.GetTop5Sports(custid, from ,to)
            };

            return Json(json);
        }

        public ActionResult GetTop5Bettypes(int custid, DateTime from, DateTime to)
        {
            {
                var json = new
                {
                    data = _detailsService.GetTop5Bettypes(custid, from ,to)
                };

                return Json(json);
            }
        }
    }
}