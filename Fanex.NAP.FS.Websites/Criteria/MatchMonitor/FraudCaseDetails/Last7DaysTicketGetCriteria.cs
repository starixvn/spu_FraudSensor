namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor.FraudCaseDetails
{
    using Fanex.Data.Repository;
    using System;

    public class Last7DaysTicketGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey() => "FS_Rpt_Ticket_Get";

        public override bool IsValid() => true;
    }
}