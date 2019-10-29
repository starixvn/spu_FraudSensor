using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP
{
    public class OfficeIpTransaction
    {
        public int CustId { get; set; }

        public string Transdate { get; set; }

        public string Ip { get; set; }

        public int SubscriberId { get; set; }

    }
}
