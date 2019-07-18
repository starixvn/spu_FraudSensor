namespace Fanex.NAP.FS.Library.Criteria.FraudCase.ScannedOfficeIP
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class OfficeIPInternalAccountInsertCriteria : CommandBase
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
