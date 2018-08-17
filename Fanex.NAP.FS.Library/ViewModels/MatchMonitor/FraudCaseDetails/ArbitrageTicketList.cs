using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails
{
    public class ArbitrageTicketList
    {
        public IEnumerable<ArbitrageTicket> Home { get; set; }

        public IEnumerable<ArbitrageTicket> Away { get; set; }
    }
}
