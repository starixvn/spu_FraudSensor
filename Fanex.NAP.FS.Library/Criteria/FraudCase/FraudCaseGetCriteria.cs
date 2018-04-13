namespace Fanex.NAP.FS.Library.Criteria.FraudCase
{
    using Fanex.Data.Repository;

    public class FraudCaseGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_FraudCase_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}