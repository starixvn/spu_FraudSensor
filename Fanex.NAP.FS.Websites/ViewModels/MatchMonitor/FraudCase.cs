using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.FS.Websites.ViewModels.MatchMonitor
{
    public class FraudCase
    {
        public FraudCase(int id, string name, IEnumerable<Models.MatchMonitor.FraudCase> data)
        {
            FraudCaseName = name;
            FraudCaseId = id;

            if (data.All(f => !f.SuspiciousTickets.HasValue))
            {
                Violations = null;
            }
            else
            {
                Violations = data.Sum(f => f.SuspiciousTickets.GetValueOrDefault());
            }
        }

        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public int? Violations { get; set; }
    }
}