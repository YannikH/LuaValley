using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Util
{
    internal class Var: LuaAPI
    {
        Dictionary<object, Dictionary<string, object>> varStorage = new Dictionary<object, Dictionary<string, object>>();
        public Var(APIManager api): base(api,"Var") { }

        private Dictionary<string, object> GetStore(object store)
        {
            if (store == null) return null;
            if (varStorage.ContainsKey(store)) return varStorage[store];
            varStorage[store] = new Dictionary<string, object>();
            return varStorage[store];
        }

        public string GetStoreKey(object store)
        {
            if (store is string inputStr && inputStr  == "lv_global") return "lv_global";

            return "";
        }

        public void Set(object store, string key, object value)
        {
            var storage = GetStore(store);
            storage[key] = value;
        }

        public void Set(string key, object value)
        {
            Set("lv_global", key, value);
        }

        public object Get(object store, string key, object fallback = null)
        {
            if (store == null) return null;
            var storage = GetStore(store);
            return storage.ContainsKey(key) ? storage[key] : null;
        }

        public object Get(string key, object fallback = null)
        {
            return Get("lv_global", key, fallback);
        }
    }
}
