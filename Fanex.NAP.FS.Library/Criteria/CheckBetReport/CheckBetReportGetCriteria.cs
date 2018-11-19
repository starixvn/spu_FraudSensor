namespace Fanex.NAP.FS.Library.Criteria.CheckBetReport
{
    using Fanex.Data.Repository;
    using System;

    public class CheckBetReportGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public override string GetSettingKey() => "FS_Rpt_CheckBet_Get";

        public override bool IsValid() => true;
    }
}