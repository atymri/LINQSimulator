using LinqSimulator;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public class CommandProcessor
    {
        private readonly LinqEngine _engine;
        private readonly Dictionary<string, Func<string, Task>> _commands;

        public CommandProcessor(LinqEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _commands = InitializeCommands();
        }

        private Dictionary<string, Func<string, Task>> InitializeCommands()
        {
            return new Dictionary<string, Func<string, Task>>(StringComparer.OrdinalIgnoreCase)
            {
                ["filter"] = FilterAsync,
                ["where"] = FilterAsync, // Alias
                ["select"] = SelectAsync,
                ["orderby"] = OrderByAsync,
                ["sort"] = OrderByAsync, // Alias
                ["groupby"] = GroupByAsync,
                ["group"] = GroupByAsync, // Alias
                ["any"] = AnyAsync,
                ["all"] = AllAsync,
                ["count"] = CountAsync,
                ["sum"] = SumAsync,
                ["avg"] = AvgAsync,
                ["average"] = AvgAsync, // Alias
                ["min"] = MinAsync,
                ["max"] = MaxAsync,
                ["first"] = FirstAsync,
                ["last"] = LastAsync,
                ["help"] = _ =>
                {
                    HelpDisplay.Show();
                    return Task.FromResult<IEnumerable<object>>(Enumerable.Empty<object>());
                }

            };
        }

        public async Task ProcessAsync(string input)
        {
            var parts = CommandParser.Parse(input);
            if (parts.Command == null)
            {
                ConsoleHelper.WriteWarning("Invalid command format.");
                return;
            }

            if (_commands.TryGetValue(parts.Command, out var handler))
            {
                await handler(parts.Arguments);
            }
            else
            {
                ConsoleHelper.WriteWarning($"Unknown command '{parts.Command}'. Type 'help' for available commands.");
            }
        }

        private async Task FilterAsync(string lambda)
        {
            if (string.IsNullOrWhiteSpace(lambda))
            {
                ConsoleHelper.WriteWarning("Filter requires a lambda expression. Example: filter x => (int)x > 10");
                return;
            }

            try
            {
                var predicate = await LambdaCompiler.CompileAsync<bool>(lambda, _engine.ScriptOptions);
                var result = _engine.CurrentResult.Where(predicate);
                _engine.UpdateResult(result);
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "filter", "filter x => (int)x > 10");
            }
        }

        private async Task SelectAsync(string lambda)
        {
            if (string.IsNullOrWhiteSpace(lambda))
            {
                ConsoleHelper.WriteWarning("Select requires a lambda expression. Example: select x => new { Value = x, Square = ((int)x) * ((int)x) }");
                return;
            }

            try
            {
                var selector = await LambdaCompiler.CompileAsync<object>(lambda, _engine.ScriptOptions);
                var result = _engine.CurrentResult.Select(selector);
                _engine.UpdateResult(result);
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "select", "select x => new { Value = x, Double = ((int)x) * 2 }");
            }
        }

        private async Task OrderByAsync(string args)
        {
            var (lambda, isDescending) = CommandParser.ParseOrderBy(args);

            try
            {
                var keySelector = await LambdaCompiler.CompileAsync<object>(lambda, _engine.ScriptOptions);
                var result = isDescending
                    ? _engine.CurrentResult.OrderByDescending(keySelector)
                    : _engine.CurrentResult.OrderBy(keySelector);
                _engine.UpdateResult(result);
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "orderby", "orderby x => x or orderby x => (int)x desc");
            }
        }

        private async Task GroupByAsync(string args)
        {
            var (keyLambda, aggregateFunction) = CommandParser.ParseGroupBy(args);

            if (string.IsNullOrWhiteSpace(keyLambda))
            {
                ConsoleHelper.WriteWarning("GroupBy requires a key selector. Example: groupby x => (int)x % 2 count");
                return;
            }

            try
            {
                var keySelector = await LambdaCompiler.CompileAsync<object>(keyLambda, _engine.ScriptOptions);
                var groups = _engine.CurrentResult.GroupBy(keySelector);
                var result = GroupByAggregator.Aggregate(groups, aggregateFunction);
                _engine.UpdateResult(result);
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "groupby", "groupby x => (int)x % 2 count");
            }
        }

        private void Take(string args)
        {
            if (!ValidationHelper.TryParsePositiveInt(args, out int count))
            {
                ConsoleHelper.WriteWarning("Take requires a positive integer. Example: take 5");
                return;
            }

            var result = _engine.CurrentResult.Take(count);
            _engine.UpdateResult(result);
        }

        private void Skip(string args)
        {
            if (!ValidationHelper.TryParseNonNegativeInt(args, out int count))
            {
                ConsoleHelper.WriteWarning("Skip requires a non-negative integer. Example: skip 3");
                return;
            }

            var result = _engine.CurrentResult.Skip(count);
            _engine.UpdateResult(result);
        }

        private void Distinct()
        {
            var result = _engine.CurrentResult.Distinct();
            _engine.UpdateResult(result);
        }

        private async Task AnyAsync(string lambda)
        {
            try
            {
                bool result;
                if (string.IsNullOrWhiteSpace(lambda))
                {
                    result = _engine.CurrentResult.Any();
                }
                else
                {
                    var predicate = await LambdaCompiler.CompileAsync<bool>(lambda, _engine.ScriptOptions);
                    result = _engine.CurrentResult.Any(predicate);
                }
                ConsoleHelper.WriteResult(result.ToString());
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "any", "any x => (int)x > 5");
            }
        }

        private async Task AllAsync(string lambda)
        {
            try
            {
                bool result;
                if (string.IsNullOrWhiteSpace(lambda))
                {
                    result = !_engine.CurrentResult.Any(); // All items satisfy empty condition only if no items
                }
                else
                {
                    var predicate = await LambdaCompiler.CompileAsync<bool>(lambda, _engine.ScriptOptions);
                    result = _engine.CurrentResult.All(predicate);
                }
                ConsoleHelper.WriteResult(result.ToString());
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "all", "all x => (int)x > 0");
            }
        }

        private Task CountAsync(string lambda)
        {
            try
            {
                var count = string.IsNullOrWhiteSpace(lambda)
                    ? _engine.CurrentResult.Count()
                    : _engine.CurrentResult.Count(x => x != null);
                ConsoleHelper.WriteResult(count.ToString());
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error counting: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task SumAsync(string args)
        {
            try
            {
                var sum = NumericHelper.Sum(_engine.CurrentResult);
                ConsoleHelper.WriteResult(sum.ToString("F2"));
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error calculating sum: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task AvgAsync(string args)
        {
            try
            {
                var avg = NumericHelper.Average(_engine.CurrentResult);
                ConsoleHelper.WriteResult(avg.ToString("F2"));
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error calculating average: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task MinAsync(string args)
        {
            try
            {
                var min = NumericHelper.Min(_engine.CurrentResult);
                ConsoleHelper.WriteResult(min?.ToString() ?? "null");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error finding minimum: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task MaxAsync(string args)
        {
            try
            {
                var max = NumericHelper.Max(_engine.CurrentResult);
                ConsoleHelper.WriteResult(max?.ToString() ?? "null");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error finding maximum: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private async Task FirstAsync(string lambda)
        {
            try
            {
                object result;
                if (string.IsNullOrWhiteSpace(lambda))
                {
                    result = _engine.CurrentResult.FirstOrDefault();
                }
                else
                {
                    var predicate = await LambdaCompiler.CompileAsync<bool>(lambda, _engine.ScriptOptions);
                    result = _engine.CurrentResult.FirstOrDefault(predicate);
                }
                ConsoleHelper.WriteResult(ResultPrinter.FormatItem(result));
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "first", "first x => (int)x > 5");
            }
        }

        private async Task LastAsync(string lambda)
        {
            try
            {
                object result;
                if (string.IsNullOrWhiteSpace(lambda))
                {
                    result = _engine.CurrentResult.LastOrDefault();
                }
                else
                {
                    var predicate = await LambdaCompiler.CompileAsync<bool>(lambda, _engine.ScriptOptions);
                    result = _engine.CurrentResult.LastOrDefault(predicate);
                }
                ConsoleHelper.WriteResult(ResultPrinter.FormatItem(result));
            }
            catch (Exception ex)
            {
                HandleCompilationError(ex, "last", "last x => (int)x < 10");
            }
        }

        private void Export(string format)
        {
            try
            {
                FileExporter.Export(_engine.CurrentResult, format);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Export failed: {ex.Message}");
            }
        }

        private Task<IEnumerable<object>> ShowHelpAsync()
        {
            HelpDisplay.Show();
            return Task.FromResult<IEnumerable<object>>(Enumerable.Empty<object>());
        }

        private static void HandleCompilationError(Exception ex, string command, string example)
        {
            if (ex is CompilationErrorException cex)
            {
                ConsoleHelper.WriteError("Compilation error:");
                foreach (var diagnostic in cex.Diagnostics)
                {
                    ConsoleHelper.WriteError($"  {diagnostic}");
                }
                ConsoleHelper.WriteInfo($"Example: {command} {example}");
            }
            else
            {
                ConsoleHelper.WriteError($"Error in {command}: {ex.Message}");
                ConsoleHelper.WriteInfo($"Example: {command} {example}");
            }
        }
    }
}
