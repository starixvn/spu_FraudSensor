namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class ArbitrageRuleCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

        public string Associations { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_Arbitrage_Rule";

        public override bool IsValid() => true;
    }
}