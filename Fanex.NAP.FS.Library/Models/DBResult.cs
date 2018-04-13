namespace Fanex.NAP.FS.Library.Models
{
    using System.Collections;

    public class DBResult
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public Hashtable OutParam { get; set; }
    }
}