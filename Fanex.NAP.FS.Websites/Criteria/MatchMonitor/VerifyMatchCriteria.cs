using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    public class VerifyMatchCriteria : CriteriaBase
    {
        public bool IsVerified { get; set; }

        public int Verifier { get; set; }

        public int MatchId { get; set; }

        public override string GetSettingKey()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}