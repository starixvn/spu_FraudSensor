using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    public class VerifyMatchCriteria : CriteriaBase
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