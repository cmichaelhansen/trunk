using System;
using System.Text;

namespace Utility.ExtensionMethods
{
    public static class TimeSpanExtensions
    {
        public static string NiceFormat(this TimeSpan input)
        {
            // build a nicely formatted string for days, hours, minutes, seconds

            var niceFormat = new StringBuilder();
            
            if (input.Days > 0)
                niceFormat.Append(string.Format("{0} day{1}, ", input.Days, Puralize(input.Days)));
            if (input.Hours > 0)
                niceFormat.Append(string.Format("{0} hour{1}, ", input.Hours, Puralize(input.Hours)));
            if (input.Minutes > 0)
                niceFormat.Append(string.Format("{0} min{1}, ", input.Minutes, Puralize(input.Minutes)));
            if (input.Seconds > 0)
                niceFormat.Append(string.Format("{0} sec{1}", input.Seconds, Puralize(input.Seconds)));

            return niceFormat.ToString();
        }

        private static string Puralize(int source)
        {
            return source == 1 ? "" : "s";
        }
    }
}
