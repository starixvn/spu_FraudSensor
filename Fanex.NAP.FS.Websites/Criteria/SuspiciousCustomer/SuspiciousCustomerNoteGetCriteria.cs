using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Criteria.SuspiciousCustomer
{
    public class SuspiciousCustomerNoteGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_SuspiciousCustomer_GetNotes";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}