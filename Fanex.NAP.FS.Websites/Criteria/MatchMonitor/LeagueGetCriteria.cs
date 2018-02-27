using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    public class LeagueGetCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public int SportId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_League_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}