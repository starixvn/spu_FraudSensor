using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Core
{
   public static class Ultilities
    {
        public static void WriteLog(string filename, IEnumerable<string> body)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.WriteAllLines(filePath, body);
        }
    }
}
