using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Service.MatchMonitor;
using System;
using System.Web.Mvc;

namespace Fanex.NAP.FS.Websites.Controllers
{
    public class MatchMonitorController : BaseController
    {
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;

            return View(MatchMonitorService.GetBalanceUpDate());
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetMatches(MatchGetAllCriteria criteria)
        {
            return Json(MatchMonitorService.GetMatches(criteria));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetLeagues(LeagueGetCriteria criteria)
        {
            return Json(MatchMonitorService.GetLeagues(criteria));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult VerifyMatch(VerifyMatchCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;

            return Json(MatchMonitorService.VerifyMatch(criteria));
        }

        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult ExportExcel(MatchGetAllCriteria criteria)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/template.xlsx");

            var fileName = DateTime.Now.ToString("MMM-d-yyyy") + ".xlsx";

            return File(MatchMonitorService.ExportExcel(criteria, templatePath), "application/vnd.ms-excel", fileName);
        }
    }
}