using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaValley.API;
using StardewModdingAPI;
using NLua;
using NLua.Exceptions;
using StardewValley;
using LuaValley.API.Game;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using LuaValley.API.Util;
using LuaValley.Helpers;

namespace LuaValley
{
    internal class LuaProvider
    {
        private static LuaProvider instance;
        Lua state;
        LuaValley mod;
        List<string> requiredFiles = new List<string>();
        APIManager apiManager;
        IContentPack pack;
        HashSet<string> defaultStateKeys = new HashSet<string>();
        bool isDebugging = false;
        public LuaProvider(LuaValley mod, IContentPack pack)
        {
            this.mod = mod;
            this.pack = pack;
            this.apiManager = new APIManager(mod, this);
            CreateState();
            apiManager.inject(state);
            var logger = new FileLogger(mod.Helper.DirectoryPath + "/functions.md");
            apiManager.LogFunctions(logger);
        }

        public IContentPack GetPack() { return pack; }

        private void CreateState()
        {
            state = new Lua();
            state["require"] = require;
            state["Debug"] = Debug;
            state["Debug"] = Debug;
        }
        private void DebugFreeze()
        {
            Console.WriteLine("Pausing Lua execution, type continue in this console to stop debugging");
            Console.WriteLine("Press the enter key to execute the next line of lua");
            ////Console.WriteLine("Waiting");
            //ConsoleKey key = ConsoleKey.Clear;
            //while (key != ConsoleKey.Enter && key != ConsoleKey.Spacebar)
            //{
            //    key = Console.ReadKey().Key;
            //}
            //if (key == ConsoleKey.Enter)
            //{
            //    StopDebug();
            //}
            string result = Console.ReadLine() ?? "";
            Console.WriteLine("Received: " + result);
            while (result == null && isDebugging)
            {
                result = Console.ReadLine() ?? "";
                Console.WriteLine("Received" + result);
            }
            if (result == "continue")
            {
                StopDebug();
            }
        }

        public void StopDebug()
        {
            state.SetDebugHook(KeraLua.LuaHookMask.Disabled, 0);
            isDebugging = false;
        }

        public void Debug()
        {
            mod.Monitor.Log("Pausing Lua execution, press enter in this console to continue lua execution (this might take some seconds to take effect)", LogLevel.Error);
            isDebugging = true;
            state.SetDebugHook(KeraLua.LuaHookMask.Line, 1);
            state.DebugHook += OnDebug;
        }

        private void printLocals(KeraLua.LuaDebug debug)
        {
            mod.Monitor.Log("Locals: ", LogLevel.Warn);
            //for(int i = 0; i < 10; i++)
            //{
            //    string local = state.GetLocal(debug, i);
            //    object res = state.Pop();
            //    mod.Monitor.Log("   : " + local, LogLevel.Warn);
            //}
            var globs = state.GetTable("_G");
            foreach(var key in globs.Keys)
            {
                
                if (!defaultStateKeys.Contains(key))
                {
                    var value = state[(string)key];
                    mod.Monitor.Log("   " + key + " = " + value.ToString(), LogLevel.Warn);
                }
            }
        }
        
        private void printPos(KeraLua.LuaDebug debug)
        {
            //mod.Monitor.Log("Function " + debug.Name + ", line:" + debug.CurrentLine, LogLevel.Warn);
            var globs = state.GetTable("_G");
            var str = state.Globals;
            var trace = state.GetDebugTraceback();
            var info = state.GetInfo("source ", ref debug);
            mod.Monitor.Log(trace, LogLevel.Debug);
            mod.Monitor.Log(trace, LogLevel.Debug);
        }

        private void OnDebug(object? sender, NLua.Event.DebugHookEventArgs e)
        {
            if (!isDebugging) return;
            Console.Clear();
            printPos(e.LuaDebug);
            printLocals(e.LuaDebug);
            Thread breaker = new Thread(DebugFreeze);
            breaker.Start();
            breaker.Join();
        }

        private void Sleep()
        {

        }

        public void PushState(string key, object content)
        {
            state[key] = content;
        }

        public APIManager getAPIManager()
        {
            return apiManager;
        }

        private void Execute(string code)
        {
            try
            {
                this.state.DoString(code);
            }
            catch (Exception e)
            {
                mod.Monitor.Log("An error happened while executing Lua: " + e.Message, LogLevel.Error);
            }
        }

        public void Reset()
        {
            requiredFiles.Clear();
            apiManager.reset();
            CreateState();
            apiManager.inject(state);
        }

        public void require(string filePath)
        {
            if (mod == null) return;
            var slashedPath = filePath.Replace(".", "/");
            slashedPath += ".lua";
            string[] loadPath = { pack.DirectoryPath,  slashedPath};
            var finalPath = Path.Combine(loadPath);
            var code = File.ReadAllText(finalPath);
            Execute(code);
        }

        public LuaTable CreateTable()
        {
            return (LuaTable)state.DoString("return {}")[0];
        }
        public List<T> ToList<T>(LuaTable t, bool fromKeys = false)
        {
            List<T> responseObjects = new List<T>();
            int i = 0;
            var baseList = fromKeys ? t.Keys : t.Values;
            foreach (var key in baseList)
            {
                responseObjects.Add((T)(object)key);
                i++;
            }
            return responseObjects;
        }

        public LuaTable ToTable(IEnumerable<object> list)
        {
            var table = CreateTable();
            int key = 0;
            foreach (var item in list)
            {
                table[key++] = item;
            }
            return table;
        }

        public bool Load()
        {
            mod.Monitor.Log("Loading lua mod: " + pack.Manifest.Name, LogLevel.Trace);
            Reset();
            var loadPath = Path.Combine(pack.DirectoryPath, "mod.lua");
            var code = File.ReadAllText(loadPath);
            try
            {
                Execute(code);
                var globs = state.GetTable("_G");
                defaultStateKeys = ToList<string>(globs, true).ToHashSet<string>();
                mod.Monitor.Log("Loaded Lua mod: " + pack.Manifest.Name, LogLevel.Trace);
                return true;
            } catch (Exception e)
            {
                mod.Monitor.Log("Failed to load Lua mod: " + pack.Manifest.Name, LogLevel.Error);
                return false;
            }
        }

        public void GameStart()
        {
            LuaFunction start = state["gamestart"] as LuaFunction;
            if (start != null)
            {
                CallSafe(start, pack.Manifest.Name);
            }
        }

        public void SaveLoaded()
        {
            LuaFunction start = state["saveloaded"] as LuaFunction;
            if (start != null)
            {
                CallSafe(start, pack.Manifest.Name);
            }
        }

        public object? CallSafe(LuaFunction func, params object[] arguments)
        {
            if (func == null) return null;
            try
            {
                //new Thread(() =>
                //{
                    return func.Call(arguments);
                //}).Start();
            } catch (Exception e)
            {
                mod.Monitor.Log("An error happened while calling Lua: " + e.Message, LogLevel.Error);
                return null;
            }
        }
    }
}
