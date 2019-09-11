namespace Fanex.NAP.FS.Library.Criteria.Account.Evidence
{
    using Fanex.Data.Repository;

    public class GetAccountAffectedEvidence : CriteriaBase
    {
        public int targetId { get; set; }

        public bool targetType { get; set; }

        public int userId { get; set; }

        public bool isMaster { get; set; }

        public override string GetSettingKey() => "FPS_DC_Rpt_ChainToEvidence";

        public override bool IsValid() => true;
    }
}