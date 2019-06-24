namespace Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    public class BetTypes
    {
        public int BetType { get; set; }

        public string BetTypeName
        {
            get
            {
                return CachedRef.GetBettypeName(BetType);
            }
        }
    }
}
