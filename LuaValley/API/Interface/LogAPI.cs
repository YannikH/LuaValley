using LuaValley.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Interface
{
    internal class LogAPI : LuaAPI
    {
        //LuaConsole console;
        public LogAPI(APIManager api) : base(api, "LogAPI")
        {
            //console = new LuaConsole(api);
        }

        public void Info(string content)
        {
            api.mod.Monitor.Log(content, LogLevel.Info);
        }

        public void Error(string content)
        {
            api.mod.Monitor.Log(content, LogLevel.Error);
        }

        public void Warn(string content)
        {
            api.mod.Monitor.Log(content, LogLevel.Warn);
        }

        public void Inspect(object e)
        {
            if (e == null)
            {
                api.mod.Monitor.Log("Attempted to inspect null object", LogLevel.Warn);
                return;
            }
            Type myType = e.GetType();
            api.mod.Monitor.Log("Inspecting type: " + myType.Name, LogLevel.Info);
            foreach (MethodInfo method in myType.GetMethods())
            {
                api.mod.Monitor.Log("   Inspecting method: " + method.Name, LogLevel.Info);
                foreach (ParameterInfo param in method.GetParameters())
                {
                    api.mod.Monitor.Log("       Parameter: \"" + param.Name + "\", type: " + param.ParameterType, LogLevel.Info);
                }
                api.mod.Monitor.Log("       Return type: " + method.ReturnType, LogLevel.Info);
            }

            foreach (PropertyInfo prop in myType.GetProperties())
            {
                string output = "   Property: \"" + prop.Name + "\", type: " + prop.PropertyType;
                api.mod.Monitor.Log(output, LogLevel.Info);
            }
        }
    }
}
