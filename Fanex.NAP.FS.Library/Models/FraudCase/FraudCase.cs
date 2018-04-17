namespace Fanex.NAP.FS.Library.Models.FraudCase
{
    using System;
    using System.Collections.Generic;

    public class FraudCase
    {
        public int FraudCaseId { get; set; }

        public int ScheduleTime { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<string> MatchRules { get; set; }

        public IEnumerable<string> TicketRules { get; set; }

        public static FraudCase GetFraudCase(FraudCaseId id)
        {
            var result = new FraudCase
            {
                FraudCaseId = (int)id,
                ScheduleTime = 60
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
                        "Currency <> Test Currency"
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
                        "Currency <> Test Currency"
                    };

                    break;

                default:
                    throw new NotImplementedException("Fraud case id " + id + " hasn't been implemented");
            }

            return result;
        }
    }
}