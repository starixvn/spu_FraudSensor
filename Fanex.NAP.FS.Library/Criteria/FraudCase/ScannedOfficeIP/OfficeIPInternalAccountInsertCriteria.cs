namespace Fanex.NAP.FS.Library.Criteria.FraudCase.ScannedOfficeIP
{
    using Fanex.Data.Repository;
    public class OfficeIPInternalAccountInsertCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string Username { get; set; }        

        public override string GetSettingKey()
        {
            return "FS_OfficeIPInternalAccount_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
