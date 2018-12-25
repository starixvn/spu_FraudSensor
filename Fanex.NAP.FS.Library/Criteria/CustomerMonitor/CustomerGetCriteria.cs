namespace Fanex.NAP.FS.Library.Criteria.CustomerMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class CustomerGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_CustomerMonitor_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}