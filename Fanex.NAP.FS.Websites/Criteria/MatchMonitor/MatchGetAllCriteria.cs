using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    public class MatchGetAllCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public int Verified { get; set; }

        public string MatchStatus { get; set; }

        public string LeagueIds { get; set; }

        public bool IsMainMarket { get; set; }

        public bool IsTest { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitorReportGet";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}