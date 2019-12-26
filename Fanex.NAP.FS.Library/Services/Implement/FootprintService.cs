namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.Footprint;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Collections.Generic;

    public class FootprintService : IFootprintService
    {
        public IEnumerable<FootprintAssociation> GetFootprintAssociations(int custId)
        {
            return DataService.GetData<FootprintAssociation>(new FootprintAssociationsGetCriteria(custId));
        }
    }
}