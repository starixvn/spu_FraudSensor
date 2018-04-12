namespace Fanex.NAP.FS.WinService
{
    using Library.DTO;

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

                case 8:
                    return new DelayGoalExecutor(fraudCase);

                default:
                    return new GhostExecutor(fraudCase);
            }
        }
    }
}