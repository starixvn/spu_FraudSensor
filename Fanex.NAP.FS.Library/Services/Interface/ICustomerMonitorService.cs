namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.CustomerMonitor;
    using Fanex.NAP.FS.Library.ViewModels.CustomerMonitor.CustomerDetails;
    using System.Collections.Generic;

    public interface ICustomerMonitorService
    {
        IEnumerable<Customer> GetCustomers(CustomerGetCriteria criteria);

        DBResult UpdateNote(UpdateNoteCriteria criteria);

        IEnumerable<TicketDetails> GetTicketDetails(GetCustomerTicketsCriteria criteria, bool saveChoice = false);

        byte[] ExportExcel(GetCustomerTicketsCriteria criteria, CustomerDetailsInfo detailsInfo, string templatePath);
    }
}