using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LuaValley.Helpers
{
    static class ModFinder
    {
        public static string FindModPath(LuaValley lvmod, string modName)
        {
            string lvModPath = lvmod.Helper.DirectoryPath;
            DirectoryInfo? modsFolder = Directory.GetParent(lvModPath);
            if (modsFolder == null) return "";
            string[] subfolders = Directory.GetDirectories(modsFolder.FullName);
            foreach (string subfolder in subfolders)
            {

                string text = File.ReadAllText(subfolder + "/manifest.json");
                if (text == null || text.Length == 0) continue;
                JsonNode? manifest = JsonNode.Parse(text);
                if (manifest == null) continue;
                var nameNode = manifest["UniqueID"];
                if (nameNode == null) continue;
                string name = nameNode.GetValue<string>();
                if (name.ToLower() == modName.ToLower())
                {
                    return subfolder;
                }
            }
            return "";
        }
    }
}
