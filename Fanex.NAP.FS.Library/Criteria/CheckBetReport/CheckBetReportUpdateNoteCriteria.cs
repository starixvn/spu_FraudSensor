namespace Fanex.NAP.FS.Library.Criteria.CheckBetReport
{
    using Fanex.Data.Repository;
    using System;

    public class CheckBetReportUpdateNoteCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string Note { get; set; }
      
        public override string GetSettingKey() => "FS_Rpt_CheckBet_UpdateNote";

        public override bool IsValid() => true;
    }
}
