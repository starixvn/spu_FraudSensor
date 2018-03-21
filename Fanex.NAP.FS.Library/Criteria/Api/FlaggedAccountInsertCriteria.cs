namespace Fanex.NAP.FS.Library.Criteria
{
    using Fanex.Data.Repository;

    public class FlaggedAccountInsertCriteria : CriteriaBase
    {
        public string EviCode { get; set; }

        public string DirectAccounts { get; set; }

        public string IndirectAccounts { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FlaggedAccount_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(EviCode);
        }
    }
}