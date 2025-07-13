using System;
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
    }
}
