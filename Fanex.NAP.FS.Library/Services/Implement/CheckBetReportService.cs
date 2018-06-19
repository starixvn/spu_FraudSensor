namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.CheckBetReport;
    using System;
    using System.Collections.Generic;

    public class CheckBetReportService : ICheckBetReportService
    {
        public IEnumerable<CheckBetTicket> Generate(CheckBetReportGetCriteria criteria)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    yield return new CheckBetTicket
                    {
                        BetId = 10074823887,
                        TransDate = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss"),
                        Currency = "HKK",
                        Bettype = "Gravity Theory",
                        Username = "Simon",
                        Stake = new ViewModels.History<double?>
                        {
                            New = 10.5,
                            Old = 10.8
                        },
                        Choice = new ViewModels.History<string>
                        {
                            New = "Topology",
                            Old = "String theory"
                        },
                        Odds = new ViewModels.History<string>
                        {
                            New = "0.8 (MY)",
                            Old = "0.9 (MY)"
                        },
                        PoistionTaking = new ViewModels.History<double?>
                        {
                            New = 5,
                            Old = 8
                        },
                        Status = new ViewModels.History<string>
                        {
                            New = "WIN",
                            Old = "DRAW"
                        },
                        Creator = "Leonard Susskind",
                        Group = i
                    };
                }
            }
        }
    }
}