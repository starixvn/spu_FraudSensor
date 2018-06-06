namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class OfficeIPRuleCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_OfficeIP_Rule";

        public override bool IsValid() => true;
    }
}