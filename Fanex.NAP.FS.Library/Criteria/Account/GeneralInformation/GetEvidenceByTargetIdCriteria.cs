namespace Fanex.NAP.FS.Library.Criteria.Account.GeneralInformation
{
    using Fanex.Data.Repository;

    public class GetEvidenceByTargetIdCriteria : CriteriaBase
    {
        public int targetId { get; set; }

        public bool objType { get; set; }

        public string subIdList { get; set; }

        public bool isMaster { get; set; }

        public override string GetSettingKey() => "FPS_DC_GetEvidenceByTargetId";

        public override bool IsValid() => true;
    }
}