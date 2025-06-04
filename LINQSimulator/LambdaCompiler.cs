using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQSimulator
{
    public static class LambdaCompiler
    {
        public static async Task<Func<object, T>> CompileAsync<T>(string lambda, ScriptOptions options)
        {
            ValidateLambda(lambda);

            var code = $"System.Func<object, {GetTypeName<T>()}> func = {lambda}; func";

            try
            {
                var script = CSharpScript.Create<Func<object, T>>(code, options);
                var result = await script.RunAsync();
                return result.ReturnValue ?? throw new InvalidOperationException("Compilation returned null function");
            }
            catch (CompilationErrorException)
            {
                throw; // Re-throw compilation errors as-is
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to compile lambda: {ex.Message}", ex);
            }
        }

        private static void ValidateLambda(string lambda)
        {
            if (string.IsNullOrWhiteSpace(lambda))
                throw new ArgumentException("Lambda expression cannot be empty");

            // Basic validation - must contain =>
            if (!lambda.Contains("=>"))
                throw new ArgumentException("Lambda expression must contain '=>'");

            // Check for potential security issues
            var dangerousPatterns = new[] { "System.IO", "File.", "Directory.", "Process.", "Assembly." };
            foreach (var pattern in dangerousPatterns)
            {
                if (lambda.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException($"Lambda expression contains potentially dangerous code: {pattern}");
            }
        }

        private static string GetTypeName<T>()
        {
            var type = typeof(T);
            if (type == typeof(object)) return "object";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(int)) return "int";
            if (type == typeof(double)) return "double";
            if (type == typeof(string)) return "string";
            return type.FullName;
        }
    }
}
