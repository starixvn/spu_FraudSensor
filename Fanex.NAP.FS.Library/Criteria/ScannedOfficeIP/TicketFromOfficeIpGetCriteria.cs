namespace Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP
{
    using Fanex.Data.Repository;
    using System;

    public class TicketFromOfficeIpGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "";

        public override bool IsValid() => true;
    }
}