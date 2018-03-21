using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor.FraudCaseDetails
{
    public class GetTicketCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public int FraudCaseId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_Details_Get_ByTicket";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}