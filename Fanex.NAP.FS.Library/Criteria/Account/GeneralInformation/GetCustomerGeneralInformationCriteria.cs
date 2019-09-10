namespace Fanex.NAP.FS.Library.Criteria.Account.GeneralInformation
{
    using Fanex.Data.Repository;

    public class GetCustomerGeneralInformationCriteria : CriteriaBase
    {
        public int ip_accountId { get; set; }

        public bool ip_isMaster { get; set; }

        public int ip_userId { get; set; }

        public override string GetSettingKey() => "FPS_DC_GetCustomerGeneralInfo";

        public override bool IsValid() => true;
    }
}