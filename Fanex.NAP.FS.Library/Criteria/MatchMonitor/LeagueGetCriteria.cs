namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class LeagueGetCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public int SportId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_League_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}