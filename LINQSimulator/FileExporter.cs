using LinqSimulator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class FileExporter
    {
        public static void Export(IEnumerable<object> data, string format)
        {
            format = format?.ToLower()?.Trim() ?? "json";

            switch (format)
            {
                case "csv":
                    ExportCsv(data);
                    break;
                case "json":
                    ExportJson(data);
                    break;
                case "txt":
                case "text":
                    ExportText(data);
                    break;
                default:
                    throw new ArgumentException($"Unsupported export format: {format}. Use csv, json, or txt.");
            }
        }

        private static void ExportCsv(IEnumerable<object> data)
        {
            var filename = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var csv = GenerateCsv(data);

            File.WriteAllText(filename, csv, Encoding.UTF8);
            ConsoleHelper.WriteSuccess($"Exported to {filename}");

            if (csv.Length < 1000) // Show small results
            {
                Console.WriteLine("\nCSV Content:");
                Console.WriteLine(csv);
            }
        }

        private static void ExportJson(IEnumerable<object> data)
        {
            var filename = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            File.WriteAllText(filename, json, Encoding.UTF8);
            ConsoleHelper.WriteSuccess($"Exported to {filename}");

            if (json.Length < 1000) // Show small results
            {
                Console.WriteLine("\nJSON Content:");
                Console.WriteLine(json);
            }
        }

        private static void ExportText(IEnumerable<object> data)
        {
            var filename = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var content = new StringBuilder();

            foreach (var item in data)
            {
                content.AppendLine(ResultPrinter.FormatItem(item));
            }

            File.WriteAllText(filename, content.ToString(), Encoding.UTF8);
            ConsoleHelper.WriteSuccess($"Exported to {filename}");
        }

        private static string GenerateCsv(IEnumerable<object> data)
        {
            var csv = new StringBuilder();
            var items = data.ToList();

            if (items.Count == 0)
                return string.Empty;

            // Check if we have anonymous objects for header generation
            var firstItem = items.First();
            var firstType = firstItem?.GetType();

            if (firstType != null && firstType.IsClass && firstType.Namespace == null)
            {
                // Generate header for anonymous types
                var properties = firstType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var headers = properties.Select(p => EscapeCsvValue(p.Name));
                csv.AppendLine(string.Join(",", headers));

                // Generate data rows
                foreach (var item in items)
                {
                    if (item == null)
                    {
                        csv.AppendLine(string.Join(",", properties.Select(_ => "")));
                        continue;
                    }

                    var values = properties.Select(p =>
                        EscapeCsvValue(p.GetValue(item)?.ToString() ?? ""));
                    csv.AppendLine(string.Join(",", values));
                }
            }
            else
            {
                // Simple values - no header
                foreach (var item in items)
                {
                    csv.AppendLine(EscapeCsvValue(item?.ToString() ?? ""));
                }
            }

            return csv.ToString();
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            if (value.Contains("\"") || value.Contains(",") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
