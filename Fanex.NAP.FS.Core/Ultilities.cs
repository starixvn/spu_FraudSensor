namespace Fanex.NAP.FS.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class Ultilities
    {
        public static void WriteLog(string filename, IEnumerable<string> body)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.WriteAllLines(filePath, body);
        }
    }
}
