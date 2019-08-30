namespace Fanex.NAP.FS.Library.Criteria.Account.GeneralInformation
{
    using Fanex.Data.Repository;

    public class GetAssociationByAccountCriteria : CriteriaBase
    {
        public int accountId { get; set; }

        public override string GetSettingKey() => "FPS_DC_Rpt_GetAssociationByAccount";

        public override bool IsValid() => true;
    }
}