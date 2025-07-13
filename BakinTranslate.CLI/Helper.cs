using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BakinTranslate.CLI
{
    internal static class Helper
    {
        public static ConstructorInfo GetConstrctor(this Type type,params string[] args)
        {
            return type.GetTypeInfo().DeclaredConstructors.First(m =>
            {
                var parameters = m.GetParameters();
                return parameters.Length == args.Length &&
                    parameters.Zip(args, (p1, p2) => p1.ParameterType.FullName.StartsWith(p2))
                        .All(it => it);
            });
        }

        public static MethodInfo GetMethod(this Type type, string name, params string[] args)
        {

            return type.GetTypeInfo().DeclaredMethods.First(m =>
            {
                var parameters = m.GetParameters();
                return m.Name == name &&
                parameters.Length == args.Length &&
                    parameters.Zip(args, (p1, p2) => p1.ParameterType.FullName.StartsWith(p2))
                        .All(it => it);
            });
        }

        public static IEnumerable<string> ConvertRawStringToKeys(string text)
        {
            if (text.Contains('\t'))
            {
                string[] array = text.Split('\n');
                for (int i = 0; i < array.Length; i++)
                {
                    string[] array2 = array[i].Split('\t');
                    for (int j = 1; j < array2.Length; j++)
                        yield return array2[j].Replace("\\n", "\r\n");
                }
            }
            else if(text.Contains("\r\n"))
                yield return text;
            else if (text.Contains("\n"))
                yield return text.Replace("\n", "\r\n");
            else if (text.Contains("\\n"))
                yield return text.Replace("\\n", "\r\n");
            else
                yield return text;
        }
    }
}
