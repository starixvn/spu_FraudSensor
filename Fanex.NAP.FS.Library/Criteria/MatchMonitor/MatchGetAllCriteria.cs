namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class MatchGetAllCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public bool? IsVerified { get; set; }

        public string MatchStatus { get; set; }

        public string LeagueIds { get; set; }

        public bool IsMainMarket { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}