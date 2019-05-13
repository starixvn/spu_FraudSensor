using System.Xml.Serialization;

namespace Fanex.NAP.FS.Library.Models.FraudCaseTask
{
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;

    
    [XmlType(TypeName = "Match")]
    public class ArbitrageTicket
    {
        [XmlAttribute]
        public int MatchId { get; set; }

        [XmlAttribute]
        public int CustId { get; set; }

        [XmlAttribute("BetType")]
        public int Bettype { get; set; }

        [XmlAttribute]
        public int LiveHomeScore { get; set; }

        [XmlAttribute]
        public int LiveAwayScore { get; set; }

        [XmlAttribute]
        public double Handicap { get; set; }
    }
}