namespace Fanex.NAP.FS.Library.Core.Consts
{
    public enum SystemId
    {
        CTSWeb = 1,
        CTSAPI = 2,
        FS = 3,
        TGRS = 4
    };

    public enum TrackingFunctionId
    {
        SuspiciousCustomer_Get = 301,
        SuspiciousCustomer_ExportToExcel = 302,
        SuspiciousCustomer_Detail_Get = 303,
        SuspiciousCustomer_Detail_UpdateAssignee = 304,
        SuspiciousCustomer_Detail_UpdateFCVerify = 305,
        SuspiciousCustomer_Detail_RemoveLogNote = 306,
        SuspiciousCustomer_Detail_InsertLogNote = 307,
        SuspiciousCustomer_Detail_UpdateCust = 308,
        MatchMonitor_Get = 309,
        MatchMonitor_UpdateMatchNote = 310,
        MatchMonitor_VerifyMatch = 311,
        MatchMonitor_ExportToExcel = 312,
        MatchMonitor_Detail_BSNG_GetTicket = 313,
        MatchMonitor_Detail_DelayGoal_GetTicket = 314,
        MatchMonitor_Detail_BSNG_ExportToExcel = 315,
        MatchMonitor_Detail_DelayGoal_ExportToExcel = 316,

        ScannedOffice_IP_Get = 317,
        ScannedOffice_IP_ExportToExcel = 318,
        DailyDataCapture_GetMatch = 319,
        DailyDataCapture_UpdateNote = 320,
        DailyDataCapture_ExportToExcel = 321,

        CheckBetRpt_Get = 322,
        CheckBetRpt_UpdateNote = 323,
        CheckBetRpt_ExportToExcel = 324,

        CustomerMonitor_Get = 325,    
        CustomerMonitor_UpdateNote = 326,
        CustomerMonitor_Detail_GetTicket = 327,
        CustomerMonitor_Detail_ExportToExcel = 328,

        AssAccMonitor_Get = 329,
        AssAccMonitor_Verify = 330,
        AssAccMonitor_UpdateNote = 331,
        AssAccMonitor_Detail_Get = 332,

        FCRule_Get = 333,
        FCRule_BSNG_UpdateStatus = 334,
        FCRule_OfficeIP_UpdateStatus = 335,
        FCRule_CheckBet_UpdateStatus = 336,
        FCRule_DailyData_UpdateStatus = 337,
        FCRule_FlaggedAccount_UpdateStatus = 338,
        FCRule_DelayGoal_UpdateStatus = 339,
        FCRule_Arbitrage_UpdateStatus = 340,
        FCRule_BetNearGoal_UpdateStatus = 341,
        FCRule_GroupBetting_UpdateStatus = 342,
        FCRule_Hedging_UpdateStatus = 343,

        FCRule_DelayGoal_GetCust = 344,
        FCRule_DelayGoal_DeleteCust = 345,
        FCRule_DelayGoal_AddCust = 346,
        FCRule_DelayGoal_ExportToExcel = 347,
        FCRule_IP_Get = 348,
        FCRule_IP_Delete = 349,
        FCRule_IP_Add = 350,
        FCRule_IP_ExportToExcel = 351,
        FCRule_InternalAccount_Get = 352,
        FCRule_InternalAccount_Add = 353,
        FCRule_InternalAccount_Delete = 354,
        FCRule_InternalAccount_ExportToExcel = 355,
        FCRule_Recipient_Get = 356,
        FCRule_Recipient_Add = 357,
        FCRule_Recipient_Delete = 358

    };
}
