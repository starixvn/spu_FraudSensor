namespace Fanex.NAP.FS.Library.Models.FraudCase
{
    using System.Collections.Generic;

    public class FraudCase
    {
        public int FraudCaseId { get; set; }

        public string ScheduleTime { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<string> MatchRules { get; set; }

        public IEnumerable<string> TicketRules { get; set; }

        public IEnumerable<string> Actions { get; set; }

        public static FraudCase GetFraudCase(FraudCaseId id)
        {
            var result = new FraudCase
            {
                FraudCaseId = (int)id,
                ScheduleTime = "In every 60 seconds"
            };

            switch (id)
            {
                case Models.FraudCaseId.BSNG:
                    result.MatchRules = new[]
                    {
                        "Sport = Soccer",
                        "Match = In-play",
                        "Score = Got Changed"
                    };

                    result.TicketRules = new[]
                    {
                        "Bet type = HDP, O/U, FT. 1X2, 1H HDP, 1H O/U, 1H 1X2",
                        "Transaction Date = Danger Period (90s before goal)",
                        "Total stake in danger period ≥ 40% Max Bet",
                        "Currency <> UUS, H, C"
                    };

                    break;

                case Models.FraudCaseId.DelayedGoal:
                    result.MatchRules = new[]
                    {
                        "Sport = Soccer",
                        "Match = In-play",
                        "Market = Main Markets"
                    };

                    result.TicketRules = new[]
                    {
                        "Customer = Delay Goal List <a href='#' class='view-list'>View List</a>"
                    };

                    break;

                case Models.FraudCaseId.FlaggedAccount:
                    result.MatchRules = new[]
                    {
                        "Sport = Socccer",
                        "Match = In-play"
                    };

                    result.TicketRules = new[]
                    {
                        "Customer has evidence on FPS",
                        "Currency <> UUS, H, C"
                    };

                    break;

                case Models.FraudCaseId.ScannedOfficeIP:
                    result.ScheduleTime = "In every 60 minutes";

                    result.MatchRules = new[]
                    {
                        "Match Status = Completed",
                        "Match State = Not Scanned"
                    };

                    result.TicketRules = new[]
                    {
                        "IP = <a href='#' id='ip-link' class='view-list'>IP List</a>",
                        "Currency <> UUS, H, C"
                    };

                    break;

                case Models.FraudCaseId.DailyDataCapture:
                    result.ScheduleTime = "Daily";

                    result.MatchRules = new[]
                    {
                        "Match Status = Completed",
                        "Re-scan = 3 times in 3 days"
                    };

                    result.TicketRules = new[]
                    {
                        "Total Company Turnover got changed",
                        "OR Total Super Turnover got changed",
                        "OR Total Master Turnover got changed",
                        "OR Total Agent Turnover got changed",
                        "OR Total tickets got changed",
                        "OR Total Settled tickets got changed",
                        "OR Total Cancelled tickets got changed"
                    };

                    result.Actions = new[]
                    {
                        "Send notification to <a id='user-link' class='view-list' href='#'>Recipient List</a>"
                    };

                    break;

                case Models.FraudCaseId.CheckBet:
                    result.MatchRules = new[]
                    {
                        "Sport = Sportsbook",
                        "Match Status = Completed",
                        "Once a day in 3 days"
                    };

                    result.TicketRules = new[]
                    {
                        "Bet type = Normal Sportsbook Bets",
                        "Currency <> UUS, H, C",
                        "(Stake got changed",
                        "OR Odds got changed",
                        "OR Choice got changed",
                        "OR SMA PT got changed",
                        "OR Rejected ticket status got changed)"
                    };

                    result.Actions = new[]
                    {
                        "Send notification to <a id='user-link' class='view-list' href='#'>Recipient List</a>"
                    };

                    break;

                case Models.FraudCaseId.Arbitrage:
                    result.MatchRules = new[]
                    {
                        "Sport = Soccer",
                        "Match = In-Play"
                    };

                    result.TicketRules = new[]
                    {
                        "Bet type = HDP, OU, 1H HDP, 1H OU",
                        "Customer/Group bet on both sides",
                        "Total stake of each side >= 500RM",
                        "Currency <> UUS, H, C"
                    };

                    break;

		case Models.FraudCaseId.BetsNearGoal:
                    result.MatchRules = new[]
                    {
                        "Sport = Soccer",
                        "Match = Completed",
                        "Scanned Status = Not Scanned"
                    };

                    result.TicketRules = new[]
                    {
                        "Bet type = HDP, O/U, FT. 1X2, 1H HDP, 1H O/U, 1H 1X2",
                        "Transaction Date = Danger Period (before goal 30s)",
                        "Ticket count >= 5",
                        "Currency <> Test Currency",
                        "Ticket Status =  WON",
                        "Violation time + 1 for one match/score"
                    };
	
                    break;

                default:
                    result.ScheduleTime = "There's no data";
                    result.TicketRules = new[] { "There's no data" };
                    result.MatchRules = new[] { "There's no data" };

                    break;
            }

            return result;
        }
    }
}