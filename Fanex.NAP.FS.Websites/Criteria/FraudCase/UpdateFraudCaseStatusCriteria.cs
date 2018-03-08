using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Websites.Criteria.FraudCase
{
    public class UpdateFraudCaseStatusCriteria : CriteriaBase
    {
        public int FraudCaseId { get; set; }

        public int UserId { get; set; }

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