namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Fanex.Data.Repository;
    using System;

    public class GetScoreHDPCriteria : CriteriaBase
    {
        public int MatchId { get; set; }
       
        public bool IsShowSpecialCurrencies { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_Details_Get_ScoreHDP";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
