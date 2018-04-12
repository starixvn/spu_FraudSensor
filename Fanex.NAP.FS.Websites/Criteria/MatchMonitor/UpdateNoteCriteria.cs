namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;

    public class UpdateNoteCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string Note { get; set; }

        public override string GetSettingKey()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }
    }
}