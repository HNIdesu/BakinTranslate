using BakinTranslate.CLI.Handler;
using BakinTranslate.CLI.Options;
using CommandLine;
using System;

namespace BakinTranslate.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var options = Parser.Default.ParseArguments(args, new Type[] { 
                typeof(DumpOptions)
            }).Value;
            if (options is DumpOptions dumpOptions)
                new DumpHandler().Handle(dumpOptions);

        }
    }
}
