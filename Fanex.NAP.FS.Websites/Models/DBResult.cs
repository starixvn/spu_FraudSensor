using System.Collections;

namespace Fanex.NAP.FS.Websites.Models
{
    public class DBResult
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public Hashtable OutParam { get; set; }
    }
}