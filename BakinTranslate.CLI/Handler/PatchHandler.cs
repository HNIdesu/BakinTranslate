using BakinTranslate.CLI.Common;
using BakinTranslate.CLI.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace BakinTranslate.CLI.Handler
{
    internal class PatchHandler
    {
        public void Handle(PatchOptions options)
        {
            var unpackDirectory = options.UnpackDirectory;
            var patchDirectory = options.PatchDirectory;
            var deserializer = new DeserializerBuilder().Build();
            foreach (var patchFile in Directory.EnumerateFiles(patchDirectory, "*.patch", SearchOption.AllDirectories))
            {
                var relativePath = Path.ChangeExtension(
                    Path.GetFullPath(patchFile).Substring(
                        Path.GetFullPath(patchDirectory).Length + 1),null);
                var outputFilePath = Path.Combine(unpackDirectory, relativePath);
                var inputFilePath = outputFilePath + ".bak";
                if(!File.Exists(inputFilePath))
                    File.Copy(outputFilePath, inputFilePath);
                var stringEntryList = deserializer.Deserialize<List<StringEntry>>(File.ReadAllText(patchFile))
                    .Where(it => it.Value != it.NewValue);
                using (var br = new BinaryReader(File.OpenRead(inputFilePath), Encoding.UTF8, leaveOpen: false))
                {
                    using (var bw = new BinaryWriter(File.OpenWrite(outputFilePath), Encoding.UTF8, leaveOpen: false))
                    {
                        var regionMap = new SortedDictionary<long, string>();
                        foreach (var stringEntry in stringEntryList)
                        {
                            regionMap[stringEntry.Offset] = stringEntry.NewValue;
                            regionMap[stringEntry.Offset + stringEntry.Length] = null;
                        }
                        if (!regionMap.ContainsKey(0))
                            regionMap[0] = null;
                        if (!regionMap.ContainsKey(br.BaseStream.Length))
                            regionMap[br.BaseStream.Length] = null;
                        var regionList = regionMap.ToList();
                        for (var i = 0; i < regionList.Count - 1; i++)
                        {
                            var regionEntry = regionList[i];
                            if (regionEntry.Value == null)
                                bw.Write(br.ReadBytes((int)(regionList[i + 1].Key - regionList[i].Key)));
                            else
                            {
                                br.BaseStream.Seek(regionList[i + 1].Key - regionList[i].Key, SeekOrigin.Current);
                                bw.Write(regionEntry.Value.Replace("<br>", "\r\n"));
                            }
                        }
                    }
                }
            }
        }
    }
}
