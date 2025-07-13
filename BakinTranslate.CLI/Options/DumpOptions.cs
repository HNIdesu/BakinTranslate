using CommandLine;

namespace BakinTranslate.CLI.Options
{
    [Verb("dump")]
    internal class DumpOptions
    {
        [Value(0, Required = true, MetaName = "game_directory")]
        public string GameDirectory { get; set; }
        [Value(1, Required = true, MetaName = "unpack_directory")]
        public string UnpackDirectory { get; set; }
        [Option('o', "output", Required = false)]
        public string OutputDirectory { get; set; }
    }
}
