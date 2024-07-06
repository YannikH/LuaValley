using LuaValley.API.Game;
using LuaValley.API.Interface;
using NLua;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuaValley.API.Game.GameAPI;

namespace LuaValley.Helpers
{
    internal class MPMessage
    {
        object content;
        public MPMessage(object content)
        {
            this.content = content;
        }

        public object GetContent()
        {
            return content;
        }
    }
}
