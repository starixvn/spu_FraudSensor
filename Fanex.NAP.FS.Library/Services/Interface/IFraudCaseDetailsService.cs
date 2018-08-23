namespace Fanex.NAP.FS.Library.Service
{
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFraudCaseDetailsService
    {
        IEnumerable<TicketDetails> GetTicketDetails(int matchId, int fraudCaseId, DateTime eventDate, bool isShowSpecialCurrencies, bool saveChoice = false);

        IEnumerable<CustomerDetails> GetCustomerDetails(int matchId, int fraudCaseId, DateTime eventDate, bool isShowSpecialCurrencies);

        byte[] ExportExcel(int matchId, int fraudCaseId, DateTime eventDate, string templatePath, bool isShowSpecialCurrencies);

        MatchFraudCaseInfo GetInfo(int matchId, int fraudCaseId, DateTime eventDate);

        IEnumerable<TicketDetails> GetLast7DaysDetails(int custId, bool isShowSpecialCurrencies);

        IEnumerable<AssociatedCustomer> GetAssociatedAccounts(int custId);

        BettypeOrder GetOrder(string bettype);

        IEnumerable<TicketInfo> GetLast7DaysInfo(int custId, bool isShowSpecialCurrencies);

        IEnumerable<CustomerTotalAssociation> GetTotalAssociations(IEnumerable<int> custIds);

        IEnumerable<CustomerAssociation> GetAssociations(int custId);

        IEnumerable<TicketDetails> GetTicketFromIP(string ip, bool isShowSpecialCurrencies);

        byte[] ExportToExcelIpTickets(string ip, string template, bool isShowSpecialCurrencies);
        
        IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(int matchId, DateTime eventDate);

        byte[] ExportToExcelIpTickets(string ip, string template);
    }
}