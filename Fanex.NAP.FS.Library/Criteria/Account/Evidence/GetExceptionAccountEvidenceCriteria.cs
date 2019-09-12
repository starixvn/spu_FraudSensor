namespace Fanex.NAP.FS.Library.Criteria.Account.Evidence
{
    using Fanex.Data.Repository;

    public class GetExceptionAccountEvidenceCriteria : CriteriaBase
    {
        public int ip_AccountId { get; set; }
   
        public int ip_UserId { get; set; }

        public bool ip_IsMaster { get; set; }
     
        public override string GetSettingKey() => "FPS_DC_Rpt_GetExceptionByAccountId";

        public override bool IsValid() => true;
    }
}