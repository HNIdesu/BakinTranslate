using System;

namespace BakinTranslate.CLI
{
    internal class ScriptWrapper
    {
        protected ScriptWrapper() { }
        public static Type ScriptType;
        protected object _InnerObject;
        public static ScriptWrapper Wrap(object obj) => new ScriptWrapper() { _InnerObject = obj };
        public void saveToText(Action<string, string, int> write)
        {
            ScriptType.GetMethod("saveToText", "System.Action`3")
                .Invoke(_InnerObject, new object[] { write });
        }

    }
}
