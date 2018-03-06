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
                    throw new ArgumentOutOfRangeException();

                case 2:
                    throw new ArgumentOutOfRangeException();

                case 3:
                    throw new ArgumentOutOfRangeException();

                case 4:
                    throw new ArgumentOutOfRangeException();

                case 5:
                    throw new ArgumentOutOfRangeException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
