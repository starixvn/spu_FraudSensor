namespace Fanex.NAP.FS.WinService.Console
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            DbSettingProviderManager
               .StartNewSession()
               .UseDefaultConnectionStringProvider()
               .UseDefaultDbSettingProvider(resourcePath:  "~/App_Data", enableWatching: true)
               .Run();


            var a = FraudCaseFactory.GetExecutor(new FraudCase { FraudCaseId = (int) FraudCaseId.Arbitrage });

            a.Execute();

            Write(a);

            Pause();
        }

        private static void Pause() => System.Console.ReadLine();

        private static void Write(object str) => System.Console.WriteLine(str);
    }
}