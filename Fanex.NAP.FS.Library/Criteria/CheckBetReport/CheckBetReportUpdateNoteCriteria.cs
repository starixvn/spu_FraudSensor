namespace Fanex.NAP.FS.Library.Criteria.CheckBetReport
{
    using Fanex.Data.Repository;    

    public class CheckBetReportUpdateNoteCriteria : CriteriaBase
    {
        public long TransId { get; set; }

        public int UserId { get; set; }

        public string Note { get; set; }

        public override string GetSettingKey() => "FS_Rpt_CheckBetNote_Update";

        public override bool IsValid() => true;
    }
}