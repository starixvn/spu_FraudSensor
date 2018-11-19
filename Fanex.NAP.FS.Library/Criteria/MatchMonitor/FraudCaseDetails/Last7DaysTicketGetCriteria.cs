namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Fanex.Data.Repository;

    public class Last7DaysTicketGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_Ticket_Get";
        }

        public override bool IsValid() => true;
    }
}