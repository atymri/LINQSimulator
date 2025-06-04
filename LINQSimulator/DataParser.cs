using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class DataParser
    {
        public static List<object> ParseInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<object>();

            var items = new List<object>();
            var parts = SplitRespectingQuotes(input);

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                items.Add(ParseValue(trimmed));
            }

            return items;
        }

        private static string[] SplitRespectingQuotes(string input)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;
            char quoteChar = '"';

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if ((c == '"' || c == '\'') && !inQuotes)
                {
                    inQuotes = true;
                    quoteChar = c;
                    current.Append(c);
                }
                else if (c == quoteChar && inQuotes)
                {
                    inQuotes = false;
                    current.Append(c);
                }
                else if (c == ',' && !inQuotes)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
                parts.Add(current.ToString());

            return parts.ToArray();
        }

        private static object ParseValue(string value)
        {
            // Handle quoted strings
            if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                (value.StartsWith("'") && value.EndsWith("'")))
            {
                return value.Substring(1, value.Length - 2);
            }

            // Try boolean
            if (bool.TryParse(value, out bool boolValue))
                return boolValue;

            // Try integer
            if (int.TryParse(value, out int intValue))
                return intValue;

            // Try decimal
            if (decimal.TryParse(value, out decimal decimalValue))
                return decimalValue;

            // Try double
            if (double.TryParse(value, out double doubleValue))
                return doubleValue;

            // Default to string
            return value;
        }
    }
}
