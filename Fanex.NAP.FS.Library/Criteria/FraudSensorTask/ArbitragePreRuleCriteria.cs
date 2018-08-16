namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;
    using System;

    public class ArbitragePreRuleCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_Arbitrage_PreRule";

        public override bool IsValid() => true;
    }
}