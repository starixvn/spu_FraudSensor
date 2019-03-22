using Fanex.NAP.FS.Library.FSMQ;
using Fanex.NAP.FS.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Core.BaseClasses.RabbitMQ
{
    public class FraudCaseStatus
    {
        public FraudCaseId FraudCase { get; set; }
        public SendingType SendingType { get; set; }
    }
}
