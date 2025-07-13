using CommandLine;

namespace BakinTranslate.CLI.Options
{
    [Verb("patch")]
    internal class PatchOptions
    {
        [Value(0, Required = true, MetaName = "unpack_directory")]
        public string UnpackDirectory { get; set; }
        [Value(1, Required = true, MetaName = "patch_directory")]
        public string PatchDirectory { get; set; }
    }
}
