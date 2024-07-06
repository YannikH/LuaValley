using StardewValley.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.Helpers
{
    internal class DebugCatcher: IGameLogger
    {
        string content = "";
        public void Verbose(string msg) { content += msg; }
        public void Debug(string msg) { content += msg; }
        public void Info(string msg) { content += msg; }
        public void Warn(string msg) { content += msg; }
        public void Error(string msg, Exception e) { content += msg; }

        public string GetContent() { return content; }
    }
}
