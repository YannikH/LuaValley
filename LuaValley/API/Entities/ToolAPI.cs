using Microsoft.Xna.Framework;
using NLua;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuaValley.API.Game.GameAPI;

namespace LuaValley.API.Entities
{
    internal class ToolAPI: LuaAPI
    {
        private HashSet<string> disabledTools = new HashSet<string>();
        private Dictionary<string, LuaFunction> toolBeforeUses = new Dictionary<string, LuaFunction>();
        private Dictionary<string, LuaFunction> toolOverrides = new Dictionary<string, LuaFunction>();
        private bool shouldResetTool = true;
        public ToolAPI(APIManager manager): base(manager, "ToolAPI")
        {
            manager.mod.Helper.Events.Input.ButtonPressed += OnButtonPressed;
            manager.mod.Helper.Events.Input.ButtonReleased += OnButtonReleased;
        }

        public Tool ActiveTool()
        {
            return (Game1.player != null) ? Game1.player.CurrentTool : null;
        }

        public void OverrideToolFunction(string itemId, LuaFunction useFunction)
        {
            toolOverrides[itemId] = useFunction;
        }

        public void AddBeforeUseFunction(string itemId, LuaFunction beforeFunction)
        {
            toolBeforeUses.Add(itemId, beforeFunction);
        }

        public void DisableTool(string itemId)
        {
            disabledTools.Add(itemId);
        }

        public void RestoreTool(string itemId)
        {
            toolOverrides.Remove(itemId);
            toolBeforeUses.Remove(itemId);
            disabledTools.Remove(itemId);
        }

        public void UseTool()
        {
            if (Game1.player.CurrentTool == null) return;
            Farmer who = Game1.player;
            float oldStamina = who.stamina;
            if (who.IsLocalPlayer)
            {
                who.CurrentTool.DoFunction(who.currentLocation, (int)who.GetToolLocation().X, (int)who.GetToolLocation().Y, 1, who);
            }
            who.lastClick = Vector2.Zero;
            who.checkForExhaustion(oldStamina);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            
            if (e.Button != SButton.MouseLeft && e.Button != SButton.C && e.Button != SButton.ControllerA) return;
            var activeTool = Game1.player.CurrentTool;
            if (activeTool != null)
            {
                var id = activeTool.ItemId;
                if (disabledTools.Contains(id) || disabledTools.Contains("all"))
                {
                    api.mod.Helper.Input.Suppress(e.Button);
                    return;
                }
                if (toolBeforeUses.ContainsKey(id))
                {
                    var result = api.lua.CallSafe(toolBeforeUses[activeTool.ItemId]);
                    if (result != null && result.GetType().IsArray && ((object[])result).Length > 0)
                    {
                        object[] resArr = (object[])result;
                        if (resArr[0] is bool resBool && !resBool) {
                            api.mod.Helper.Input.Suppress(e.Button);
                            return;
                        }
                    }
                }
                if (toolOverrides.ContainsKey(id))
                {
                    shouldResetTool = false;
                    Game1.player.toolOverrideFunction = OverrideTool;
                }
            }
        }

        private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
        {
            shouldResetTool = true;
        }

        private void OverrideTool(Farmer who)
        {
            var activeTool = Game1.player.CurrentTool;
            if (activeTool != null && toolOverrides.ContainsKey(activeTool.ItemId))
            {
                api.lua.CallSafe(toolOverrides[activeTool.ItemId]);
            }
            if (shouldResetTool)
            {
                Game1.player.toolOverrideFunction = null;
            }
        }

        public override void Reset()
        {
            if (Game1.player != null)
            {
                Game1.player.toolOverrideFunction = null;
            }
            toolOverrides = new Dictionary<string, LuaFunction>();
            toolBeforeUses = new Dictionary<string, LuaFunction>();
        }
    }
}
