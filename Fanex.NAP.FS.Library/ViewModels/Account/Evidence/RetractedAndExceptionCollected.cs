namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence
{
    using Fanex.NAP.FS.Library.Models.Account.Evidence;
    using System.Collections.Generic;

    public class RetractedAndExceptionCollected
    {
        public IEnumerable<AccountEvidence> AccountsRetracted { get; set; }
        public IEnumerable<AccountExceptionEvidence> AccountsException { get; set; }
    }
}