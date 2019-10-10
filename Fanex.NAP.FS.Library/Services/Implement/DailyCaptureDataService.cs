namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.DailyDataCapture;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.DailyDataMonitor;
    using RemotingRef;
    using System.Collections.Generic;
    using System.Linq;

    public class DailyCaptureDataService : IDailyDataCaptureService
    {
        public IEnumerable<CapturedMatch> GetMatches(DailyDataGetCriteria criteria)
        {
            return DataService.GetData<Models.DailyDataCapture.CapturedMatch>(criteria)
                .Select(m => new CapturedMatch
                {
                    MatchId = m.MatchId,
                    MatchCode = m.MatchCode,
                    League = CachedRef.GetLeagueName(m.LeagueId),
                    Home = CachedRef.GetTeamName(m.HomeId),
                    Away = CachedRef.GetTeamName(m.AwayId),
                    AgentTO = new ViewModels.History<double?>
                    {
                        New = m.NewTotalAgentTO,
                        Old = m.OldTotalAgentTO
                    },
                    CancelledTickets = new ViewModels.History<int?>
                    {
                        New = m.NewTotalCancelledTickets,
                        Old = m.OldTotalCancelledTickets
                    },
                    CompanyTO = new ViewModels.History<double?>
                    {
                        New = m.NewTotalCompanyTO,
                        Old = m.OldTotalCompanyTO
                    },
                    EventDate = m.EventDate.ToString("MMM dd, yyyy HH:mm:ss"),
                    MasterTO = new ViewModels.History<double?>
                    {
                        New = m.NewTotalMasterTO,
                        Old = m.OldTotalMasterTO
                    },
                    ScannedDate = m.CapturedDate.ToString("MMM dd, yyyy HH:mm:ss"),
                    SettledTickets = new ViewModels.History<int?>
                    {
                        New = m.NewTotalSettledTickets,
                        Old = m.OldTotalSettledTickets
                    },
                    SuperTO = new ViewModels.History<double?>
                    {
                        New = m.NewTotalSuperTO,
                        Old = m.OldTotalSuperTO
                    },
                    Tickets = new ViewModels.History<int?>
                    {
                        New = m.NewTotalTickets,
                        Old = m.OldTotalTickets
                    },
                    Note = m.Note ?? string.Empty
                });
        }

        public DBResult UpdateDailyDataCaptureOfNote(DailyDataCaptureOfNote dataCaptureOfNote)
        {
            var criteria = new UpdateDailyDataCaptureOfNoteCriteria
            {
                UserId = dataCaptureOfNote.UserId,
                MatchIds = dataCaptureOfNote.MatchIds,
                Note = dataCaptureOfNote.Note ?? string.Empty
            };

            return DataService.UpdateData(criteria, "Update Note Daily Data Capture");
        }
    }
}