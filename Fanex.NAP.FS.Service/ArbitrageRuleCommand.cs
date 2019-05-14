namespace Fanex.NAP.FS.Service
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ArbitrageRuleCommand : NonQueryCommand
    {
        public string Matches { get; set; }

        public string Associations { get; set; }

        public override string GetSettingKey() => "FS_FraudCase_Arbitrage_Rule";

        public override bool IsValid() => true;
    }
}
