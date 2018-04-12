namespace Fanex.NAP.FS.Websites.Criteria.FraudCase
{
    using Fanex.Data.Repository;

    public class UpdateFraudCaseStatusCriteria : CriteriaBase
    {
        public int FraudCaseId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_CaseStatus_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}