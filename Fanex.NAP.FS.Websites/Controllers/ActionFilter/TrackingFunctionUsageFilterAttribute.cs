namespace Fanex.NAP.FS.Websites.Controllers.ActionFilter
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Services.Implement;
    using System;  
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class TrackingFuncUsageFilterAttribute : ActionFilterAttribute
    {
        private readonly IMonitorFuncAccessService _monitorService;

        public TrackingFuncUsageFilterAttribute(SystemId systemId, TrackingFunctionId functionId, string IsSkipTrackingFuncAttr = "false")
        {
            this._monitorService = new MonitorFuncAccessService();
            this.CurrentSession = new CurrentSession();
            this.FunctionId = functionId;
            this.SystemId = systemId;
            this.IsSkipTrackingFuncAttr = IsSkipTrackingFuncAttr;
        }

        public TrackingFunctionId FunctionId { get; set; }

        public ICurrentSession CurrentSession { get; set; }

        public SystemId SystemId { get; set; }

        public string IsSkipTrackingFuncAttr { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (IsSkipTrackingFuncAttr != null && filterContext.ActionParameters.ContainsKey(IsSkipTrackingFuncAttr))
                {
                    var isSkipTracking = filterContext.ActionParameters[IsSkipTrackingFuncAttr] as bool? ?? true;

                    if (isSkipTracking)
                    {
                        return;
                    }   
                }

                _monitorService.StatsAccessmentInsert(SystemId, FunctionId, CurrentSession.UserId);

                base.OnActionExecuting(filterContext);
            }
            catch (Exception ex)
            {
                //var logMessage = new LogMessage(ex.Message)
                //                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                //                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                var logMessage = $"Tracking function usage - Insert Stats Accessment fail - {ex.Message}";

                LogService.Error(logMessage);
            }
        }
    }
}