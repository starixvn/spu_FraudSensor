namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class ArbitrageCleanUpCriteria : CriteriaBase
    {
        public string Matches { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_Arbitrage_CleanUp";

        public override bool IsValid() => true;
    }
}