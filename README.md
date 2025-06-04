# LINQ Simulator

LINQ Simulator is an interactive C# console application designed to let you experiment with LINQ queries in real time. Instead of writing a full project and recompiling every time you want to tweak a query, this tool uses Roslyn scripting under the hood to dynamically compile and execute your lambda expressions against an in-memory collection of values.

## How It Works

Behind the scenes, the program:

- Parses a comma-separated list of inputs (integers, doubles, or strings) and stores them as object values
- Maintains an active "current result" collection, which can be transformed by executing commands like `filter`, `select`, `orderby`, `groupby`, `take`, `skip`, `distinct`, `any`, and `all`
- Supports chaining multiple commands in sequence—every new operation pushes the previous state onto an undo stack, so you can type `undo`/`redo` to move backward or forward through your query history
- Allows grouping with aggregates (e.g. `count`, `sum`, `avg`, `min`, `max`), and pretty-prints anonymous types or grouped results in the console
- Exports the current result set as CSV or JSON files via a simple `export csv` or `export json` command

## Dynamic Lambda Compilation

Because it uses `Microsoft.CodeAnalysis.CSharp.Scripting`, you can write lambdas like:

```csharp
filter x => (int)x > 10
select x => new { Value = x, Square = ((int)x) * ((int)x) }
groupby x => ((int)x) % 2 count
```

and see immediate results—no additional compilation steps required.

## Project Structure

On the project page, you'll find:

- A single Visual Studio solution with a console-app (`Program.cs`) that wires up the REPL loop and command parser
- A `ScriptOptions` setup that pre-loads `System.Linq`, so all standard LINQ operators (`Where`, `Select`, `OrderBy`, etc.) are available at runtime
- Helper methods for robust error handling (compilation errors are caught and reported, with hints about quoting string literals), plus logic to convert between int/double and string inputs automatically

## How to Run

1. Clone the repo and open it in Visual Studio (or any IDE that supports .NET Core)
2. Add the following NuGet packages:
   - `Microsoft.CodeAnalysis.CSharp.Scripting` (for dynamic lambda compilation)
   - `Newtonsoft.Json` (for JSON export)
3. Build and launch the console app. You'll be prompted to paste a list of comma-separated values
4. After that, type `help` to see all available commands

## Use Cases

Whether you're learning LINQ for the first time or you simply want a convenient "playground" to prototype queries without writing full data models or setting up a database, LINQ Simulator makes it easy to explore filtering, projection, grouping, sorting, pagination, and more—all in one lightweight console tool.
