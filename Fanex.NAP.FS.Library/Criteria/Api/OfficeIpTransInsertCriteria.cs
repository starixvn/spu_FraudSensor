using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Criteria.Api
{
    public class OfficeIpTransInsertCriteria : CriteriaBase
    {
        public string OfficeIPTransXML { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_OfficeIPLogin_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(OfficeIPTransXML);
        }
    }
}
