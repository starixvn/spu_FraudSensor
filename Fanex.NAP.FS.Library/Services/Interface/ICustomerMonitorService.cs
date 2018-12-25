namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Models.CustomerMonitor;
    using System.Collections.Generic;

    public interface ICustomerMonitorService
    {
        IEnumerable<Customer> GetCustomers(CustomerGetCriteria criteria);
    }
}