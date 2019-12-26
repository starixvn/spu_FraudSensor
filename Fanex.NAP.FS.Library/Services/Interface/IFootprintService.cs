namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.DTO;
    using System.Collections.Generic;

    public interface IFootprintService
    {
        IEnumerable<FootprintAssociation> GetFootprintAssociations(int custId);
    }
}