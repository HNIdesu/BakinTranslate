using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BakinTranslate.CLI
{
    internal class CatalogWrapper
    {
        public static Type CatalogType;
        protected object _InnerObject;
        public static CatalogWrapper Wrap(object obj) => new CatalogWrapper() { _InnerObject = obj };
        public static string sResourceDir
        {
            get => (string)CatalogType.GetField("sResourceDir").GetValue(null);
            set => CatalogType.GetField("sResourceDir").SetValue(null, value);
        }
        protected CatalogWrapper() { }
        protected static object sInstance
        {
            set => CatalogType.GetField("sInstance").SetValue(null, value);
        }
        public static CatalogWrapper init(bool inIsSrcNewGame = false)
        {
            var obj =  CatalogType.GetConstrctor("System.Boolean").Invoke(new object[] { inIsSrcNewGame });
            return Wrap(obj);
        }
        
        public int load(bool autoAddNewResource = true, string useBackup = null, bool skipSystemResources = false)
        {
            return (int)CatalogType.GetMethod("load",
                "System.Boolean",
                "System.String",
                "System.Boolean").Invoke(_InnerObject, new object[] { autoAddNewResource, useBackup, skipSystemResources });
        }
        public int load(Stream stream, object overwrite, bool ignoreMissing = false)
        {
            sInstance = _InnerObject;
            return (int)CatalogType.GetMethod("load",
               "System.IO.Stream",
               "Yukar.Common.Catalog+OVERWRITE_RULES",
               "System.Boolean").Invoke(_InnerObject, new object[] { stream, overwrite, ignoreMissing });
        }
        public List<object> getFullList()
        {
            var list = new List<object>();
            IEnumerable rawList = (IEnumerable)CatalogType.GetMethod("getFullList").Invoke(_InnerObject, new object[] {});
            foreach (var item in rawList)
                list.Add(item);
            return list;
        }
    }
}
