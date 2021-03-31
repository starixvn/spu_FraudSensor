#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
#pragma warning disable S3881 // "IDisposable" should be implemented correctly

namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.CheckBetReport;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.CheckBetReport;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CheckBetReportService : ICheckBetReportService, IDisposable
    {
        private readonly TicketHelper ticketHelper = new TicketHelper();

        private readonly BetListExcelBuilder excelBuilder = new BetListExcelBuilder();

        public IEnumerable<CheckBetTicket> Generate(CheckBetReportGetCriteria criteria)
        {
            var data = DataService.GetData<CheckBetHistory>(criteria);

            return data
                .Select(d => GetTicket(d))
                .OrderBy(t => t.Username).ThenBy(t => t.BetId);
        }

        public DBResult UpdateNote(CheckBetReportUpdateNoteCriteria criteria)
        {
            criteria.Note = criteria.Note == null ? "" : criteria.Note;

            return DataService.UpdateData(criteria, "CheckBet");
        }

        private CheckBetTicket GetTicket(CheckBetHistory history)
        {
            var oldTicket = new Ticket
            {
                TransId = history.TransId,
                Stake = history.OldStake.GetValueOrDefault(0),
                Odds = history.OldOdds.GetValueOrDefault(0),
                LiveHomeScore = history.OldLiveHomeScore.GetValueOrDefault(0),
                LiveAwayScore = history.OldLiveAwayScore.GetValueOrDefault(0),
                MatchId = history.OldMatchId.GetValueOrDefault(0),
                Status = history.OldStatus,
                BetTeam = history.OldBetteam,
                Handicap1 = history.OldHdp1.GetValueOrDefault(0),
                Handicap2 = history.OldHdp2.GetValueOrDefault(0),
                HomeId = history.HomeId,
                AwayId = history.AwayId,
                BetTypeId = history.Bettype,
                BetCheck = history.BetCheck,
                IsNeutral = history.IsNeutral,
                TransDate = history.TransDate,
                TransDesc = history.TransDesc,
                OddsSpread = history.OddsSpread,
                BetdaqId = history.BetdaqId,
                ExtraInfo = history.ExtraInfo,
                Race = history.Race,
                IsLive = true
            };

            var newTIcket = new Ticket
            {
                TransId = history.TransId,
                Stake = history.NewStake.GetValueOrDefault(0),
                Odds = history.NewOdds.GetValueOrDefault(0),
                LiveHomeScore = history.NewLiveHomeScore.GetValueOrDefault(0),
                LiveAwayScore = history.NewLiveAwayScore.GetValueOrDefault(0),
                MatchId = history.NewMatchId.GetValueOrDefault(0),
                Status = history.NewStatus,
                BetTeam = history.NewBetteam,
                Handicap1 = history.NewHdp1.GetValueOrDefault(0),
                Handicap2 = history.NewHdp2.GetValueOrDefault(0),
                HomeId = history.HomeId,
                AwayId = history.AwayId,
                BetTypeId = history.Bettype,
                BetCheck = history.BetCheck,
                IsNeutral = history.IsNeutral,
                TransDate = history.TransDate,
                TransDesc = history.TransDesc,
                OddsSpread = history.OddsSpread,
                BetdaqId = history.BetdaqId,
                ExtraInfo = history.ExtraInfo,
                Race = history.Race,
                IsLive = true
            };
            
            var oldChoice = excelBuilder.CreateChoiceBuilder(history.Bettype).Render(oldTicket, ticketHelper, null, false);
            var newChoice = excelBuilder.CreateChoiceBuilder(history.Bettype).Render(newTIcket, ticketHelper, null, false);

            Logging.LogService.Info("Start log");
            if (oldChoice == null || newChoice == null)
            {
                Logging.LogService.Info("history.Bettype: " + history.Bettype.ToString());
                if (oldTicket == null)
                {
                    Logging.LogService.Info("oldTicket is null");
                }
                else
                {
                    Logging.LogService.Info("oldTicket is not null");
                }

                if (newTIcket == null)
                {
                    Logging.LogService.Info("newTIcket is null");
                }
                else
                {
                    Logging.LogService.Info("newTIcket is not null");
                }
            }
            else 
            {
                if (newChoice.betTeam == null || oldChoice.betTeam == null)
                {
                    Logging.LogService.Info("newChoice.betTeam is null");
                }
            }

         
            return new CheckBetTicket
            {
                BetId = history.TransId,
                Bettype = CachedRef.GetBettypeName(history.Bettype),
                Choice = new ViewModels.History<string>
                {
                    New = BetListHelper.ExtractStringFromHtml(newChoice.betTeam).Pick() + BetListHelper.ExtractStringFromHtml(newChoice.Handicap) + BetListHelper.ExtractStringFromHtml(newChoice.Score),
                    Old = BetListHelper.ExtractStringFromHtml(oldChoice.betTeam).Pick() + BetListHelper.ExtractStringFromHtml(oldChoice.Handicap) + BetListHelper.ExtractStringFromHtml(oldChoice.Score)
                },
                Creator = history.Creator,
                Currency = history.Currency,
                Odds = new ViewModels.History<string>
                {
                    New = $"{history.NewOdds.Pick(0).ToString("N2")} ({CachedRef.GetOddsTypeName(history.OddsType)})",
                    Old = $"{history.OldOdds.Pick(0).ToString("N2")} ({CachedRef.GetOddsTypeName(history.OddsType)})"
                },
                PoistionTaking = new ViewModels.History<decimal?>
                {
                    New = history.NewPT,
                    Old = history.OldPT
                },
                Stake = new ViewModels.History<decimal?>
                {
                    New = history.NewStake,
                    Old = history.OldStake
                },
                Status = new ViewModels.History<string>
                {
                    New = history.NewStatus?.ToUpper(),
                    Old = history.OldStatus?.ToUpper()
                },
                TransDate = history.TransDate.ToString("MMM dd, yyyy HH:mm:ss"),
                Username = history.Username,
                Note = history.Note
            };
        }

        public void Dispose()
        {
            excelBuilder.Dispose();
        }
    }
}

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
#pragma warning restore S3881 // "IDisposable" should be implemented correctly