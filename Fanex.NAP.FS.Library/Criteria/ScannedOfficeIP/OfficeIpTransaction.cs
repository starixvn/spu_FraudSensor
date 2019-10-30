using Fanex.NAP.FS.Library.Core.Helpers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP
{
    [XName("OfficeIPTrans")]
    public class OfficeIpTransaction
    {
        public int CustId { get; set; }

        public string TransDate { get; set; }

        public string IP { get; set; }

        public int SubscriberId { get; set; }

    }
}
