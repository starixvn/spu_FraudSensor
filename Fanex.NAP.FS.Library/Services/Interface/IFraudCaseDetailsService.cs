namespace Fanex.NAP.FS.Library.Service
{
    using Criteria.MatchMonitor.FraudCaseDetails;    
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using System;
    using System.Collections.Generic;    

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
              
        IEnumerable<CustomerDetails> GetCustomerArbitrageDetails(GetArbitrageCustomerCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP);

        IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(GetArbitrageTicketCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP);

        IEnumerable<ArbitrageGroupTicketList> GetListArbitrageTickets(GetArbitrageTicketCriteria criteria);

        IEnumerable<Evidence> GetListEvidences();

        IEnumerable<Currency> GetAllCurrency(bool isShowSpecialCurrencies);

        IEnumerable<ScoreHDP> GetScoreHDP(int matchId, bool isShowSpecialCurrencies);

        byte[] ExportToExcelIpTickets(string ip, string template, bool isShowSpecialCurrencies);      
       
        string ExportArbitrageTicketByGroupCsv(GetArbitrageTicketCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP, int groupId, string score, decimal handicap, string[] array);

        string ExportArbitrageTicketAllCsv(GetArbitrageTicketCriteria ticketCriteria, IEnumerable<ScoreHDP> lstScoreHDP, string[] array);

        string ExportArbitrageCustomersCsv(GetArbitrageCustomerCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP, string[] array);
    }
}