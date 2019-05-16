namespace Fanex.NAP.FS.Library.Models.CheckBetReport
{
    using System;

    public class CheckBetHistory
    {
        public long TransId { get; set; }

        public int CustId { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public DateTime TransDate { get; set; }

        public int OddsType { get; set; }

        public string Currency { get; set; }

        public string Username { get; set; }

        public string Creator { get; set; }

        public int Bettype { get; set; }

        public bool LiveIndicator { get; set; }

        public decimal? NewStake { get; set; }

        public decimal? OldStake { get; set; }

        public decimal? NewOdds { get; set; }

        public decimal? OldOdds { get; set; }

        public int? NewLiveHomeScore { get; set; }

        public int? OldLiveHomeScore { get; set; }

        public int? NewLiveAwayScore { get; set; }

        public int? OldLiveAwayScore { get; set; }

        public decimal? NewPT { get; set; }

        public decimal? OldPT { get; set; }

        public int? NewMatchId { get; set; }

        public int? OldMatchId { get; set; }

        public string NewStatus { get; set; }

        public string OldStatus { get; set; }

        public string NewBetteam { get; set; }

        public string OldBetteam { get; set; }

        public decimal? NewHdp1 { get; set; }

        public decimal? OldHdp1 { get; set; }

        public decimal? NewHdp2 { get; set; }

        public decimal? OldHdp2 { get; set; }
        public string BetCheck { get; set; }
        public bool IsNeutral { get; set; }
        public string TransDesc { get; set; }
        public decimal OddsSpread { get; set; }
        public long BetdaqId { get; set; }
        public string ExtraInfo { get; set; }
        public object WalletType { get; set; }
        public string Race { get; set; }

        public string Note { get; set; }
    }
}