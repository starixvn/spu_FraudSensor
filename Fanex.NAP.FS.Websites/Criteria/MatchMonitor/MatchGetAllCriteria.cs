using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    public class MatchGetAllCriteria : CriteriaBase
    {
        private readonly string _sp = "FS_Rpt_MatchMonitorReportGet";

        public MatchGetAllCriteria() { }

        public MatchGetAllCriteria(string sp)
        {
            _sp = "FS_Rpt_MatchMonitorReportFromArchiveGet";
        }

        public DateTime EventDate { get; set; }

        public bool? IsVerified { get; set; }

        public string MatchStatus { get; set; }

        public string LeagueIds { get; set; }

        public bool IsMainMarket { get; set; }

        public override string GetSettingKey()
        {
            return _sp;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}