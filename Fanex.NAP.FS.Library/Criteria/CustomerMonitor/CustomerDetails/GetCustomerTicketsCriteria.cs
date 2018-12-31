namespace Fanex.NAP.FS.Library.Criteria.CustomerMonitor.CustomerDetails
{
    using Fanex.Data.Repository;
    using System;

    public class GetCustomerTicketsCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Environment { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_BNGCustomerDetails_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}