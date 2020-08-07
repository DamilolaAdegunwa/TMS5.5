using System;
using System.Collections.Generic;

namespace Egoal.Cryptography
{
    public class PemFormater
    {
        public static string Format(string str, string header, string footer)
        {
            if (str.StartsWith(header))
            {
                return str;
            }

            List<string> lines = new List<string>();
            lines.Add(header);

            int lineMaxLength = 64;
            int pos = 0;
            while (pos < str.Length)
            {
                var count = str.Length - pos < lineMaxLength ? str.Length - pos : lineMaxLength;
                lines.Add(str.Substring(pos, count));
                pos += count;
            }

            lines.Add(footer);

            return string.Join(Environment.NewLine, lines);
        }

        public static string RemoveFormat(string str, string header, string footer)
        {
            if (!str.StartsWith(header))
            {
                return str;
            }

            return str.Replace(header, string.Empty).Replace(footer, string.Empty).Replace(Environment.NewLine, string.Empty);
        }
    }
}
