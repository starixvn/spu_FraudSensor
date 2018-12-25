namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Models.CustomerMonitor;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Collections.Generic;

    public class CustomerMonitorService : ICustomerMonitorService
    {
        public IEnumerable<Customer> GetCustomers(CustomerGetCriteria criteria)
        {
            return DataService.GetData<Customer>(criteria);
        }
    }
}