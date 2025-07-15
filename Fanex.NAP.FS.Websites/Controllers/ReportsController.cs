#pragma warning disable S1694 // base class for classification

namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using System;

    //[NapSuperAuth]
    public abstract class ReportsController : BaseController
    {
        public string GetExcelReportName(string report)
        {
            return $"{report}_{DateTime.Today.ToString("yyyyMMdd")}";
        }
    }
}

#pragma warning restore S1694 // base class for classification