namespace Fanex.NAP.FS.Library.ViewModels.CheckBetReport
{
    public class CheckBetTicket
    {
        public long BetId { get; set; }

        public string TransDate { get; set; }

        public string Currency { get; set; }

        public string Bettype { get; set; }

        public string Username { get; set; }

        public History<decimal?> Stake { get; set; }

        public History<string> Odds { get; set; }

        public History<string> Choice { get; set; }

        public History<decimal?> PoistionTaking { get; set; }

        public History<string> Status { get; set; }

        public string Creator { get; set; }

        public int Group { get; set; }
    }
}