namespace Fanex.NAP.FS.Websites.Controllers.ActionFilter
{
    using Fanex.Logging;   
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Services.Implement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class TrackingFuncUsageFilterAttribute : ActionFilterAttribute
    {
        private readonly IMonitorFuncAccessService _monitorService;

        public TrackingFuncUsageFilterAttribute(SystemId systemId,
            TrackingFunctionId functionId)
        {
            this._monitorService = new MonitorFuncAccessService();
            this.CurrentSession = new CurrentSession();
            this.FunctionId = functionId;
            this.SystemId = systemId;           
            this.IsSkipTrackingFuncAttr = IsSkipTrackingFuncAttr;
        }

        public TrackingFuncUsageFilterAttribute(SystemId systemId, 
            TrackingFunctionId functionId,
            string attKey,
            params string[] attValues)
        {
            this._monitorService = new MonitorFuncAccessService();
            this.CurrentSession = new CurrentSession();
            this.FunctionId = functionId;
            this.SystemId = systemId;
            this.AttKey = attKey;
            this.AttValues = attValues;
            this.IsSkipTrackingFuncAttr = IsSkipTrackingFuncAttr;
        }

        public TrackingFunctionId FunctionId { get; set; }

        public ICurrentSession CurrentSession { get; set; }

        public SystemId SystemId { get; set; }

        public string AttKey { get; set; }

        public string[] AttValues  { get; set; }

        public string IsSkipTrackingFuncAttr { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.AttKey) && this.AttValues.Any())
                {                    
                    var key = filterContext.Controller.ValueProvider.GetValue(this.AttKey).AttemptedValue;

                    var attValues = this.AttValues.Select(s => s.Split('=')).ToDictionary(d => d[0], d => d[1]);

                    foreach (var keyValue in attValues)
                    {
                        if (keyValue.Value == key)
                        {                            
                            Enum.TryParse(keyValue.Key, out TrackingFunctionId newFunctionId);

                            this.FunctionId = newFunctionId;

                            break;
                        }  
                    }
                }

                _monitorService.StatsAccessmentInsert(SystemId, FunctionId, CurrentSession.UserId);

                base.OnActionExecuting(filterContext);
            }
            catch (Exception ex)
            {
                //var logMessage = new LogMessage($"Tracking function usage - Insert Stats Accessment fail - {ex.Message}")
                //                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                //                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();
                var logMessage = $"Tracking function usage - Insert Stats Accessment fail - {ex.Message}";
                LogService.Error(logMessage);
            }
        }
    }
}