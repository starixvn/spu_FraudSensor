namespace Fanex.NAP.FS.Library.Core.ExportExcel
{
    using System.IO;

    internal class ExcelStream : MemoryStream
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