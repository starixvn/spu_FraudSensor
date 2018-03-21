namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class FraudCaseGetByIdCriteria : CriteriaBase
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
