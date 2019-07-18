namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class VerifyMatchCriteria : CommandBase
    {
        public bool IsVerified { get; set; }

        public int UserId { get; set; }

        public int MatchId { get; set; }

        public int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitorVerification_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}