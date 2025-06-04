using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LINQSimulator;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace LinqSimulator
{
    public class Program
    {
        private static readonly LinqEngine _engine = new();

        public static async Task Main()
        {
            await _engine.RunAsync();
        }
    }

}