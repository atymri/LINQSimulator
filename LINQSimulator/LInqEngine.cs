using LinqSimulator;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public class LinqEngine
    {
        private readonly List<object> _originalData = new();
        private IEnumerable<object> _currentResult = Enumerable.Empty<object>();
        private readonly Stack<IEnumerable<object>> _undoStack = new();
        private readonly Stack<IEnumerable<object>> _redoStack = new();
        private readonly CommandProcessor _commandProcessor;
        private readonly ScriptOptions _scriptOptions;

        public LinqEngine()
        {
            _commandProcessor = new CommandProcessor(this);
            _scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(object).Assembly)
                .AddReferences(typeof(Enumerable).Assembly)
                .AddReferences(typeof(Math).Assembly)
                .AddImports("System", "System.Linq", "System.Math");
        }

        public async Task RunAsync()
        {
            ConsoleHelper.WriteTitle("LINQ Simulator v2.0");

            if (!await InitializeDataAsync())
                return;

            ConsoleHelper.WriteInfo("Type 'help' for available commands or 'exit' to quit.");

            await ProcessCommandsAsync();

            ConsoleHelper.WriteInfo("Goodbye!");
        }

        private async Task<bool> InitializeDataAsync()
        {
            while (true)
            {
                ConsoleHelper.WritePrompt("Enter a list of values separated by commas (or 'exit' to quit):");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    ConsoleHelper.WriteWarning("Please enter some data or 'exit' to quit.");
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    return false;

                try
                {
                    var parsedData = DataParser.ParseInput(input);
                    if (parsedData.Count == 0)
                    {
                        ConsoleHelper.WriteWarning("No valid data found. Please try again.");
                        continue;
                    }

                    _originalData.Clear();
                    _originalData.AddRange(parsedData);
                    _currentResult = _originalData;

                    ConsoleHelper.WriteSuccess($"Loaded {parsedData.Count} items successfully.");
                    PrintCurrentResult();
                    return true;
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteError($"Error parsing input: {ex.Message}");
                }
            }
        }

        private async Task ProcessCommandsAsync()
        {
            while (true)
            {
                Console.Write("\nλ> ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                try
                {
                    await _commandProcessor.ProcessAsync(input);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteError($"Unexpected error: {ex.Message}");
                }
            }
        }

        public IEnumerable<object> CurrentResult => _currentResult;
        public ScriptOptions ScriptOptions => _scriptOptions;

        public void UpdateResult(IEnumerable<object> newResult)
        {
            if (newResult == null)
                throw new ArgumentNullException(nameof(newResult));

            _undoStack.Push(_currentResult);
            _redoStack.Clear();
            _currentResult = newResult.ToList(); // Materialize to avoid deferred execution issues
            PrintCurrentResult();
        }

        public void Undo()
        {
            if (_undoStack.Count == 0)
            {
                ConsoleHelper.WriteWarning("Nothing to undo.");
                return;
            }

            _redoStack.Push(_currentResult);
            _currentResult = _undoStack.Pop();
            ConsoleHelper.WriteInfo("Undone.");
            PrintCurrentResult();
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
            {
                ConsoleHelper.WriteWarning("Nothing to redo.");
                return;
            }

            _undoStack.Push(_currentResult);
            _currentResult = _redoStack.Pop();
            ConsoleHelper.WriteInfo("Redone.");
            PrintCurrentResult();
        }

        public void PrintCurrentResult()
        {
            ResultPrinter.Print(_currentResult);
        }

        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            _currentResult = _originalData;
            ConsoleHelper.WriteInfo("Reset to original data.");
            PrintCurrentResult();
        }
    }
}
