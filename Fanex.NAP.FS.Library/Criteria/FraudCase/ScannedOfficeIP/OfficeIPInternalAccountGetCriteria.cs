namespace Fanex.NAP.FS.Library.Criteria.FraudCase.ScannedOfficeIP
{
    using Fanex.Data.Repository;
    public class OfficeIPInternalAccountGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_OfficeIPInternalAccount_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}