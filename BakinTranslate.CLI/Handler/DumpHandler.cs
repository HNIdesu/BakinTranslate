using BakinTranslate.CLI.Options;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace BakinTranslate.CLI.Handler
{
    internal class DumpHandler
    {
        private static readonly HashSet<string> _KeySet = new HashSet<string>();

        private static void ReadStringPostfix(ref string __result, object __instance)
        {
            foreach (var key in Helper.ConvertRawStringToKeys(__result))
                _KeySet.Add(key);
        }
        private static IEnumerable<CodeInstruction> ReadStringTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = AccessTools.Method(typeof(BinaryReader), "ReadString");
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, target);
            yield return new CodeInstruction(OpCodes.Ret);
        }

        public void Handle(DumpOptions options)
        {
            var gameDirectory = options.GameDirectory;
            var unpackDirectory = options.UnpackDirectory;
            var outputPath = options.OutputPath ?? "dic.txt";
            var assembly = Assembly.LoadFrom(Path.Combine(gameDirectory, "data", "common.dll"));
            assembly.GetType("Yukar.Common.Resource.ResourceItem").DeclaredField("sClipboardLoad").SetValue(null, true);
            assembly.GetType("Yukar.Common.Resource.ResourceItem").DeclaredField("sCurrentSourceMode")
                .SetValue(null, Enum.Parse(assembly.GetType("Yukar.Common.Resource.ResourceSource"), "RES_USER"));
            CatalogWrapper.CatalogType = assembly.GetType("Yukar.Common.Catalog");
            ScriptWrapper.ScriptType = assembly.GetType("Yukar.Common.Rom.Script");
            CatalogWrapper.sResourceDir = Path.Combine(unpackDirectory, "unpack.zip\\");
            var harmony = new Harmony("YUKAR.COMMON");
            harmony.Patch(
                Helper.GetMethod(assembly.GetType("Yukar.Common.BinaryReaderWrapper"), "ReadString"),
                transpiler: typeof(DumpHandler).GetDeclaredMethods().First(it => it.Name == "ReadStringTranspiler"),
                postfix: typeof(DumpHandler).GetDeclaredMethods().First(it => it.Name == "ReadStringPostfix"));
            var catalog = CatalogWrapper.init();
            catalog.load();
            using (var sw = new StreamWriter(
                new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read),
                Encoding.UTF8, 4096, leaveOpen: false))
            {
                foreach (var key in _KeySet)
                {
                    var encodedKey = key.Replace("\r\n", "\\n");
                    sw.WriteLine($"{encodedKey}\t{encodedKey}");
                }
            }
        }
    }
}
