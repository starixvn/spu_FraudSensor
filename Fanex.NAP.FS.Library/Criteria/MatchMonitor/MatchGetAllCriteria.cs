#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;
    using System;
    using System.Configuration;

    public class MatchGetAllCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public bool? IsVerified { get; set; }

        public string MatchStatus { get; set; }

        public string LeagueIds { get; set; }

        public bool IsMainMarket { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }
        
        public int Environment => int.Parse(ConfigurationManager.AppSettings["Environment"]);

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

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static