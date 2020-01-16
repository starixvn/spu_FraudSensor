namespace Fanex.NAP.FraudDetection.Controller.Api
{
    using Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.DTO;
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
        private readonly IFootprintService _footprintService;

        public ApiFPSController()
        {
            _flaggedAccountService = new FlaggedAccountService();
            _officeIPService = new OfficeIPService();
            _footprintService = new FootprintService();
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
            return _officeIPService.GetIps();
        }

        [HttpPost]
        public DBResult InsertOfficeIPTrans([FromBody]IEnumerable<OfficeIpTransaction> criteria)
        {
            return _officeIPService.InsertOfficeIpTrans(criteria);
        }

        [HttpGet]
        public IEnumerable<FootprintAssociation> GetFootprintAssociations(int id)
        {
            return _footprintService.GetFootprintAssociations(id);
        }
    }
}