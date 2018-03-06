namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class FraudCaseGetAllCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_FraudCaseGetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
