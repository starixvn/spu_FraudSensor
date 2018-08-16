namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class ArbitrageConditionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "FS_FraudCase_Arbitrage_Rule";

        public override bool IsValid() => true;
    }
}