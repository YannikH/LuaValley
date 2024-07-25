using Microsoft.Xna.Framework;
using NLua;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Weapons;
using StardewValley.Tools;
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
        LuaFunction? overrideFn;
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
        private LuaFunction? GetToolFn(Tool t, string eventName)
        {
            Dictionary<string, string> customFields;
            if (t is MeleeWeapon weapon)
            {
                WeaponData data = weapon.GetData(); ;
                if (data == null) return null;
                customFields = data.CustomFields;
            } else
            {
                ToolData data = t.GetToolData();
                if (data == null) return null;
                customFields = data.CustomFields;
            }
            if (customFields == null) return null;
            customFields.TryGetValue(api.lua.GetModId() + ".Lua." + eventName, out string? useFuncName);
            if (useFuncName != null)
            {
                return api.lua.GetFunction(useFuncName);
            }
            return null;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            
            if (e.Button != SButton.MouseLeft && e.Button != SButton.C && e.Button != SButton.ControllerA) return;
            if (Game1.activeClickableMenu != null) return;
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
                    handleBeforeFn(toolBeforeUses[activeTool.ItemId], e.Button);
                }
                LuaFunction? configBeforeFn = GetToolFn(activeTool, "BeforeUse");
                if (configBeforeFn != null) {
                    handleBeforeFn(configBeforeFn, e.Button);
                }
                LuaFunction? configOverrideFn = GetToolFn(activeTool, "OnUse");
                if (configOverrideFn != null)
                {
                    shouldResetTool = false;
                    overrideFn = configOverrideFn;
                    Game1.player.toolOverrideFunction = OverrideTool;
                }
                if (toolOverrides.ContainsKey(id))
                {
                    shouldResetTool = false;
                    overrideFn = toolOverrides[id];
                    Game1.player.toolOverrideFunction = OverrideTool;
                }
            }
        }

        private void handleBeforeFn(LuaFunction function, SButton button)
        {
            var result = api.lua.CallSafe(function);
            if (result != null && result.GetType().IsArray && ((object[])result).Length > 0)
            {
                object[] resArr = (object[])result;
                if (resArr[0] is bool resBool && !resBool)
                {
                    api.mod.Helper.Input.Suppress(button);
                    return;
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
            if (activeTool != null && overrideFn != null)
            {
                api.lua.CallSafe(overrideFn);
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
