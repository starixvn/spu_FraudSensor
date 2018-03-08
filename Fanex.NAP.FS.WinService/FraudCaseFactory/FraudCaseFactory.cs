namespace Fanex.NAP.FS.WinService
{
    using Library.DTO;
    using System;

    public static class FraudCaseFactory
    {
        public static FraudCaseExecutor GetExecutor(FraudCase fraudCase)
        {
            switch (fraudCase.FraudCaseId)
            {
                case 1:
                    return new BigStakeNearGoalExecutor(fraudCase);

                case 6:
                    return new FlaggedAccountExecutor(fraudCase);

                default:
                    return new GhostExecutor(fraudCase);
            }
        }
    }
}
