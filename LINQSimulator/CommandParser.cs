using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class CommandParser
    {
        public static (string Command, string Arguments) Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (null, null);

            var parts = input.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length switch
            {
                1 => (parts[0].ToLower(), string.Empty),
                2 => (parts[0].ToLower(), parts[1]),
                _ => (null, null)
            };
        }

        public static (string Lambda, bool IsDescending) ParseOrderBy(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                return ("x => x", false);

            args = args.Trim();

            if (args.Equals("desc", StringComparison.OrdinalIgnoreCase))
                return ("x => x", true);

            if (args.EndsWith(" desc", StringComparison.OrdinalIgnoreCase))
                return (args.Substring(0, args.Length - 5).Trim(), true);

            return (args, false);
        }

        public static (string KeyLambda, string AggregateFunction) ParseGroupBy(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                return (null, "items");

            var parts = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return (parts[0], "items");

            // Find where lambda ends and aggregate begins
            var lastArrow = args.LastIndexOf("=>");
            if (lastArrow == -1)
                return (parts[0], parts.Length > 1 ? parts[^1] : "items");

            // Find the end of the lambda expression
            var afterArrow = lastArrow + 2;
            var lambdaEnd = afterArrow;
            int parenDepth = 0;

            for (int i = afterArrow; i < args.Length; i++)
            {
                char c = args[i];
                if (c == '(') parenDepth++;
                else if (c == ')') parenDepth--;
                else if (c == ' ' && parenDepth == 0)
                {
                    lambdaEnd = i;
                    break;
                }
            }

            if (lambdaEnd == afterArrow)
                lambdaEnd = args.Length;

            var keyLambda = args.Substring(0, lambdaEnd).Trim();
            var aggregateFunction = lambdaEnd < args.Length
                ? args.Substring(lambdaEnd).Trim()
                : "items";

            return (keyLambda, aggregateFunction);
        }
    }

}
