#pragma warning disable S1244 // Floating point numbers should not be tested for equality

namespace Fanex.NAP.FS.Library.Models.FraudCaseTask
{
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using System;

    [XName("Match")]
    public class ArbitrageGroup
    {
        public int MatchId { get; set; }

        public int Bettype { get; set; }

        public int TotalScore { get; set; }

        public double Handicap { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as ArbitrageGroup;

            if(other is null)
            {
                return false;
            }

            return MatchId == other.MatchId
                && Bettype == other.Bettype
                && TotalScore == other.TotalScore
                && Handicap == other.Handicap;
        }

        public override int GetHashCode()
        {
            var t = Tuple.Create(MatchId, Bettype, TotalScore, Handicap);

            return t.GetHashCode();
        }
    }
}

#pragma warning restore S1244 // Floating point numbers should not be tested for equality