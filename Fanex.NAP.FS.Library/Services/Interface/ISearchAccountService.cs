namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System.Collections.Generic;

    public interface ISearchAccountService
    {
        IEnumerable<Subscriber> GetSubscribers();
    }
}
