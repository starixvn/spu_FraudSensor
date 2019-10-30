

namespace Fanex.NAP.FS.Library.Core.Utilities
{
    using System;

    public static class DateTimeExtension
    {
        public static DateTime ConvertToFirstTimeInDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }

        public static DateTime ConvertToLastTimeInDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }
    }
}