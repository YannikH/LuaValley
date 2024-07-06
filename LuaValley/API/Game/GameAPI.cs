using LuaValley.API.Interface;
using LuaValley.Helpers;
using Microsoft.Xna.Framework;
using NLua;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Game
{
    public enum SDVEvent
    {
        MenuChanged,
        Tick,
        TickSecond,
        BeforeSave,
        AfterSave,
        AfterLoad,
        DayStarted,
        DayEnding,
        TimeChanged,
        ButtonPressed,
        ButtonReleased,
        CursorMoved,
        MousewheelScrolled,
        PlayerConnected,
        PlayerDisconnected,
        InventoryChanged,
        LevelChanged,
        LocationChanged,
        ChestInventoryChanged,
        DebrisListChanged,
        FurnitureListChanged,
        NPCListChanged,
        ObjectListChanged,
        TerrainFeatureListChanged,
        MPMessageReceived
    }
    internal class GameAPI : LuaAPI
    {
        private int eventIndex = 0;
        private List<string> toRemoveEvents = new List<string>();
        private List<LuaEvent> events = new List<LuaEvent>();
        private List<QueuedFunction> queuedFunctions = new List<QueuedFunction>();
        public struct LuaEvent
        {
            public SDVEvent type;
            public string id;
            public LuaFunction callback;
            public bool triggerOnce;

            public LuaEvent(SDVEvent type, string id, LuaFunction callback, bool triggerOnce = false)
            {
                this.type = type;
                this.id = id;
                this.callback = callback;
                this.triggerOnce = triggerOnce;
            }
        }
        public struct QueuedFunction
        {
            public LuaFunction callback;
            public int endTime;
            public QueuedFunction(LuaFunction callback, int endTime)
            {
                this.callback = callback;
                this.endTime= endTime;
            }
        }
        public GameAPI(APIManager api) : base(api, "GameAPI")
        {
            var events = api.mod.Helper.Events;
            events.Display.MenuChanged += onEvent<MenuChangedEventArgs>(SDVEvent.MenuChanged);
            events.GameLoop.UpdateTicked += RunFunctionQueue;

            events.GameLoop.UpdateTicking += CleanupEvents;
            events.GameLoop.UpdateTicked += onEvent<UpdateTickedEventArgs>(SDVEvent.Tick);
            events.GameLoop.OneSecondUpdateTicked += onEvent<OneSecondUpdateTickedEventArgs>(SDVEvent.TickSecond);
            events.GameLoop.Saving += onEvent<SavingEventArgs>(SDVEvent.BeforeSave);
            events.GameLoop.Saved += onEvent<SavedEventArgs>(SDVEvent.AfterSave);
            events.GameLoop.DayStarted += onEvent<DayStartedEventArgs>(SDVEvent.DayStarted);
            events.GameLoop.DayEnding += onEvent<DayEndingEventArgs>(SDVEvent.DayEnding);
            events.GameLoop.TimeChanged += onEvent<TimeChangedEventArgs>(SDVEvent.TimeChanged);

            events.Input.ButtonPressed += onEvent<ButtonPressedEventArgs>(SDVEvent.ButtonPressed);
            events.Input.ButtonReleased += onEvent<ButtonReleasedEventArgs>(SDVEvent.ButtonReleased);
            events.Input.CursorMoved += onEvent<CursorMovedEventArgs>(SDVEvent.CursorMoved);
            events.Input.MouseWheelScrolled += onEvent<MouseWheelScrolledEventArgs>(SDVEvent.MousewheelScrolled);

            events.Multiplayer.PeerConnected += onEvent<PeerConnectedEventArgs>(SDVEvent.PlayerConnected);
            events.Multiplayer.PeerDisconnected += onEvent<PeerDisconnectedEventArgs>(SDVEvent.PlayerDisconnected);

            events.Player.InventoryChanged += onEvent<InventoryChangedEventArgs>(SDVEvent.InventoryChanged);
            events.Player.LevelChanged += onEvent<LevelChangedEventArgs>(SDVEvent.LevelChanged);
            events.Player.Warped += onEvent<WarpedEventArgs>(SDVEvent.LocationChanged);

            events.World.ChestInventoryChanged += onEvent<ChestInventoryChangedEventArgs>(SDVEvent.ChestInventoryChanged);
            events.World.DebrisListChanged += onEvent<DebrisListChangedEventArgs>(SDVEvent.DebrisListChanged);
            events.World.FurnitureListChanged += onEvent<FurnitureListChangedEventArgs>(SDVEvent.FurnitureListChanged);
            events.World.NpcListChanged += onEvent<NpcListChangedEventArgs>(SDVEvent.NPCListChanged);
            events.World.ObjectListChanged += onEvent<ObjectListChangedEventArgs>(SDVEvent.ObjectListChanged);
            events.World.TerrainFeatureListChanged += onEvent<TerrainFeatureListChangedEventArgs>(SDVEvent.TerrainFeatureListChanged);
        }

        public string AddEventHandler(string type, LuaFunction callback, bool triggerOnce = false)
        {
            try
            {
                SDVEvent typeEnum = (SDVEvent)Enum.Parse(typeof(SDVEvent), type);
                string id = type + "_" + eventIndex++;
                events.Add(new LuaEvent(typeEnum, id, callback, triggerOnce));
                return id;
            }
            catch
            {
                api.GetAPI<LogAPI>()?.Error("Unable to add eventhandler of type " + type);
            }
            return "ERROR";
        }

        public void CleanupEvents(object? sender, UpdateTickingEventArgs e)
        {
            events.RemoveAll((e) => toRemoveEvents.Contains(e.id));
        }

        public void RemoveEvent(string name)
        {
            toRemoveEvents.Add(name);
        }

        public override void Reset()
        {
            events = new List<LuaEvent>();
        }

        private EventHandler<T> onEvent<T>(SDVEvent type)
        {
            return (sender, e) =>
            {
                //mod.Monitor.Log(Convert.ToString(type), LogLevel.Info);
                foreach (var eventHandler in events)
                {
                    if (eventHandler.type == type)
                    {
                        api.lua.CallSafe(eventHandler.callback, e);
                        if (eventHandler.triggerOnce)
                        {
                            toRemoveEvents.Add(eventHandler.id);
                        }
                    }
                }
            };
        }

        public void SetCursor(string cursor = "default")
        {
            int cursorType = Game1.cursor_default;
            switch (cursor)
            {
                case "none": cursorType = Game1.cursor_none; break;
                case "wait": cursorType = Game1.cursor_wait; break;
                case "grab": cursorType = Game1.cursor_grab; break;
                case "gift": cursorType = Game1.cursor_gift; break;
                case "talk": cursorType = Game1.cursor_talk; break;
                case "look": cursorType = Game1.cursor_look; break;
                case "harvest": cursorType = Game1.cursor_harvest; break;
            }
            Game1.mouseCursor = cursorType;
        }

        public string DebugCommand(string command)
        {
            string[] commandArray = ArgUtility.SplitBySpaceQuoteAware("" + command);
            var catcher = new DebugCatcher();
            DebugCommands.TryHandle(commandArray, catcher);
            return catcher.GetContent();
        }

        public void ExecuteWhenFree(LuaFunction function)
        {
            Game1.PerformActionWhenPlayerFree(() =>
            {
                api.lua.CallSafe(function);
            });
        }

        public void RegisterTrigger(string name)
        {
            TriggerActionManager.RegisterTrigger(name);
        }

        public void RaiseTrigger(string name, params object[] args)
        {
            TriggerActionManager.Raise(name, args);
        }

        public void RegisterAction(string name, LuaFunction function)
        {
            TriggerActionManager.RegisterAction(name, createActionDelegate(function));
        }

        private TriggerActionDelegate createActionDelegate(LuaFunction function)
        {
            return (string[] args, TriggerActionContext context, out string error) =>
            {
                error = "";
                api.lua.CallSafe(function);
                return true;
            };
        }

        public void RunFunctionQueue(object? sender, UpdateTickedEventArgs e)
        {
            var time = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
            for (int i = queuedFunctions.Count- 1; i >= 0; i--)
            {
                var func = queuedFunctions[i];
                if (time > func.endTime)
                {
                    api.lua.CallSafe(func.callback);
                    queuedFunctions.RemoveAt(i);
                }
            }
        }

        public void ExecuteAfter(int milliseconds, LuaFunction function) {
            
            int endTime = (int)Math.Round(Game1.currentGameTime.TotalGameTime.TotalMilliseconds, 0) + milliseconds;
            queuedFunctions.Add(new QueuedFunction(function, endTime));
        }
    }
}
