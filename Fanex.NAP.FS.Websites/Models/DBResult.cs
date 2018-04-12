namespace Fanex.NAP.FS.Websites.Models
{
    using System.Collections;

    public class DBResult
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public Hashtable OutParam { get; set; }
    }
}