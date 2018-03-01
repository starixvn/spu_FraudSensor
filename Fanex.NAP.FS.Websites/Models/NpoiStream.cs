using System.IO;

namespace Fanex.NAP.FS.Websites.Models
{
    public class NpoiStream : MemoryStream
    {
        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }
}