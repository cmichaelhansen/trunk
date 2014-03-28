using System;

namespace Utility.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        // returns MMDDYYYY with leading 0s as necessary for filenames
        public static string FormatNowForFileName(this DateTime input)
        {
            return string.Format("{0}{1}{2}", input.Month.ToString("D2"), input.Day.ToString("D2"), input.Year.ToString("D4"));
        }
    }
}
