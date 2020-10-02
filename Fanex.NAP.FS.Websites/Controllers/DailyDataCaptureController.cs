namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.DailyDataCapture;
    using Fanex.NAP.FS.Library.ExcelConfig.DailyDataCapture;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.DailyDataMonitor;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.DailyDataCapture, Policy = Core.Business.Constant.Policy.Read)]
    public class DailyDataCaptureController : ReportsController
    {
        private readonly IDailyDataCaptureService _service = new DailyCaptureDataService();

        // GET: DailyDataCapture
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMatches(DateTime fromDate, DateTime toDate)
        {
            //var result = _service.GetMatches(new DailyDataGetCriteria { FromDate = fromDate, ToDate = toDate });

            var result = GenerateMatchesForTest();

            return Json(result);
        }

        private List<CapturedMatch> GenerateMatchesForTest()
        {
            var matches = new List<CapturedMatch>();

            matches.Add(new CapturedMatch
            {
                MatchId = 1,
                MatchCode = "1",
                League = "1",
                Home = "1",
                Away = "1",
                EventDate = "1",
                ScannedDate = "1",
                CompanyTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                SuperTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                MasterTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                AgentTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                Tickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                SettledTickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                CancelledTickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                Note = "1"
            });
            matches.Add(new CapturedMatch
            {
                MatchId = 2,
                MatchCode = "2",
                League = "2",
                Home = "2",
                Away = "2",
                EventDate = "2",
                ScannedDate = "2",
                CompanyTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                SuperTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                MasterTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                AgentTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                Tickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                SettledTickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                CancelledTickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                Note = "2"
            });
            matches.Add(new CapturedMatch
            {
                MatchId = 3,
                MatchCode = "3",
                League = "3",
                Home = "3",
                Away = "3",
                EventDate = "2",
                ScannedDate = "2",
                CompanyTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                SuperTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                MasterTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                AgentTO = new Library.ViewModels.History<double?> { Old = 1, New = 2 },
                Tickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                SettledTickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                CancelledTickets = new Library.ViewModels.History<int?> { Old = 1, New = 2 },
                Note = "2"
            });

            return matches;
        }

        public ActionResult UpdateDailyDataCaptureOfNote(DailyDataCaptureOfNote dataCaptureOfNote)
        {
            dataCaptureOfNote.UserId = CurrentSession.UserId;

            var data = _service.UpdateDailyDataCaptureOfNote(dataCaptureOfNote);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMatchesExcel(DateTime fromDate, DateTime toDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/DailyDataCapture/template.xlsx");

            var criteria = new DailyDataGetCriteria { FromDate = fromDate, ToDate = toDate };

            var file = RenderExcelFile(template, wb => new DailyDataCaptureExcelConfig().RenderExcel(wb, criteria, _service.GetMatches(criteria), null));

            return ExcelFile(file, GetExcelReportName("DailyDataMonitor"));
        }
    }
}