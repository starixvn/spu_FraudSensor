using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomerDetails
{
    public abstract class SuspiciousCustomerDetailsGeneralCriteria: CriteriaBase
    {
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid() => true;
    }
}