using LinqSimulator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class ResultPrinter
    {
        public static void Print(IEnumerable<object> result)
        {
            Console.WriteLine("\n" + "=".PadRight(50, '='));
            Console.WriteLine("RESULT:");
            Console.WriteLine("=".PadRight(50, '='));

            var items = result.ToList();
            if (items.Count == 0)
            {
                ConsoleHelper.WriteWarning("(empty result set)");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"[{i:000}] {FormatItem(items[i])}");
            }

            Console.WriteLine("=".PadRight(50, '='));
            ConsoleHelper.WriteInfo($"Total items: {items.Count}");
        }

        public static string FormatItem(object item)
        {
            if (item == null) return "null";

            var type = item.GetType();

            // Handle anonymous types
            if (type.IsClass && type.Namespace == null)
            {
                try
                {
                    return JsonConvert.SerializeObject(item, Formatting.None);
                }
                catch
                {
                    return item.ToString() ?? "null";
                }
            }

            // Handle strings
            if (item is string str)
                return $"\"{str}\"";

            return item.ToString() ?? "null";
        }
    }
}
