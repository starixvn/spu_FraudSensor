namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Fanex.Data.Repository;
    using System;

    public class GetTicketCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public int FraudCaseId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_Details_Get_ByTicket";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}