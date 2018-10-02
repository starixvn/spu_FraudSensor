#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomerDetails
{
    using System.Configuration;

    public class SuspiciousCustomerDetailsViolatedTicketGetCriteria : SuspiciousCustomerDetailsGeneralCriteria
    {
        public int Environment => int.Parse(ConfigurationManager.AppSettings["Environment"]);

        public override string GetSettingKey() => "FS_Rpt_SuspicousCustomerDetails_ViolatedTickets_Get";
    }
}

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static