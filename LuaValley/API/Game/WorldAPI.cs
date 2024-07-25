using NLua;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework.Graphics;
using xTile.Dimensions;
using xTile.Tiles;
using xTile.Layers;
using System.Reflection.Metadata.Ecma335;
using StardewValley.Extensions;
using StardewValley.Monsters;

namespace LuaValley.API.Game
{
    public struct TileInfo
    {
        public int TileIndex = 0;
        public int TileSheet = 0;
        public TileInfo (int TileIndex, int TileSheet)
        {
            this.TileIndex = TileIndex;
            this.TileSheet = TileSheet;
        }
    }
    internal class WorldAPI: LuaAPI
    {
        private static int TileActionCounter = 0;
        public static string CreateTileActionName()
        {
            return "YH_LV_TILEACTION" + TileActionCounter++;
        }
        public WorldAPI(APIManager api): base(api, "WorldAPI") { }

        public Vector2 GetMouseTile()
        {
            Vector2 position = !Game1.wasMouseVisibleThisFrame ? Game1.player.GetToolLocation() : new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y);
            Vector2 tile = new Vector2(position.X / 64f, position.Y / 64f);
            return tile;
        }
        public string LocationName() { return Game1.currentLocation.Name; }
        public string LocationName(GameLocation loc) { return loc == null ? Game1.currentLocation.Name : loc.Name; }

        public GameLocation GetLocation(string locationName = null)
        {
            if (locationName != null)
            {
                return Game1.getLocationFromName(locationName);
            }
            return Game1.player.currentLocation;
        }

        public Vector2 GetEmptyTile(Vector2 tile, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            return Utility.getRandomAdjacentOpenTile(tile, loc);
        }

        public LuaTable GetCharacters(string locationName = null)
        {
            GameLocation loc = GetLocation(locationName);
            return api.lua.ToTable(loc.characters.ToArray<NPC>());
        }

        public LuaTable GetMonsters(string locationName = null)
        {
            GameLocation loc = GetLocation(locationName);
            List<Monster> monsters = new List<Monster>();
            foreach (var character in loc.characters)
            {
                if (character is Monster monster)
                {
                    monsters.Add(monster);
                }
            }
            return api.lua.ToTable(monsters);
        }

        public bool IsOpenTile(Vector2 tile, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            return !loc.IsTileBlockedBy(tile);
        }

        public void ForEachBuilding(LuaFunction function, bool ignoreConstruction = true)
        {
            Utility.ForEachBuilding((StardewValley.Buildings.Building building) =>
            {
                api.lua.CallSafe(function, building);
                return true;
            }, ignoreConstruction);
        }

        public void ForEachLocation(LuaFunction function)
        {
            Utility.ForEachLocation((GameLocation location) =>
            {
                api.lua.CallSafe(function, location);
                return true;
            });
        }

        public void SetTileClickAction(Vector2 tile, string action, string locationName = null, bool passable = true, int tileIndex = 48)
        {
            var loc = GetLocation(locationName);
            loc.setMapTile((int)tile.X, (int)tile.Y, tileIndex, "Buildings", action);
            loc.setTileProperty((int)tile.X, (int)tile.Y, "Buildings", "Passable", "");
        }

        public void SetTileClickAction(Vector2 tile, LuaFunction action, string locationName = null, bool passable = true, int tileIndex = 48)
        {
            var loc = GetLocation(locationName);
            var name = WorldAPI.CreateTileActionName();
            var actionFunc = (GameLocation loc, string[] args, Farmer who, Point tile) =>
            {
                api.lua.CallSafe(action);
                return true;
            };
            GameLocation.RegisterTileAction(name, actionFunc);
            loc.setMapTile((int)tile.X, (int)tile.Y, tileIndex, "Buildings", name);
            loc.setTileProperty((int)tile.X, (int)tile.Y, "Buildings", "Passable", "");
        }

        public void RegisterTileAction(string name, LuaFunction action)
        {
            var actionFunc = (GameLocation loc, string[] args, Farmer who, Point tile) =>
            {
                api.lua.CallSafe(action);
                return true;
            };
            GameLocation.RegisterTileAction(name, actionFunc);
        }

        public void SetTileTouchAction(Vector2 tile, string action, string locationName = null, int tileIndex = 48)
        {
            // layer name is Back
            var loc = GetLocation(locationName);
            loc.setMapTile((int)tile.X, (int)tile.Y, tileIndex, "Back", "");
            loc.setTileProperty((int)tile.X, (int)tile.Y, "Back", "TouchAction", action);
        }

        public void SetMapTileIndex(Vector2 tile, int index, string layer, int tileSheet = 0, string locationName = null)
        {
            var loc = GetLocation(locationName);
            loc.setMapTile((int)tile.X, (int)tile.Y, index, layer, "", tileSheet);
        }
        
        public int GetMapTileIndex(Vector2 tile, string layer, string locationName = null)
        {
            var loc = GetLocation(locationName);
            var tileRef = loc.map.RequireLayer(layer).Tiles[(int)tile.X, (int)tile.Y];
            if (tileRef != null)
            {
                return tileRef.TileIndex;
            }
            return -1;
        }
    }
}
