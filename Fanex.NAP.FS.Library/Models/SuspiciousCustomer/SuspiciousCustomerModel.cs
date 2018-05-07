namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomer
{
    public class SuspiciousCustomerModel
    {
        public int CustId { get; set; }
        
        public bool? IsDelinquent { get; set; }

        public bool? IsHedge { get; set; }

        public bool? IsVerified { get; set; }

        public bool? IsDirty { get; set; }
    }
}