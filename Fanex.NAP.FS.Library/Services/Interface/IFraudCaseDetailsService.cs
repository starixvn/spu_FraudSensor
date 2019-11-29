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
        IEnumerable<TicketDetails> GetTicketDetails(GetTicketCriteria criteria, bool saveChoice = false);

        IEnumerable<CustomerDetails> GetCustomerDetails(GetCustomerCriteria criteria);

        byte[] ExportExcelForDetails(GetTicketCriteria criteria, string templatePath);

        MatchFraudCaseInfo GetMatchInfo(int matchId, int fraudCaseId, DateTime eventDate);

        IEnumerable<TicketDetails> GetLast7DaysDetails(int custId, bool isShowSpecialCurrencies);

        IEnumerable<AssociatedCustomer> GetAssociatedAccounts(int custId);

        BettypeOrder GetOrder(string bettype);

        IEnumerable<TicketInfo> GetLast7DaysInfo(int custId, bool isShowSpecialCurrencies);

        IEnumerable<CustomerTotalAssociation> GetTotalAssociations(IEnumerable<int> custIds);

        IEnumerable<CustomerAssociation> GetAssociations(int custId);

        IEnumerable<TicketDetails> GetTicketsFromIP(string ip, bool isShowSpecialCurrencies);
              
        IEnumerable<CustomerDetails> GetArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria);       

        IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(GetArbitrageTicketCriteria criteria, bool isSummary);

        IEnumerable<Evidence> GetListEvidences();

        IEnumerable<Currency> GetAllCurrency(bool isShowSpecialCurrencies);

        IEnumerable<ScoreHDP> GetScoreHDP(GetScoreHDPCriteria criteria);

        IEnumerable<Site> GetSites();

        string ReplaceFilterEvidences(string evidences);

        byte[] ExportExcelTicketsByIP(string ip, string template, bool isShowSpecialCurrencies);

        byte[] ExportExcelArbitrageTickets(GetArbitrageTicketCriteria criteria, string[] array, string templatePath, bool isSummary);
      
        byte[] ExportExcelArbitrageCustomers(GetArbitrageCustomerCriteria criteria, string[] array, string templatePath, IEnumerable<long> listCustIdFromTickets);

        IEnumerable<FlaggedAccountTicket> GetFlaggedAccountTickets(FlaggedAccountCriteria criteria);

        IEnumerable<CustomerDetails> GetFlaggedAccountCustomers(FlaggedAccountCriteria criteria, bool isExportFile = false);       

        byte[] ExportExcelFlaggedAccounts(FlaggedAccountCriteria criteria, string templatePath);

        IEnumerable<BetTypes> GetBetTypes();
    }
}