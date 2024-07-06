using Microsoft.Xna.Framework;
using NLua;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace LuaValley.API.Entities
{
    internal class ObjectAPI : LuaAPI
    {
        public ObjectAPI(APIManager api) : base(api, "ObjectAPI") { }

        public Object Create(string id)
        {
            return ItemRegistry.Create<Object>(id);
        }

        public void AddToInventory(Object obj)
        {
            Game1.player.addItemByMenuIfNecessary(obj);
        }

        public void AddToInventory(Farmer f, Object obj)
        {
            f.addItemByMenuIfNecessary(obj);
        }

        public void Drop(Object obj, Vector2 pos, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            loc.debris.Add(new Debris(obj, pos));

        }

        public void Remove(Object obj, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            if (obj != null)
            {
                loc.removeObject(obj.TileLocation, false);
                obj.performRemoveAction();
            }
        }

        public void SetSprite(Object obj, int id)
        {
            obj.ParentSheetIndex = id;
        }

        public void Place(Object obj, Vector2 tilePos, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            Utility.tryToPlaceItem(loc, obj, (int)tilePos.X * 64, (int)tilePos.Y * 64);
        }

        public void SetPos(Object obj, Vector2 tilePos)
        {
            obj.TileLocation = tilePos;
        }

        public void SetInvisible(Object obj, bool invisible)
        {
            obj.isTemporarilyInvisible = invisible;
        }

        public Object FromTile(int x, int y, string locationName = null)
        {

            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            return loc.getObjectAtTile(x, y);
        }

        public Object ActiveObject()
        {
            return Game1.player.ActiveObject;
        }

        public LuaTable GetItems(Object obj)
        {
            var table = api.lua.CreateTable();
            if (obj is Chest)
            {
                var items = (obj as Chest).Items;
                int key = 0;
                foreach (var item in items)
                {
                    table[key++] = item;
                }
            }
            return table;
        }

        public void ForEachCrop(LuaFunction function)
        {
            Utility.ForEachCrop((Crop crop) =>
            {
                api.lua.CallSafe(function, crop);
                return true;
            });
        }

        public void ForEachItem(LuaFunction function)
        {
            Utility.ForEachItem((Item item) =>
            {
                api.lua.CallSafe(function, item);
                return true;
            });
        }

        public void ForEachItemIn(string locationName, LuaFunction function)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null && locationName != "")
            {
                loc = Game1.getLocationFromName(locationName);
            }

            Utility.ForEachItemIn(loc, (Item item) =>
            {
                api.lua.CallSafe(function, item);
                return true;
            });
        }
    }
}
