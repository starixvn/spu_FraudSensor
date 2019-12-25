namespace Fanex.NAP.FraudDetection.Controller.Api
{
    using Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Models.FraudCase.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Services.Implement;
    using FS.Library.Criteria.Api;
    using FS.Library.Models;
    using FS.Library.Services;
    using FS.Library.Services.Interface;
    using System.Collections.Generic;
    using System.Web.Http;

    public class ApiFPSController : ApiController
    {
        private readonly IFlaggedAccountService _flaggedAccountService;
        private readonly IOfficeIPService _officeIPService;

        public ApiFPSController()
        {
            _flaggedAccountService = new FlaggedAccountService();
            _officeIPService = new OfficeIPService();
        }

        [HttpPost]
        public DBResult AddEvidence([FromBody]FlaggedAccountInsertCriteria criteria)
        {
            criteria.EviCode = criteria.EviCode ?? string.Empty;
            criteria.IndirectAccounts = criteria.IndirectAccounts ?? string.Empty;
            criteria.DirectAccounts = criteria.DirectAccounts ?? string.Empty;
            return _flaggedAccountService.InsertAccounts(criteria);
        }

        [HttpPost]
        public DBResult RemoveEvidence([FromBody]FlaggedAccountDeleteCriteria criteria)
        {
            criteria.EviCode = criteria.EviCode ?? string.Empty;
            criteria.IndirectAccounts = criteria.IndirectAccounts ?? string.Empty;
            criteria.DirectAccounts = criteria.DirectAccounts ?? string.Empty;
            return _flaggedAccountService.DeleteAccounts(criteria);
        }

        [HttpGet]
        public IEnumerable<OfficeIp> GetOfficeIPList()
        {
            var data = _officeIPService.GetIps();
            return data;
        }

        [HttpPost]
        public DBResult InsertOfficeIPTrans([FromBody]IEnumerable<OfficeIpTransaction> criteria)
        {
            var result = _officeIPService.InsertOfficeIpTrans(criteria);
            return result;
        }

        [HttpGet]
        public IEnumerable<int> GetFootprintAssociations(int id)
        {
            return new List<int> { 2,
            3501,
            4355,
            2966534,
            3823657,
            4216230,
            17837457,
            28395129,
            37843284,
            37865203,
            37929624,
            37950957,
            37951800,
            37953146,
            37957389,
            37963527,
            37978026,
            37978785,
            37984354,
            37985440,
            38009570,
            38010574,
            38011425,
            38012582,
            38018358,
            38021324,
            38027589,
            38029966,
            38032900,
            38044572,
            38045448,
            38054138,
            38056355,
            38067027,
            38072423,
            38085139,
            38085501,
            38087801,
            38089538,
            38093022,
            38095660,
            38097843,
            38110843,
            38120936,
            38125758,
            38126579,
            38126615,
            38126699,
            38126737,
            38126763,
            38126793,
            38127955,
            38138206,
            38142175,
            38165331,
            38165600,
            38180216,
            38182354,
            38198946,
            38208227,
            38212591,
            38242155,
            38254068,
            38257060,
            38262187,
            38272635,
            38280464,
            38304031,
            38309781,
            38311098,
            38318388,
            38319910,
            38334839,
            38335840,
            38337805,
            38338749,
            38345857,
            38350033,
            38376845,
            38377687,
            38380218,
            38382894,
            38396647,
            38404517,
            38408559,
            38410331,
            38411243,
            38413753,
            38432009,
            38454228,
            38456652,
            38461450,
            38472843,
            38480777,
            38486257,
            38486445,
            38487173,
            38487353,
            38487831,
            38488102,
            38489912,
            38490079,
            38490122,
            38492252,
            38492275,
            38492536,
            38493458,
            38493552,
            38494816,
            38497449,
            38499227,
            38503480,
            38507513,
            38508918,
            38508932,
            38512680,
            38515090,
            38517592,
            38522713,
            38525493,
            38527168,
            38529240,
            38533255,
            38534768,
            38535089,
            38537834,
            38538075,
            38538243,
            38542809,
            38547305,
            38547867,
            38547886,
            38548146,
            38554208,
            38556396,
            38558004,
            38561370,
            38574773,
            38575103,
            38576731,
            38577253};
        }
    }
}