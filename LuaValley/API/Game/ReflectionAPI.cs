using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Game
{
    internal class ReflectionAPI: LuaAPI
    {
        public ReflectionAPI(APIManager api) : base(api, "ReflectionAPI") { }

        public bool GetBool(object target, string fieldName)
        {
            return GetValue<bool>(target, fieldName);
        }

        public string GetString(object target, string fieldName)
        {
            return GetValue<string>(target, fieldName);
        }

        public int GetInt(object target, string fieldName)
        {
            return GetValue<int>(target, fieldName);
        }

        public float GetFloat(object target, string fieldName)
        {
            return GetValue<float>(target, fieldName);
        }

        public void SetBool(object target, string fieldName, bool value)
        {
            SetValue<bool>(target, fieldName, value);
        }

        public void SetString(object target, string fieldName, string value)
        {
            SetValue<string>(target, fieldName, value);
        }

        public void SetInt(object target, string fieldName, int value)
        {
            SetValue<int>(target, fieldName, value);
        }

        public void SetFloat(object target, string fieldName, float value)
        {
            SetValue<float>(target, fieldName, value);
        }

        public T GetValue<T>(object target, string fieldName)
        {
            return api.mod.Helper.Reflection.GetField<T>(target, fieldName).GetValue();
        }

        public void SetValue<T>(object target, string fieldName, T value)
        {
            api.mod.Helper.Reflection.GetField<T>(target, fieldName).SetValue(value);
        }
    }
}
