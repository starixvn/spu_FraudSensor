using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Criteria.FraudCase
{
    public class FraudCaseGetCriteria:CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_FraudCase_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}