namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using System;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.DailyDataCapture, Policy = Core.Business.Constant.Policy.Read)]
    public class DailyDataCaptureController : ReportsController
    {
        // GET: DailyDataCapture
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMatches(DateTime fromDate, DateTime toDate)
        {
            return null;
        }

        public ActionResult GetMatchesExcel(DateTime fromDate, DateTime toDate)
        {
            return null;
        }
    }
}