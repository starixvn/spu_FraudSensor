namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Service.Utilities;
    using Library.Core.Helpers;
    using Library.Criteria.MatchMonitor.FraudCaseDetails;
    using Library.Service;
    using Service;
    using Service.MatchMonitor;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : ReportsController
    {
        private readonly IFraudCaseDetailsService _fraudCaseDetailService;

        private readonly IMatchMonitorService _matchMonitorService;

        private readonly bool _isShowSpecialCurrencies;

        private const string SessionCustomerList = "CustomerList";

        public MatchMonitorController()
        {
            _fraudCaseDetailService = new FraudCaseDetailsService(int.Parse(ConfigurationManager.AppSettings["Environment"]), int.Parse(ConfigurationManager.AppSettings["MailGroupId"]));
            _matchMonitorService = new MatchMonitorService();
            _isShowSpecialCurrencies = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies) > 0;
        }

        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);
            ViewBag.Sites = _fraudCaseDetailService.GetSites();

            return View(_matchMonitorService.GetBalanceUpDate());
        }

        #region Common
        [HttpPost]
        public ActionResult GetMatches(MatchFilter filter)
        {
            filter.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var matchList = _matchMonitorService.GetMatches(filter);

            return Json(new { data = matchList.Matches }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLeagues(LeagueGetCriteria criteria)
        {
            return Json(_matchMonitorService.GetLeagues(criteria));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult VerifyMatch(VerifyMatchCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;

            return Json(_matchMonitorService.VerifyMatch(criteria));
        }

        [HttpPost]
        public ActionResult GenerateExcel(MatchFilter filter)
        {
            filter.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/template.xlsx");
            var fileName = "MatchMonitor_" + DateTime.Now.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT) + ".xlsx";
            var excelFileContent = _matchMonitorService.ExportExcel(filter, templatePath);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        public ActionResult DownloadExcel(string fileId, string fileName)
        {
            if (TempData[fileId] != null)
            {
                byte[] excelFileContent = TempData[fileId] as byte[];
                TempData[fileId] = null;

                return File(excelFileContent, "application/vnd.ms-excel", fileName);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateMatchNote(MatchNoteUpdateCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
            criteria.Notes = criteria.Notes ?? string.Empty;

            return Json(_matchMonitorService.UpdateMatchNote(criteria));
        }

        public ActionResult GetLast7DaysTicket(int custId)
        {
            return Json(_fraudCaseDetailService.GetLast7DaysDetails(custId, _isShowSpecialCurrencies));
        }

        public ActionResult GetAssociation(int custId)
        {
            return Json(_fraudCaseDetailService.GetAssociatedAccounts(custId));
        }

        public ActionResult GetTicketFromIP(string ip)
        {
            return Json(_fraudCaseDetailService.GetTicketsFromIP(ip, _isShowSpecialCurrencies));
        }

        public ActionResult GetExcelTicketFromIP(string ip)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/ticket by ip.xlsx");
            var fileName = string.Format("TicketsofIP_{0}_{1}", ip, DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT));

            return ExcelFile(_fraudCaseDetailService.ExportExcelTicketsByIP(ip, template, _isShowSpecialCurrencies), fileName);
        }

        #endregion Common

        #region Other Fraud Cases
        public ActionResult GetFraudCaseDetails(int fraudCaseId, int matchId, DateTime eventDate, int violations)
        {
            var model = _fraudCaseDetailService.GetMatchInfo(matchId, fraudCaseId, eventDate);
            ViewBag.BUDate = _matchMonitorService.GetBalanceUpDate().BalanceUpDay;

            if (fraudCaseId == (int)Library.Models.FraudCaseId.Arbitrage
                || fraudCaseId == (int)Library.Models.FraudCaseId.FlaggedAccount)
            {
                ViewBag.SiteList = _fraudCaseDetailService.GetSites();
                ViewBag.CurrencyList = _fraudCaseDetailService.GetAllCurrency(_isShowSpecialCurrencies);

                var evidences = _fraudCaseDetailService.GetListEvidences();
                ViewBag.EvidenceList = evidences;
                ViewBag.OtherEvidenceCodes = evidences.Where(e => e.IsOther).Select(e => e.EvidenceCode).Join();
            }

            return View("FraudCaseDetails/Index", model);
        }

        public ActionResult GetFraudCaseTicketDetails(GetTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;

            var fraudTicketsDetails =
                _fraudCaseDetailService.GetTicketDetails(criteria)
                .GroupBy(t => t.Bettype)
                .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(fraudTicketsDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFraudCaseCustomerDetails(GetCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var fraudCustomersDetails = _fraudCaseDetailService.GetCustomerDetails(criteria);

            return Json(fraudCustomersDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcelForDetail(GetTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/template.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT));

            return File(_fraudCaseDetailService.ExportExcelForDetails(criteria, template), "application/vnd.ms-excel", fileName);
        }

        #endregion Other Fraud Cases

        #region Flagged Accounts

        [HttpPost]
        public ActionResult GetFlaggedAccountTickets(GetFlaggedAccountTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;

            //var flaggedAccountTickets =
            //   _fraudCaseDetailService.GetFlaggedAccountTickets(criteria)
            //   .GroupBy(t => t.Bettype)
            //   .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            //return Json(flaggedAccountTickets, JsonRequestBehavior.AllowGet);

            var flaggedAccountTickets =
               _fraudCaseDetailService.GetFlaggedAccountTickets(criteria);

            flaggedAccountTickets = GenerateSampleFlaggedAccountTickets(flaggedAccountTickets);
            var result = flaggedAccountTickets.GroupBy(t => t.Bettype)
                            .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(result, JsonRequestBehavior.AllowGet);
        }        

        [HttpPost]
        public ActionResult GetFlaggedAccountCustomers(GetFlaggedAccountCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;

            var customers = _fraudCaseDetailService.GetFlaggedAccountCustomers(criteria);

            var data = GenerateSampleFlaggedAccountCustomers(customers);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFlaggedAccountAssociationCustomers(string custIds)
        {
            custIds = "147233,405322,1016533,1070531,1234245,1329800,1431899,1509972,1549462,1824031,1848951,2184612,2265639,2363779,2566275,2793125,2987479,3041673,3055349,3599714,3730102,3805499,4050102,5268762,5353738,5356934,5511017,6535843,6633031,7692191,7696035,7754756,8020321,8272628,8386268,10065470,10164415,10548765,11557035,11557059,12006070,12281739,12297739,12306698,12328501,12330524,12668473,12668821,12676086,12737562,13675590,13725824,13830479,13960982,13986660,13995869,14000974,14045526,14106813,14172224,14905538,14905711,15180532,15272750,15314921,15430393,15735655,15834999,15977897,16496332,16806066,16875881,17125855,17134191,17136714,17144012,17156097,17175687,17298631,17336464,17552000,17710280,17729374,17879894,17902442,18086440,18431282,18478583,18684418,18799483,18861214,18864571,19032904,19032909,19064534,19363055,19383464,19744804,20224502,20427048,20515123,20593075,20709663,20771909,20771917,20948102,20984991,21418825,21464805,21553385,21558006,21596496,21613060,21618971,21785983,21934573,22020371,22206490,22265851,22292331,22321842,22391808,22447891,22450779,22471523,22710855,22710872,22710900,22710922,22710984,22711000,22711035,22711198,22720362,22777520,22809919,22977018,23014068,23048391,23135423,23211030,23385556,23406911,23503628,23593099,23831296,23853159,23909596,23924312,24002181,24060224,24076660,24192510,24192518,24222400,24228652,24271424,24271446,24271458,24271476,24271490,24271500,24271520,24440735,24456285,24542952,24563903,24644062,24648168,24695453,24695468,24695494,24695587,24695779,24695818,24695883,24695920,24695928,24734835,24734854,24734872,24734877,24735879,24792782,24796335,24796345,24948665,24957989,25050907,25412732,25580544,25640180,25648239,25654867,25654877,25925364,25967189,26094657,26166987,26173832,26179721,26206677,26252580,26284652,26554001,26579500,26613051,26684847,26709669,26858270,26947171,27023397,27120757,27140112,27341893,27384557,27389579,27437261,27485948,27495501,27553404,27585607,27641605,27715698,27812589,27812614,27833802,27911877,27929151,28030812,28033826,28172407,28175046,28184215,28184260,28190827,28356165,28389327,28420591,28454980,28471078,28508301,28541667,28551263,28551279,28551306,28558396,28593277,28757817,28771170,28817934,28817938,28817951,28817962,28817989,28818000,28818026,28818037,28818055,28818059,28818127,28818211,28818244,28818253,28818292,28818320,28818341,28818348,28848917,28849849,28851420,28891305,28891361,28891365,28891368,28891374,28891426,28891429,28893300,28893309,28893318,28893340,28893401,28893404,28893474,28893484,28893602,28893622,28893651,28894187,28894239,28894245,28894257,28894272,28921464,28923067,28925953,28943276,28947535,28949996,28977003,29003036,29047056,29059218,29083390,29092316,29093401,29093412,29093442,29093474,29094486,29094520,29094527,29094545,29094551,29094557,29094623,29094629,29094658,29094692,29094715,29096404,29096531,29096539,29096659,29096664,29096714,29099629,29099969,29100458,29100575,29154122,29162094,29162227,29163661,29248152,29255380,29307336,29330214,29437827,29469547,29476742,29507014,29508408,29508412,29508419,29547337,29547342,29591395,29657442,29743168,29757365,29768829,29843818,29856717,29893280,29900927,29917394,29925691,29955121,30014010,30024129,30059700,30078705,30080389,30095670,30101585,30104851,30116297,30117278,30137195,30142363,30207092,30217595,30220845,30232088,30245982,30304538,30375966,30393808,30402275,30464514,30528361,30607495,30623138,30642469,30691354,30694079,30704412,30713252,30820691,30824267,30866199,30926945,30955826,31055217,31136569,31142804,31170507,31185936,31207998,31209645,31268887,31278112,31309865,31329806,31410620,31452173,31464993,31480900,31550630,31595731,31626997,31686530,31693613,31762657,31774754,31815979,31820186,31910112,31910164,31911025,31959147,31959426,31971051,31985639,32004948,32014146,32019994,32021471,32022103,32089989,32109652,32151175,32154829,32203869,32219983,32232398,32232766,32232833,32232915,32234463,32265349,32348731,32375574,32388997,32390117,32412986,32413022,32413067,32413156,32413224,32413243,32413247,32413365,32413549,32413774,32413780,32413789,32413797,32413799,32413802,32413824,32413838,32413864,32413875,32413911,32413927,32413930,32413933,32413940,32413943,32413963,32413967,32465888,32467545,32492315,32503534,32531710,32557032,32557038,32557045,32557052,32557060,32557068,32557085,32557097,32557106,32557845,32619385,32623847,32661126,32692881,32699711,32768962,32769976,32811135,32820602,32821296,32827834,32827907,32828309,32838976,32844916,32846172,32848066,32857178,32859675,32889802,32950754,32984460,33007779,33021516,33044935,33047440,33064479,33169354,33175055,33183959,33282341,33344388,33381490,33381498,33381506,33384211,33424541,33472637,33483870,33485179,33495520,33495528,33495533,33495543,33495556,33501796,33513511,33538353,33558165,33563812,33573082,33580835,33586326,33629206,33632328,33636445,33666106,33703226,33707729,33733202,33734594,33759968,33759980,33759986,33759991,33760013,33760031,33774572,33805815,33834604,33857267,33876217,33920225,33924664,33933610,33954297,33974984,34024222,34028910,34041526,34043166,34049306,34063302,34113599,34130573,34138657,34141204,34146843,34172629,34173191,34277365,34305084,34305472,34308747,34314745,34450326,34450832,34451654,34451833,34463180,34464665,34464990,34465063,34466828,34467080,34467405,34472264,34474247,34474535,34475259,34475408,34475655,34486233,34488764,34488979,34490863,34506066,34528110,34528282,34586670,34587054,34587463,34587625,34587685,34587830,34587899,34588909,34614825,34614939,34665207,34666284,34731670,34733993,34753228,34753388,34788776,34796213,34899015,34916699,35142365,35152939,35157148,35161589,35189523,35192779,35215018,35218603,35267599,35280906,35299204,35329352,35329369,35329399,35347138,35351046,35354204,35373737,35393218,35416430,35425269,35426705,35455058,35469543,35485545,35488853,35494969,35536212,35559882,35567288,35567643,35575380,35576517,35591110,35618967,35619334,35626852,35644835,35698744,35713005,35777454,35822408,35825528,35842198,35846084,35863437,35868354,35893947,35920120,35949850,35964767,35977853,35981898,36002162,36015883,36056190,36076007,36083515,36098390,36115155,36120914,36150080,36154414,36207935,36213305,36235782,36247005,36254980,36288221,36321935,36346258,36369553,36379164,36395479,36396523,36403101,36433359,36454751,36463409,36540444,36541990,36542023,36542256,36542265,36586543,36593535,36593872,36593893,36594036,36639202,36693287,36775938,36776490,36777580,36887658,36888680,36906812,36954454,36962064,36966594,36994666,37048385,37058998,37074715,37093371,37095692,37099194,37107297,37108190,37114586,37117100,37122666,37140173,37146709,37151288,37152726,37165981,37167612,37173774,37182940,37186465,37186487,37186611,37187479,37199180,37212635,37215280,37234305,37258381,37267333,37272867,37304496,37322664,37325153,37331275,37352214,37366860,37378327,37405397,37405408,37405427,37405473,37405575,37405594,37405652,37405824,37405828,37406027,37409191,37421953,37454184,37491613,37494003,37525303,37528078,37529175,37589889,37599427,37614535,37625951,37631990,37641472,37645103,37690000,37697742,37703460,37710793,37712928,37714136,37728615,37772684,37868107,37892425,37896060,37897300,37923654,37930842,37951326,37956141,37960490,37964408,37967286,37977384,37987185,37987195,37987210,38005081,38005803,38018365,38023385,38025383,38029201,38051747,38056154,38057887,38060785,38064839,38064928,38070644,38076595,38088892,38094489,38100071,38107432,38108394,38116456,38128590,38143793,38147826,38155240,38194843,38215266,38215346,38229535,38230426,38232155,38242537,38257117,38260409,38260712,38271743,38280193,38283672,38284860,38291508,38307992,38319439,38321288,38321802,38325040,38327539,38329653,38339511,38340369,38352299,38357953,38372175,38399888,38416413,38424362,38440952,38453338,38457123,38479804,38490728,38492435,38499004,38507855,38540115,38541293,38548021,38548743,38552719,38553008,38558243,38566816,38571921,38572177,38572208,38584188,38584859,38585802,38594056,38602086,38610943,38611591,38613748,38626486,38631210,38631943,38639014,38639028,38639044,38639053,38639064,38639070,38639079,38639107,38639952,38646191,38652217,38666385,38670290,38672809,38674161,38675279,38680336,38683959,38685104,38685631,38687108,38688550,38694291,38694327,38695572,38698229,38704477,38713474,38716081,38717204,38728594,38730446,38747696,38747896,38755388,38767358,38773348,38776700,38776775,38784517,38784570,38800238,38812382,38813188,38813290,38814464,38816447,38827338,38828337,38847472,38851853,38852708,38856096,38866780,38870389,38874976,38878580,38879110,38885850,38893487,38898573,38906486,38907707,38912591,38920101,38920758,38925422,38926874,38927532,38927792,38938632,38944681,38945434,38946082,38953278,38954326,38954777,38959730,38962926,38984092,38988341,38992392,38995628,39012666,39015709,39019730,39022774,39024332,39037598,39047336,39048310,39051506,39052778,39060533,39061813,39068161,39075627,39080713,39084032,39084640,39095056,39097198,39097480,39100266,39100497,39100754,39101495,39101842,39102697,39104563,39104566,39106188,39110183,39136380,39138618,39142018,39145149,39152741,39178798,39180600,39185488,39227345,39229603,39237779,39237815,39237944,39238089,39238101,39238145,39241265,39241351,39243641,39249084,39249222,39263276,39265185,39265255,39265389,39265407,39265420,39265815,39265887,39265897,39265908,39265998,39266045,39266063,39266073,39266899,39268514,39269113,39271681,39272413,39272640,39273924,39274122,39274180,39277052,39277843,39277893,39277931,39277968,39278007,39280834,39280865,39280881,39280902,39280909,39280920,39280925,39280938,39280949,39280961,39280980,39280988,39281171,39281233,39281253,39281263,39281276,39281284,39281296,39281305,39281326,39281337,39281353,39281400,39281404,39281408,39281416,39281424,39281439,39281452,39281502,39281512,39281519,39281523,39281524,39281529,39281532,39281537,39281542,39281555,39281558,39281561,39281572,39281871,39281877,39281891,39281897,39281903,39281909,39281929,39281939,39281946,39281962,39281968,39281977,39281981,39281985,39282004,39282020,39282024,39282876,39282893,39282966,39283574,39283881,39283889,39290736,39292947,39299616,39302314,39302452,39302610,39302677,39305344,39315326,39315612,39326862,39335002,39339203,39339768,39341072,39341146,39342359,39342944,39342949,39343150,39343313,39346596,39346603,39346623,39347413,39348519,39359863,39359888,39359932,39360748,39361485,39361583,39361603,39361620,39361831,39361905,39361942,39363151,39364110,39364672,39366404,39366858,39368101,39371122,39372982,39373054,39374915,39376659,39376972,39377274,39377835,39378901,39380402,39380462,39381152,39381608,39383066,39383159,39383282,39390474,39390496,39392673,39393103,39393127,39398643,39414270,39414290,39414352,39414362,39414403,39414412,39414432,39415631,39418144,39418201,39418211,39418296,39418307,39418455,39418471,39418476,39418553,39419310,39430937,39434257,39441159,39445354,39453107,39454694,39471427,39471818,39474538,39477049,39482636,39483684,39484010,39484570,39484825,39486443,39488193,39489690,39490056,39499812,39501885,39503787,39503816,39504270,39508505,39508937,39511543,39511897,39517722,39519271,39523301,39543804,39547900,39550938,39575998,39580868,39583673,39637524,39641000,39643807,39645331,39647439,39653176,39654077,39656006,39683529,39685652,39686748,39687640,39687684,39690607,39691842,39697112,39706771,39728169,39731154,39731497,39732861,39733607,39739103,39739386,39745218,39746557,39747208,39756900,39758446,39786940,39799021,39810163,39826536,39835608,39854736,39858734,39862141,39862411,39869465,39872147,39890266,39890536,39890687,39891631,39891656,39895964,39898688,39898764,39899575,39899655,39901243,39911720,39912242,39916125,39916316,39929496,39929669,39933279,39933297,39933331,39933438,39933460,39934018,39935270,39938094,39940691,39941928,39941940,39950203,39955718,39965895,39967992,39975530,39976173,39980598,39986902,39988808,39992739,39994653,39995896,39996320,40001689,40004302,40005447,40008092,40008447,40009596,40010720,40010809,40010877,40010986,40013678,40015004,40016365,40019158,40019170,40019182,40019219,40019233,40019255,40019294,40019307,40019387,40019909,40020556,40023153,40027354,40034824,40039009,40039438,40044145,40045055,40045741,40047112,40048036,40048518,40048529,40048540,40048552,40048582,40048639,40048692,40048740,40048790,40048847,40049030,40053367,40053389,40053685,40053696,40053773,40054487,40061019,40062246,40068077,40071929,40073442,40074039,40074543,40074568,40074661,40076625,40079095,40081703,40083049,40083245,40083287,40085751,40088493,40093611,40095700,40099656,40100284,40100289,40101282,40101936,40101941,40101985,40103088,40103100,40103500,40103511,40105539,40106741,40109135,40117336,40117610,40117692,40117695,40117729,40117745,40117781,40117784,40117787,40117799,40122084,40123548,40124541,40125044,40125322,40127989,40128624,40128758,40128908,40129381,40129467,40129535,40130372,40130632,40130929,40132919,40133135,40133479,40134250,40134457,40134513,40134595,40135524,40135632,40135782,40136379,40136822,40137375,40137402,40137809,40145584,40147809,40147842,40149671,40155558,40157698,40157983";
            var custIdList = custIds.Split(',').Select(Int32.Parse).ToList();
            var customers = _fraudCaseDetailService.GetTotalAssociations(custIdList);

            var result = GenerateSampleCustomerTotalAssociations(customers);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static IEnumerable<FlaggedAccountTicket> GenerateSampleFlaggedAccountTickets(IEnumerable<FlaggedAccountTicket> details)
        {
            var tickets = new List<FlaggedAccountTicket>();

            for (int i = 1; i < 150; i++)
            {
                tickets.AddRange(details);
            }

            return tickets;
        }

        private static IEnumerable<CustomerDetails> GenerateSampleFlaggedAccountCustomers(IEnumerable<CustomerDetails> details)
        {
            var customers = new List<CustomerDetails>();

            for (int i = 1; i < 250; i++)
            {
                customers.AddRange(details);
            }

            return customers;
        }

        private static IEnumerable<CustomerTotalAssociation> GenerateSampleCustomerTotalAssociations(IEnumerable<CustomerTotalAssociation> details)
        {
            var customers = new List<CustomerTotalAssociation>();

            for (int i = 1; i < 200; i++)
            {
                customers.AddRange(details);
            }

            return customers;
        }

        [HttpPost]
        public ActionResult SaveFileFlaggedAccounts(GetFlaggedAccountTicketCriteria criteria)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/flagged account.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT));
            criteria.SaveChoice = true;
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var excelFileContent = _fraudCaseDetailService.ExportExcelFlaggedAccounts(criteria, template);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        public ActionResult GetBetTypes()
        {
            var data = _fraudCaseDetailService.GetBetTypes();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Arbitrage        
        [HttpPost]
        public ActionResult GetArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetArbitrageTickets(criteria, false);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetArbitrageTicketsGroupSummary(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var aribitrageTickets = _fraudCaseDetailService.GetArbitrageTickets(criteria, true).ToList();

            var custIdList = new List<long>();
            foreach (var item in aribitrageTickets.Select(a => a.Home.Select(t => t.CustId)))
            {
                custIdList.AddRange(item);
            }

            foreach (var item in aribitrageTickets.Select(a => a.Away.Select(t => t.CustId)))
            {
                custIdList.AddRange(item);
            }

            SessionManager.Set(SessionCustomerList, custIdList.Distinct().ToList());

            return new JsonResult { Data = new { Content = aribitrageTickets }, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public JsonResult GetFraudCaseArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            if (criteria.IsOnlyCN88)
            {
                criteria.Site = "CN88";
            }

            var arbitrageCustomerDetails = _fraudCaseDetailService.GetArbitrageCustomerDetails(criteria);

            if (SessionManager.Get<List<long>>(SessionCustomerList) != null)
            {
                var custIdList = SessionManager.Get<List<long>>(SessionCustomerList);

                if (arbitrageCustomerDetails is List<CustomerDetailsForMuchData>)
                {
                    var arbitrageCustomerDetailsMuchData = arbitrageCustomerDetails as List<CustomerDetailsForMuchData>;
                    var totalCustomersCombineWithTickets = arbitrageCustomerDetailsMuchData[0].CustIdList.Join(custIdList.AsEnumerable(),
                                custA => custA,
                                custB => custB,
                                (custA, custB) => custA).Count();
                    arbitrageCustomerDetailsMuchData[0].TotalCustomers = totalCustomersCombineWithTickets;

                    return Json(arbitrageCustomerDetailsMuchData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    arbitrageCustomerDetails = arbitrageCustomerDetails.Join(custIdList.AsEnumerable(),
                                custA => custA.CustId,
                                custB => custB,
                                (custA, custB) => custA);
                }
            }

            return Json(arbitrageCustomerDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFileArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT)) + ".xlsx";
            string[] filterInfo = GenerateFilterInfo(criteria);
            bool isSummary = criteria.GroupId == 0;
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var excelFileContent = _fraudCaseDetailService.ExportExcelArbitrageTickets(criteria, filterInfo, templatePath, isSummary);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        [HttpPost]
        public ActionResult SaveFileArbitrageCustomers(GetArbitrageCustomerCriteria criteria)
        {
            List<long> custIdList = null;
            if (SessionManager.Get<List<long>>(SessionCustomerList) != null && criteria.IsByIPAndDevice)
            {
                custIdList = SessionManager.Get<List<long>>(SessionCustomerList);
            }

            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT)) + ".xlsx";
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            string[] filterInfo = GenerateFilterInfo(criteria);
            var excelFileContent = _fraudCaseDetailService.ExportExcelArbitrageCustomers(criteria, filterInfo, templatePath, custIdList);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        private string[] GenerateFilterInfo(ArbitrageDetailsCriteria criteria)
        {
            string[] filterInfo = new string[10];

            // get Market
            filterInfo[0] = criteria.LiveStatus == -1 ? "All" : (criteria.LiveStatus == 0 ? "Non-Live" : "Live");

            // get Currency
            filterInfo[1] = criteria.CurrencyNames;

            // get Evidence name
            string strCode = string.Empty;
            var otherEvidenceCode = criteria.OtherEvidenceCodes;
            if (otherEvidenceCode != null && criteria.EvidenceCodeList != null)
            {
                strCode = criteria.EvidenceCodeList;
                strCode = strCode.Replace(otherEvidenceCode, "Others");
                strCode = strCode.Replace(',', '/');
            }

            filterInfo[2] = _fraudCaseDetailService.ReplaceFilterEvidences(strCode);

            // From date, to date
            filterInfo[3] = criteria.TransDateFromString != null ? criteria.TransDateFromString.Replace(',', '-') : string.Empty;
            filterInfo[4] = criteria.TransDateToString != null ? criteria.TransDateToString.Replace(',', '-') : string.Empty;

            // get Score/HDP
            filterInfo[5] = criteria.KeyName;

            // Sites
            filterInfo[6] = criteria.Sites;

            filterInfo[7] = criteria.Odds == null ? string.Empty : criteria.Odds.ToString();
            filterInfo[8] = criteria.IsOnlyCN88 ? "Yes" : "No";
            filterInfo[9] = criteria.IsByIPAndDevice ? "Yes" : "No";

            return filterInfo;
        }

        public JsonResult GetScoreHDP(GetScoreHDPCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetScoreHDP(criteria);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Arbitrage       

    }
}