namespace Fanex.NAP.FS.WinService.Console
{
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var a = FraudCaseFactory.GetExecutor(new FraudCase { FraudCaseId = (int) FraudCaseId.DailyDataCapture });

            Write(a);

            Pause();
        }

        private static void Pause() => System.Console.ReadLine();

        private static void Write(object str) => System.Console.WriteLine(str);
    }
}