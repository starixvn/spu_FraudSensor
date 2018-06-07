namespace Fanex.NAP.FS.Library.Criteria.DailyDataCapture
{
    using Fanex.Data.Repository;
    using System;

    public class DailyDataGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "FS_Rpt_DailyDataMonitoring_Get";

        public override bool IsValid() => true;
    }
}