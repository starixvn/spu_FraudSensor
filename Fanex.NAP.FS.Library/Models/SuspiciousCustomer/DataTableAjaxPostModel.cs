using Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer;

namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomer
{
    public class DataTableAjaxPostModel : SuspiciousCustomerReportGetCriteria
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        //public SuspiciousCustomerReportGetCriteria Search { get; set; }
    }
}
