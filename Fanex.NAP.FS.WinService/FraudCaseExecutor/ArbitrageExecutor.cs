namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.FraudCaseTask;
    using System.Collections.Generic;
    using System.Linq;

    [FraudCase(Library.Models.FraudCaseId.Arbitrage)]
    public class ArbitrageExecutor : FraudCaseExecutor
    {
        private static IDictionary<ArbitrageGroup, IList<int>> cache = new Dictionary<ArbitrageGroup, IList<int>>();

        private IEnumerable<ArbitrageTicket> tickets;

        private IEnumerable<ArbitrageGroup> finishedGroups;

        public ArbitrageExecutor(FraudCase fraudCase) : base(fraudCase)
        {
        }

        protected override void ExecutePrecondition()
        {
            var tuples = DataService.QueryMultiple<ArbitrageTicket, ArbitrageGroup>(new ArbitragePreconditionCriteria());

            tickets = tuples.Item1;
            finishedGroups = tuples.Item2;
        }

        protected override void ExecuteRule()
        {
#if DEBUG
            tickets
               .GroupBy(t => new ArbitrageGroup
               {
                   Bettype = t.Bettype,
                   Handicap = t.Handicap,
                   MatchId = t.MatchId,
                   TotalScore = t.LiveAwayScore + t.LiveHomeScore
               })
               .All(g =>
               {
                   GroupData(g.Key, g);
                   return true;
               });
#else
         tickets
           .AsParallel()
           .GroupBy(t => new ArbitrageGroup
           {
               Bettype = t.Bettype,
               Handicap = t.Handicap,
               MatchId = t.MatchId,
               TotalScore = t.LiveAwayScore + t.LiveHomeScore
           })
           .ForAll(g => GroupData(g.Key, g));
#endif
        }

        protected override void TakeNextAction()
        {
        }

        protected override void ClearOldResult()
        {
            if (finishedGroups != null)
            {
                foreach (var item in finishedGroups)
                {
                    cache.Remove(item);
                }
            }

            finishedGroups = null;
        }

        private static void GroupData(ArbitrageGroup group, IEnumerable<ArbitrageTicket> tickets)
        {
            var previous = GetCachedGroup(group);

            var newCustids = tickets.Select(t => t.CustId).Distinct();

            var groups = GroupCustomer(newCustids.Concat(previous).Distinct())
                .ToDictionary(h => GetGroupId(h, previous));

            var customers = IndexCustomer(newCustids, groups);

            var criteria = new ArbitrageRuleCriteria
            {
                Matches = tickets.ToXmlString(),
                Associations = customers.ToXmlString()
            };

            DataService.Execute(criteria);
        }

        private static IList<int> GetCachedGroup(ArbitrageGroup group)
        {
            if (!cache.ContainsKey(group))
            {
                cache.Add(group, new List<int>());
            }

            return cache[group];
        }

        private static IEnumerable<HashSet<int>> GroupCustomer(IEnumerable<int> custids)
        {
            return ApiHelper.Post<IEnumerable<HashSet<int>>>(AppSettings.MS_WEBAPI_GROUP_CUSTOMER, custids).Result;
        }

        private static int GetGroupId(HashSet<int> group, IList<int> cachedGroups)
        {
            foreach (var item in cachedGroups)
            {
                if (group.Contains(item))
                {
                    return item;
                }
            }

            var id = group.First();

            cachedGroups.Add(id);

            return id;
        }

        private static IEnumerable<ArbitrageCustomer> IndexCustomer(IEnumerable<int> custids, IDictionary<int, HashSet<int>> groups)
        {
            foreach (var item in custids)
            {
                var groupId = groups.FirstOrDefault(kv => kv.Value.Contains(item)).Key;

                if (groupId == 0)
                {
                    yield return new ArbitrageCustomer
                    {
                        CustId = item,
                        GroupId = item
                    };
                }
                else
                {
                    yield return new ArbitrageCustomer
                    {
                        CustId = item,
                        GroupId = groupId
                    };
                }
            }
        }
    }
}