namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Data;
    using Data.Repository;
    using System;

    public class SuspiciousCustomerUpdateCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public bool? IsDelinquent { get; set; }

        public bool? @IsHedge { get; set; }

        public bool? IsVerified { get; set; }

        public bool? @IsDirty { get; set; }

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