namespace Fanex.NAP.FS.WinService.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            var service = new FraudSensorService();

            System.Console.WriteLine("Start to run window service manually.");

            service.RunOnStart();

            System.Console.WriteLine("Window service is running now.");
            while (true)
            {
            }
        }
    }
}
