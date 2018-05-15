namespace Fanex.NAP.FS.WinService.Console
{
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using Fanex.NAP.FS.Library.DTO;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var a = new[]
            {
                new CustomerEvidence { CustId = 5, EvidenceCode = "1.5", EvidenceName = "afdaf"}
            };

            Write(a.ToXmlString());

            Pause();
        }

        private static void Pause() => System.Console.ReadLine();

        private static void Write(object str) => System.Console.WriteLine(str);
    }
}