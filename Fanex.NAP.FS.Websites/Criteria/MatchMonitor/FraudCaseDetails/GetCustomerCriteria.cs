namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor.FraudCaseDetails
{
    using Fanex.Data.Repository;
    using System;

    public class GetCustomerCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public int FraudCaseId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_Details_Get_ByCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}