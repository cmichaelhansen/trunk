using System;
using System.IO;
using System.Text;

namespace Utility.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string RemoveFileExtension(this FileInfo fileInfo)
        {
            var position = fileInfo.Name.LastIndexOf('.');

            if (position > 0)
            {
                return fileInfo.Name.Substring(0, position);
            }

            return fileInfo.Name;
        }

        public static string HideSqlSecurity(this string input)
        {
            
            // Turns User Id=sa;Password=1234 
            // into User Id=*****;Password=*****

            input = FindAndReplace(input, "User Id");
            return FindAndReplace(input, "Password");

        }

        private static string FindAndReplace(string input, string searchTerm)
        {
            int start = 0, end = 0;
            var output = new StringBuilder();

            start = input.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase);

            if (start > 0)
            {
                var pos = start;
                string c = string.Empty;
               
                while (c.Equals(";", StringComparison.CurrentCultureIgnoreCase) == false && pos < input.Length)
                {
                    c = input.Substring(pos, 1);
                    end = pos++;
                }

                if (end == input.Length - 1)
                    end++;

                var turnOn = true;

                for (int i = 0; i < input.Length; i++)
                {
                    if (i == start)
                    {
                        turnOn = false;
                        output.Append(string.Format("{0}=*****", searchTerm));
                    }
                    if (i == end)
                        turnOn = true;

                    if (turnOn)
                        output.Append(input.Substring(i, 1));

                }

            }
            else
            {
                output.Append(input);
            }

            return output.ToString();
        }


        public static string FormatPath(this string input)
        {
            if (!input.EndsWith(@"\"))
                input += @"\";

            return input;
        }

        public static string CleanGeneralTerm(this string input)
        {
            // helps remove "(Outdoor)" an anything else specified in parenthesis in the juno source data
            if (input.Contains("("))
            {
                var position = input.IndexOf('(');
                input = input.Remove(position).Trim();
            }
            return input;
        }

        public static string MakeSqlFriendly(this string input)
        {
            return input.Replace(@"'", @"''");
        }

        public static string NullIfEmpty(this string input)
        {
            return string.IsNullOrEmpty(input) ? null : input;
        }
    }
}
