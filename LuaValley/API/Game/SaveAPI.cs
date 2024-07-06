using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using StardewModdingAPI.Events;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace LuaValley.API.Game
{
    internal class SaveAPI : LuaAPI
    {
        JsonNode saveData = JsonNode.Parse("{}");
        public SaveAPI(APIManager api) : base(api, "SaveAPI")
        {
            api.mod.Helper.Events.GameLoop.Saved += onSaved;
            api.mod.Helper.Events.GameLoop.SaveLoaded += onLoaded;
        }

        private void onSaved(object? sender, SavedEventArgs e)
        {
            initSaveFile();
            File.WriteAllText(getSavePath(), saveData.ToString());
        }

        private void onLoaded(object? sender, SaveLoadedEventArgs e)
        {
            initSaveFile();
            var text = File.ReadAllText(getSavePath());
            saveData = JsonNode.Parse(text);
        }

        private string getSavePath()
        {
            string filename = SaveGame.FilterFileName(Game1.GetSaveGameName()) + "_" + Game1.uniqueIDForThisGame;
            return Path.Combine(Program.GetSavesFolder(), filename, "luaValley.json");
        }

        private void initSaveFile()
        {
            var path = getSavePath();
            if (!File.Exists(path))
            {
                File.Create(path);
                File.WriteAllText(getSavePath(), "{}");
            }
        }

        public void Set(string key, int value)
        {
            if (saveData != null) saveData[key] = value;
        }

        public void Set(string key, string value)
        {
            if (saveData != null) saveData[key] = value;
        }

        public object Get(string key)
        {
            if (saveData != null)
            {
                return saveData[key].GetValue<object>();
            }
            return null;
        }
    }
}
