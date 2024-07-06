using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Interface
{
    internal class TextAPI: LuaAPI
    {
        public delegate void drawCommand(SpriteBatch b);
        Dictionary<string, drawCommand> drawList = new Dictionary<string, drawCommand>();
        private int drawIndex = 0;
        public TextAPI(APIManager api) : base(api, "TextAPI")
        {
            api.mod.Helper.Events.Display.Rendered += Draw;
        }

        public string Localize(string key, object? args)
        {
            return api.mod.Helper.Translation.Get(key, args);
        }

        private string getDrawKey()
        {
            return "TXT_" + drawIndex++;
        }

        public string DrawTextBubble(string text, Vector2 tilePos, bool requireHover = false, string key = "")
        {
            if (key == "") { key = getDrawKey(); }
            drawList[key] = ((SpriteBatch b) =>
            {
                if (requireHover)
                {
                    Vector2 mousePos = !Game1.wasMouseVisibleThisFrame ? Game1.player.GetToolLocation() : new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y);
                    Vector2 tile = new Vector2(mousePos.X / 64f, mousePos.Y / 64f);
                    if (Math.Round(tile.X) != Math.Round(tilePos.X) || Math.Round(tile.Y) != Math.Round(tilePos.Y)) return;
                }
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(tilePos.X * 64 + 32, tilePos.Y * 64 - 32));
                SpriteText.drawSmallTextBubble(b, text, position, -1, -1, false);
            });
            return key;
        }

        public string DrawText(string text, Vector2 tilePos, string key = "")
        {
            if (key == "") { key = getDrawKey(); }
            drawList[key] = ((SpriteBatch b) =>
            {
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(tilePos.X * 64, tilePos.Y * 64 - 32));
                SpriteText.drawStringWithScrollCenteredAt(b, text, (int)position.X, (int)position.Y);
            });
            return key;
        }

        private void Draw(object? sender, RenderedEventArgs e)
        {
            foreach (var drawCommand in drawList.Values)
            {
                drawCommand(e.SpriteBatch);
            }
        }

        public void removeDrawing(string key)
        {
            drawList.Remove(key);
        }

        public void ClearText()
        {
            drawList.Clear();
        }
    }
}
