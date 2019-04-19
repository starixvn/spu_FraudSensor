using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Model
{
    public class FraudCaseHealth
    {
        public int FraudCaseId { get; set; }

        public bool IsLive { get; set; }

        public int TimeOut { get; set; }

        public int LetstalkCurrentTimes { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastTimeSignal { get; set; }
    }
}
