namespace Fanex.NAP.FS.Library.Criteria.Account.Evidence
{
    using Fanex.Data.Repository;

    public class GetAccountEvidenceCriteria : CriteriaBase
    {
        public int ip_TargetId { get; set; }

        public bool ip_TargetType { get; set; }

        public int ip_UserId { get; set; }

        public bool ip_IsMaster { get; set; }

        public bool ip_IsActiveEvidence { get; set; }

        public override string GetSettingKey() => "FPS_DC_Rpt_ChainToEvidence";

        public override bool IsValid() => true;
    }
}