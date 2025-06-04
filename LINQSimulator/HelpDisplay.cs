using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class HelpDisplay
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("LINQ SIMULATOR COMMANDS");
            Console.WriteLine("======================");
            Console.ResetColor();

            var commands = new[]
            {
                ("FILTERING & SELECTION", new[]
                {
                    ("filter/where <lambda>", "Filter items by condition", "filter x => (int)x > 10"),
                    ("select <lambda>", "Transform/project items", "select x => new { Value = x, Square = ((int)x)*((int)x) }"),
                    ("distinct", "Remove duplicate items", "distinct")
                }),

                ("ORDERING & GROUPING", new[]
                {
                    ("orderby/sort [lambda] [desc]", "Sort items", "orderby x => x desc"),
                    ("groupby/group <lambda> [agg]", "Group and aggregate", "groupby x => (int)x % 2 count"),
                    ("", "Available aggregates: count, sum, avg, min, max, items", "")
                }),

                ("QUANTIFIERS & AGGREGATES", new[]
                {
                    ("take <n>", "Take first n items", "take 5"),
                    ("skip <n>", "Skip first n items", "skip 3"),
                    ("any [lambda]", "Check if any item matches", "any x => (int)x > 100"),
                    ("all [lambda]", "Check if all items match", "all x => (int)x > 0"),
                    ("count", "Count items", "count"),
                    ("sum", "Sum numeric values", "sum"),
                    ("avg/average", "Average of numeric values", "avg"),
                    ("min", "Find minimum value", "min"),
                    ("max", "Find maximum value", "max"),
                    ("first [lambda]", "Get first item (optionally matching)", "first x => (int)x > 5"),
                    ("last [lambda]", "Get last item (optionally matching", "last")
                }),

                ("UTILITY COMMANDS", new[]
                {
                    ("export/save <format>", "Export results (csv, json, txt)", "export json"),
                    ("undo", "Undo last operation", "undo"),
                    ("redo", "Redo last undone operation", "redo"),
                    ("reset", "Reset to original data", "reset"),
                    ("help", "Show this help", "help"),
                    ("exit", "Exit the application", "exit")
                })
            };

            foreach (var (category, categoryCommands) in commands)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(category);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(new string('-', category.Length));
                Console.ResetColor();

                foreach (var (command, description, example) in categoryCommands)
                {
                    if (string.IsNullOrEmpty(command))
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"  {description}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"  {command,-25}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($" - {description}");

                        if (!string.IsNullOrEmpty(example))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"    Example: {example}");
                        }
                        Console.ResetColor();
                    }
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("TIPS:");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("• Use (int)x, (double)x, (string)x for type casting");
            Console.WriteLine("• String values in input should be quoted: \"hello\", 'world'");
            Console.WriteLine("• Lambda expressions must use => syntax");
            Console.WriteLine("• Commands are case-insensitive");
            Console.WriteLine("• Use Ctrl+C to cancel current operation");
            Console.ResetColor();
        }
    }
}
