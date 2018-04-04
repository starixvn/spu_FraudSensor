using NPOI.XSSF.UserModel;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Websites.ExcelConfig
{
    public static class Utilities
    {
        public static XSSFRichTextString GetLegend(this XSSFRichTextString source, string key, string value)
        {
            source.Append(key + ": ", new XSSFFont { IsBold = true });
            source.Append(value);

            return source;
        }
    }
}