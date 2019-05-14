namespace Fanex.NAP.FS.Service
{
    using Fanex.Data.Repository;

    public class FlaggedAccountRuleCommand : NonQueryCommand
    {
        public string MatchIds { get; set; }

        public string FlaggedAccountsXML { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_FlaggedAccount_Rule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
