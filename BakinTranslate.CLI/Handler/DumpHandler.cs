using BakinTranslate.CLI.Common;
using BakinTranslate.CLI.Options;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;

namespace BakinTranslate.CLI.Handler
{
    internal class DumpHandler
    {
        private static string CurrentFilePath;
        private static readonly Dictionary<string, List<StringEntry>> StringDict =
            new Dictionary<string, List<StringEntry>>();

        private static void ReadStringPrefix(object __instance)
        {
            var binaryReaderWrapperType = __instance.GetType();
            var currentOffsetBias = (long)binaryReaderWrapperType.DeclaredField("currentOffsetBias").GetValue(__instance);
            var position = currentOffsetBias + ((BinaryReader)__instance).BaseStream.Position;
            ReadStringPosition = position;
        }

        private static void ReadStringPostfix(ref string __result, object __instance)
        {
            var binaryReaderWrapperType = __instance.GetType();
            var currentOffsetBias = (long)binaryReaderWrapperType.DeclaredField("currentOffsetBias").GetValue(__instance);
            var position = currentOffsetBias + ((BinaryReader)__instance).BaseStream.Position;
            var length = position - ReadStringPosition;
            var value = __result.Replace("\r\n", "<br>");
            StringDict[CurrentFilePath].Add(
                new StringEntry(ReadStringPosition, (int)length, value, value));
        }

        [ThreadStatic]
        private static long ReadStringPosition;
        public void Handle(DumpOptions options)
        {
            var gameDirectory = options.GameDirectory;
            var unpackDirectory = options.UnpackDirectory;
            var outputDirectory = options.OutputDirectory ?? Path.GetFullPath("output");
            var assembly = Assembly.LoadFrom(Path.Combine(gameDirectory, "data", "common.dll"));
            assembly.GetType("Yukar.Common.Resource.ResourceItem").DeclaredField("sClipboardLoad").SetValue(null, true);
            assembly.GetType("Yukar.Common.Resource.ResourceItem").DeclaredField("sCurrentSourceMode")
                .SetValue(null, Enum.Parse(assembly.GetType("Yukar.Common.Resource.ResourceSource"), "RES_USER"));
            CatalogWrapper.CatalogType = assembly.GetType("Yukar.Common.Catalog");
            ScriptWrapper.ScriptType = assembly.GetType("Yukar.Common.Rom.Script");
            CatalogWrapper.sResourceDir = Path.Combine(unpackDirectory, "unpack.zip");
            var harmony = new Harmony("YUKAR.COMMON");
            harmony.Patch(assembly.GetType("Yukar.Common.BinaryReaderWrapper").GetMethod("ReadString"),
                postfix: typeof(DumpHandler).GetDeclaredMethods().First(it => it.Name == "ReadStringPostfix"),
                prefix: typeof(DumpHandler).GetDeclaredMethods().First(it => it.Name == "ReadStringPrefix"));
            foreach (var file in Directory.EnumerateFiles(
                unpackDirectory, "*.rbr", SearchOption.AllDirectories))
            {
                CurrentFilePath = Path.GetFullPath(file).Substring(Path.GetFullPath(unpackDirectory).Length + 1);
                StringDict[CurrentFilePath] = new List<StringEntry>();
                using (var fs = File.OpenRead(file))
                {
                    var catalog = CatalogWrapper.init(false);
                    var overwrite = Enum.Parse(assembly.GetType("Yukar.Common.Catalog+OVERWRITE_RULES"), "NEVER");
                    catalog.load(fs, overwrite);
                }
                CurrentFilePath = null;
            }
            var serializer = new SerializerBuilder().Build();
            foreach (var entry in StringDict)
            {
                var savePath = Path.Combine(outputDirectory, entry.Key + ".patch");
                var saveDirectory = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(saveDirectory))
                    Directory.CreateDirectory(saveDirectory);
                var yaml = serializer.Serialize(entry.Value);
                File.WriteAllText(savePath, yaml);
            }

        }
    }
}
