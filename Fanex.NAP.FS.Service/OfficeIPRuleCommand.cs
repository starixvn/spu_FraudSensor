namespace Fanex.NAP.FS.Service
{
    using Fanex.Data.Repository;

    public class OfficeIPRuleCommand : NonQueryCommand
    {
        public string MatchIds { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_OfficeIP_Rule";

        public override bool IsValid() => true;
    }
}
