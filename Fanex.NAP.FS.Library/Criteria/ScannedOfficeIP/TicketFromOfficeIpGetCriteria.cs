namespace Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP
{
    using Fanex.Data.Repository;
    using System;

    public class TicketFromOfficeIpGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public override string GetSettingKey() => "FS_Rpt_OfficeIP_Get";

        public override bool IsValid() => true;
    }
}