using LuaValley.API.Interface;
using LuaValley.Helpers;
using NLua;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuaValley.API.Game.GameAPI;

namespace LuaValley.API.Game
{
    internal class MultiplayerAPI: LuaAPI
    {
        int eventIndex = 0;
        private List<LuaEvent> events = new List<LuaEvent>();
        private List<string> toRemoveEvents = new List<string>();
        public MultiplayerAPI(APIManager api): base(api, "MultiplayerAPI") {
            api.mod.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }
        public void SendMessage(object content)
        {
            api.mod.Helper.Multiplayer.SendMessage(new MPMessage(content), "MyMessageType", modIDs: new[] { api.mod.ModManifest.UniqueID });
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            foreach (LuaEvent evt in events)
            {
                api.lua.CallSafe(evt.callback, e.ReadAs<MPMessage>());
                if (evt.triggerOnce)
                {
                    toRemoveEvents.Add(evt.id);
                }
            }
            CleanupEvents();
        }

        public string AddMessageHandler(LuaFunction callback, bool triggerOnce = false)
        {
            try
            {
                string id = "mp_" + eventIndex++;
                events.Add(new LuaEvent(SDVEvent.MPMessageReceived, id, callback, triggerOnce));
                return id;
            }
            catch
            {
                api.GetAPI<LogAPI>()?.Error("Unable to add eventhandler");
            }
            return "ERROR";
        }

        private void CleanupEvents()
        {
            events.RemoveAll((e) => toRemoveEvents.Contains(e.id));
        }

        public void RemoveMessageHandler(string name)
        {
            toRemoveEvents.Add(name);
        }

        public void ClearHandlers()
        {
            events = new List<LuaEvent>();
        }
    }
}
