#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

namespace Fanex.NAP.FS.Library.Criteria.SuspiciousInfo
{
    using Fanex.Data.Repository;
    using System.Configuration;

    public class SuspiciousInfoGetCriteria : CriteriaBase
    {
        public SuspiciousInfoGetCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; set; }

        public int Environment => int.Parse(ConfigurationManager.AppSettings["Environment"]);

        public override string GetSettingKey()
        {
            return "FS_SuspiciousCustomer_FraudCases_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static