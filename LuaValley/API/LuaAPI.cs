using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API
{
    internal abstract class LuaAPI
    {
        protected APIManager api;
        private string apiName;
        protected LuaAPI(APIManager api, string name)
        {
            this.apiName = name;
            this.api = api;
        }

        public string GetName()
        {
            return this.apiName;
        }

        public virtual void Reset() { }

        public void LogFunctions()
        {
            Type myType = GetType();
            api.mod.Monitor.Log("Logging API: " + apiName, LogLevel.Info);
            foreach (MethodInfo method in myType.GetMethods())
            {
                if (!method.IsPublic || method.DeclaringType != GetType()) continue;
                string methodString = "     ";
                methodString += apiName + ":" + method.Name + "(";
                bool first = true;
                foreach (ParameterInfo param in method.GetParameters())
                {
                    
                    if (first)
                    {
                        first = false;
                    } else
                    {
                        methodString += ", ";
                    }
                    methodString += param.ParameterType + " " + param.Name;
                    if (param.DefaultValue != DBNull.Value)
                    {
                        if (param.DefaultValue == null || param.DefaultValue == "")
                        {
                            methodString += "(optional)";
                        } else
                        {
                            methodString += " = " + param.DefaultValue.ToString();
                        }
                    }
                }
                methodString += ")";
                if (method.ReturnType != null && method.ReturnType != typeof(void))
                {
                    methodString += ", returns: " + method.ReturnType.ToString();
                }
                api.mod.Monitor.Log(methodString, LogLevel.Info);
            }
        }
    }
}
