namespace Fanex.NAP.FS.Library.Criteria
{
    using Data;
    using Data.Repository;
    using System;

    public class SuspiciousCustomerReportGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Username { get; set; }

        public bool? IsVerified { get; set; }

        public int? FraudCaseId { get; set; }

        [SpParam(Ignored = true)]
        public int Classification { get; set; }

        public bool? IsDelinquent
        {
            get
            {
                if(Classification == 0 || Classification == 1)
                {
                    return true;
                }

                return null;
            }
        }

        public bool? IsDirty
        {
            get
            {
                if (Classification == 0 || Classification == 2)
                {
                    return true;
                }

                return null;
            }
        }
        public bool? IsHedge
        {
            get
            {
                if (Classification == 0 || Classification == 3)
                {
                    return true;
                }

                return null;
            }
        }

        public int? AssigneeId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_SuspiciousCustomer_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
