namespace Fanex.NAP.FS.Library.Services
{
    using Core;
    using Data.Repository;
    using NAP.Core.Command;
    using System.Collections.Generic;
    using System;
    using Criteria;

    public class FlaggedAccountService : FSBaseService, IFlaggedAccountService
    {
        public ICommandResult InsertAccounts(ICriteria criteria)
        {
            return UpdateData(criteria);
        }

        public ICommandResult DeleteAccounts(ICriteria criteria)
        {
            return UpdateData(criteria);
        }
    }
}
