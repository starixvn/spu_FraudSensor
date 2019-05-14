namespace Fanex.NAP.FS.Service
{
    using Fanex.Data.Repository;
    public class CheckBetRuleCommand : NonQueryCommand
    {
        public override string GetSettingKey() => "FS_FraudCase_CheckBet_Rule";

        public override bool IsValid() => true;
    }
}
