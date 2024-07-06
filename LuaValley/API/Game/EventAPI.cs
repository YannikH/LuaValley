using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Game.Game
{
    internal class EventAPI : LuaAPI
    {
        public EventAPI(APIManager api) : base(api, "EventAPI") { }

        public bool HasActiveEvent()
        {
            return Game1.CurrentEvent != null;
        }

        public Event PlayScript(string script, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            var newEvent = new Event(script);

            loc.currentEvent = newEvent;
            Game1.eventUp = true;
            return newEvent;
        }

        public Event CreateEvent()
        {
            return new Event("");
        }

        public void EndEvent()
        {
            try
            {
                Game1.currentLocation.currentEvent = null;
            }
            catch { }
        }

        public Event CurrentEvent()
        {
            return Game1.CurrentEvent;
        }

        public void AddScript(Event evt, string script)
        {
            var addCommands = Event.ParseCommands(script);
            var curCommands = evt.eventCommands;
            var newCommands = new string[addCommands.Length + curCommands.Length];
            curCommands.CopyTo(newCommands, 0);
            addCommands.CopyTo(newCommands, curCommands.Length);
            evt.eventCommands = newCommands;
        }

        public void AllowControl(bool canRun = false)
        {
            Game1.player.CanMove = true;
            Game1.viewportFreeze = false;
            Game1.forceSnapOnNextViewportUpdate = true;
            Game1.globalFade = false;
            Game1.player.canOnlyWalk = !canRun;
        }

        public void EndControl(Event evt)
        {
            evt.EndPlayerControlSequence();
            evt.currentCommand++;
        }

        public void Play(Event evt, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            loc.currentEvent = evt;
            Game1.eventUp = true;
        }
    }
}
