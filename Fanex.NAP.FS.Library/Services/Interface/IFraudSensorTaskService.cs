namespace Fanex.NAP.FS.Library.Services
{
    using DTO;
    using System.Collections.Generic;

    public interface IFraudSensorTaskService
    {
        IEnumerable<Match> BigStakeNearGoalExecutePrecondition();

        void BigStakeNearGoalExecuteRule(string matchIds);

        IEnumerable<Match> FlaggedAccountExecutePrecondition();

        void FlaggedAccountExecuteRule(string matchIds);
    }
}
