namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class FraudCaseGetAllCriteria : CriteriaBase
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
