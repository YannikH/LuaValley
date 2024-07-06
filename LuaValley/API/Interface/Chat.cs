using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Interface
{
    internal class Chat : LuaAPI
    {
        public Chat(APIManager api) : base(api, "ChatAPI") { }

        public void Error(string text)
        {
            if (Game1.chatBox != null)
            {
                Game1.chatBox.addErrorMessage(text);
            }
        }

        public void Info(string text)
        {
            if (Game1.chatBox != null)
            {
                Game1.chatBox.addInfoMessage(text);
            }
        }

        public void Info(params object[] arguments)
        {

        }
    }
}
