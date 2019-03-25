namespace Fanex.NAP.FS.Library.Criteria.FraudCase.ScannedOfficeIP
{    
    using Fanex.Data.Repository;
    public class OfficeIPInternalAccountDeleteCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey()
        {
            return "FS_OfficeIPInternalAccount_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
