namespace Fanex.NAP.FS.Library.Models
{
    using Newtonsoft.Json;
    using System.Collections;

    public class DBResult
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        [JsonIgnore]
        public Hashtable OutParam { get; set; }
    }
}