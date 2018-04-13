namespace Fanex.NAP.FS.Library.Criteria.FraudCase
{
    using Fanex.Data.Repository;

    public class GetIsActiveCriteria : CriteriaBase
    {
        public int FraudCaseId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_GetById";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}