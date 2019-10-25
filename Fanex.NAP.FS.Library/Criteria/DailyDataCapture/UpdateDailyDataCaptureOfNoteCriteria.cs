namespace Fanex.NAP.FS.Library.Criteria.DailyDataCapture
{
    using Fanex.Data.Repository;

    public class UpdateDailyDataCaptureOfNoteCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string MatchIds { get; set; }

        public string Note { get; set; }

        public int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_ChangedMatchNote_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
