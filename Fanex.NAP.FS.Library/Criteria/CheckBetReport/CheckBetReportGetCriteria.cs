namespace Fanex.NAP.FS.Library.Criteria.CheckBetReport
{
    using Fanex.Data.Repository;
    using System;

    public class CheckBetReportGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "";

        public override bool IsValid() => true;
    }
}