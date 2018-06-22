#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.CheckBetReport;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.CheckBetReport;
    using RemotingRef;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class CheckBetReportService : ICheckBetReportService
    {
        private readonly TicketHelper ticketHelper = new TicketHelper();

        private readonly BetListHTMLBuilder htmlBuilder = new BetListHTMLBuilder();

        public IEnumerable<CheckBetTicket> Generate(CheckBetReportGetCriteria criteria)
        {
            var data = DataService.GetData<CheckBetHistory>(criteria);

            var groups = BuildGroup(GetFPSData(data.Select(d => d.CustId))).ToList();

            return data
                .Select(d => GetTicket(d, groups))
                .GroupBy(t => t.Group)
                .OrderByDescending(g => g.Count())
                .SelectMany(g => g.OrderBy(t => t.Username).ThenBy(t => t.BetId));
        }

        private CheckBetTicket GetTicket(CheckBetHistory history, List<HashSet<int>> groups)
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
                IsLive = true
            };

            var oldChoice = htmlBuilder.BuildChoiceBlock(oldTicket, ticketHelper, null, false);
            var newChoice = htmlBuilder.BuildChoiceBlock(newTIcket, ticketHelper, null, false);

            return new CheckBetTicket
            {
                BetId = history.TransId,
                Bettype = RefData.GetBetTypeName(history.Bettype.ToString(), "en"),
                Choice = new ViewModels.History<string>
                {
                    New = BetListHelper.ExtractStringFromHtml(newChoice.betTeam).Pick() + BetListHelper.ExtractStringFromHtml(newChoice.Handicap) + BetListHelper.ExtractStringFromHtml(newChoice.Score),
                    Old = BetListHelper.ExtractStringFromHtml(oldChoice.betTeam).Pick() + BetListHelper.ExtractStringFromHtml(oldChoice.Handicap) + BetListHelper.ExtractStringFromHtml(oldChoice.Score)
                },
                Creator = history.Creator,
                Currency = history.Currency,
                Group = groups.FindIndex(g => g.Contains(history.CustId)),
                Odds = new ViewModels.History<string>
                {
                    New = $"{history.NewOdds.Pick(0).ToString("N2")} ({RefData.GetOddsTypeName(history.OddsType.ToString(), "en")})",
                    Old = $"{history.OldOdds.Pick(0).ToString("N2")} ({RefData.GetOddsTypeName(history.OddsType.ToString(), "en")})"
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
                Username = history.Username
            };
        }

        private IEnumerable<HashSet<int>> BuildGroup(IEnumerable<IEnumerable<int>> sets)
        {
            var groups = new List<HashSet<int>>();

            if (sets.Any())
            {
                groups.Add(new HashSet<int>(sets.First()));

                foreach (var set in sets.Skip(1))
                {
                    var newGroup = new HashSet<int>(set);

                    foreach (var g in groups.Where(g => set.Intersect(g).Any()))
                    {
                        newGroup.UnionWith(g);
                        g.Clear();
                    }

                    groups.RemoveAll(g => g.Count == 0);
                    groups.Add(newGroup);
                }
            }

            return groups;
        }

        private IEnumerable<IEnumerable<int>> GetFPSData(IEnumerable<int> data)
        {
            var fpsData = ApiHelper.Post<CustomerList, IEnumerable<CustomerAssociation>>(
                    AppSettings.Get("FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT"),
                    new CustomerList { CustIdList = data.Join() },
                    int.Parse(ConfigurationManager.AppSettings["Environment"]),
                    int.Parse(ConfigurationManager.AppSettings["MailingListId"])
                ).Result;

            return data.GroupJoin(fpsData, d => d, d => d.FromCustId, (key, g) => g.Select(d => d.ToCustId).Concat(new[] { key }));
        }
    }
}

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static