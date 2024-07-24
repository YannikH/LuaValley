using LuaValley.API.Interface;
using NLua;
using StardewValley.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API
{
    internal class APIManager
    {
        public LuaValley mod;
        public LuaProvider lua;
        private List<LuaAPI> apiList = new List<LuaAPI>();

        public APIManager(LuaValley mod, LuaProvider lua)
        {
            this.mod = mod;
            CreateAPIs();
            this.lua = lua;
        }

        private void CreateAPIs()
        {
            var types = this.GetType().Assembly.GetTypes();
            foreach (Type t in types)
            {
                if (!t.IsAbstract && typeof(LuaAPI).IsAssignableFrom(t))
                {
                    LuaAPI instance = (LuaAPI)Activator.CreateInstance(t, this);
                    apiList.Add(instance);
                }
            }
        }

        public APIType? GetAPI<APIType>()
        {
            foreach ( var api in apiList)
            {
                if (api is APIType)
                {
                    return (APIType)(object)api;
                }
            }
            return (APIType)(object)null;
        }

        public void inject(Lua state)
        {
            foreach (LuaAPI api in apiList)
            {
                state[api.GetName()] = api;
            }
            state["LogFunctions"] = LogFunctions;
        }

        public void reset()
        {
            foreach (LuaAPI api in apiList)
            {
                api.Reset();
            }
        }

        public void LogFunctions(IGameLogger logger)
        {
            foreach (LuaAPI api in apiList)
            {
                api.LogFunctions(logger);
            }
        }
    }
}
