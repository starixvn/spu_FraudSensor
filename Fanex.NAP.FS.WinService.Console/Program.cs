namespace Fanex.NAP.FS.WinService.Console
{
    using Fanex.NAP.FS.Library.DTO;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var a = FraudCaseFactory.GetExecutor(new FraudCase { FraudCaseId = 95 });

            Write(a);

            Pause();
        }

        private static void Pause() => System.Console.ReadLine();

        private static void Write(object str) => System.Console.WriteLine(str);
    }
}