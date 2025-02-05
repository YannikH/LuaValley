﻿using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using NLua;
using LuaValley.API;

namespace LuaValley
{
    internal sealed class LuaValley: Mod
    {
        List<LuaProvider> providers = new List<LuaProvider>();
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.ConsoleCommands.Add("continue", "Used to continue lua execution when debugging", StopDebugging);
            helper.ConsoleCommands.Add("Lua:Debug", "", StartDebugging);
        }

        private void GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            List<IContentPack> contentpacks = new List<IContentPack>(this.Helper.ContentPacks.GetOwned());
            foreach (IContentPack luaMod in contentpacks)
            {
                var modLua = new LuaProvider(this, luaMod);
                providers.Add(modLua);
                modLua.Load();
                modLua.GameStart();
            }
            var dependentMods = GetDependentMods();
            foreach (IManifest luaMod in dependentMods)
            {
                var modLua = new LuaProvider(this, luaMod);
                providers.Add(modLua);
                modLua.Load();
                modLua.GameStart();
            }
        }

        private void StartDebugging(string command, string[] arguments)
        {
            foreach (var provider in providers)
            {
                provider.Debug();
            }
        }

        private void StopDebugging(string command, string[] arguments)
        {
            Monitor.Log("Stop debugging command received", LogLevel.Error);
            foreach(var provider in providers)
            {
                provider.StopDebug();
            }
        }

        private List<IManifest> GetDependentMods()
        {
            List<IManifest> mods = new List<IManifest>();
            var allMods = Helper.ModRegistry.GetAll();
            foreach (IModInfo mod in allMods)
            {
                foreach (IManifestDependency dependency in mod.Manifest.Dependencies)
                {
                    if (dependency.UniqueID == ModManifest.UniqueID)
                    {
                        mods.Add(mod.Manifest);
                    }
                }
            }

            return mods;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs args)
        {
            foreach (var lua in providers)
            {
                lua.SaveLoaded();
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            if (e.Button == SButton.X)
            {
                foreach (var lua in providers)
                {
                    lua.Load();
                    lua.GameStart();
                    if (Context.IsWorldReady)
                    {
                        lua.SaveLoaded();
                    }
                    this.Monitor.Log("Mod scripts reloaded", LogLevel.Info);
                }
            }
        }
    }
}