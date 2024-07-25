using LuaValley.API.Game;
using LuaValley.API.Interface;
using LuaValley.API;
using Microsoft.Xna.Framework;
using NLua;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Monsters;

namespace LuaValley.API.Entities
{
    internal class CharacterAPI : LuaAPI
    {
        public CharacterAPI(APIManager api) : base(api, "CharacterAPI") { }

        public NPC GetNPC(string name)
        {
            return Game1.getCharacterFromName(name);
        }
        public Farmer GetPlayer()
        {
            return Game1.player;
        }

        public Farmer Get(string name)
        {
            var farmers = Game1.getAllFarmers();
            foreach (var farmer in farmers)
            {
                if (farmer.Name == name)
                {
                    return farmer;
                }
            }
            return null;
        }

        public Farmer GetHost()
        {
            return Game1.MasterPlayer;
        }

        public LuaTable GetInventory()
        {
            return GetInventory(Game1.player);
        }

        public LuaTable GetInventory(Farmer farmer)
        {
            return api.lua.ToTable(farmer.Items);
        }

        public Character OnTile(int x, int y, string locationName = null)
        {
            GameLocation loc = Game1.player.currentLocation;
            if (locationName != null)
            {
                loc = Game1.getLocationFromName(locationName);
            }
            return loc.isCharacterAtTile(new Vector2(x, y));
        }

        public Vector2 GetTilePos(Character character)
        {
            var tilePos = character.TilePoint;
            return new Vector2(tilePos.X, tilePos.Y);
        }

        public Vector2 GetPlayerTilePos()
        {
            var tilePos = Game1.player.TilePoint;
            return new Vector2(tilePos.X, tilePos.Y);
        }

        public string GetLocationName(Character character)
        {
            return character.currentLocation.name;
        }

        public void Say(NPC character, string text, string emotion = "neutral")
        {
            if (character != null)
            {
                api.GetAPI<DialogueAPI>()?.Create(text, character, emotion);
            }
        }

        public void Say(NPC character,LuaTable lines, string emotion = "Neutral")
        {
            List<string> linesList = new List<string>();
            foreach (string line in lines.Values)
            {
                if (line is string)
                {
                    linesList.Add(line);
                }
            }
            if (character != null)
            {
                string delim = Dialogue.dialogueBreakDelimited;
                string joinedText = String.Join(delim, linesList.ToArray());
                api.GetAPI<DialogueAPI>()?.Create(joinedText, character, emotion);
            }
        }

        public void Warp(Vector2 tile, Character character, string locationName = "")
        {
            var loc = Game1.getLocationFromName(locationName);
            if (loc == null)
            {
                api.GetAPI<LogAPI>()?.Error("Attempted to warp to invalid location " + locationName);
                return;
            }
            if (character is Farmer)
            {
                Game1.warpFarmer(locationName, (int)tile.X, (int)tile.Y, false);
            } else
            {
                Game1.warpCharacter((NPC)character, locationName, tile);
                ((NPC)character).setTilePosition((int)tile.X, (int)tile.Y);
            }
        }

        public void ForEachCharacter(LuaFunction function, bool includeEventActors = true)
        {
            Utility.ForEachCharacter((NPC character) =>
            {
                api.lua.CallSafe(function, character);
                return true;
            }, includeEventActors);
        }

        public void ForEachVillager(LuaFunction function, bool includeEventActors = true)
        {
            Utility.ForEachVillager((NPC character) =>
            {
                api.lua.CallSafe(function, character);
                return true;
            }, includeEventActors);
        }

        public NPC CreateNPC(string spritePath, Vector2 tile)
        {
            //string spriteName = "Characters\\TrashBear";
            AnimatedSprite sprite = new AnimatedSprite(spritePath, 0, 16, 32);
            NPC character = new NPC(sprite, tile * 64f, 0, spritePath);
            character.AllowDynamicAppearance = false;
            //character.Breather = false;
            character.HideShadow = character.Sprite.SpriteWidth >= 32;
            Game1.player.currentLocation.addCharacter(character);
            character.currentLocation = Game1.player.currentLocation;
            return character;
        }

        public void ResizeSprite(Character character, Vector2 size)
        {
            character.Sprite.SpriteWidth = (int)size.X;
            character.Sprite.SpriteHeight = (int)size.Y;
        }

        public void PlayAnimation(Character character, LuaTable framesTable)
        {
            character.Sprite.ClearAnimation();
            List<FarmerSprite.AnimationFrame> frames = new List<FarmerSprite.AnimationFrame>();
            foreach (LuaTable set in framesTable.Values)
            {
                var list = api.lua.ToList<Int64>(set);
                var frame = new FarmerSprite.AnimationFrame(Convert.ToInt32(list[0]), Convert.ToInt32(list[1]));
                character.Sprite.AddFrame(frame);
            }
        }

        public void ClearAnimation(Character character)
        {
            character.Sprite.ClearAnimation();
        }

        public void EndAnimation(Character character)
        {
            character.Sprite.StopAnimation();
        }

        public void FaceDirection(Character character, int direction)
        {
            character.faceDirection(direction);
        }

        public Monster CreateMonster(string name, Vector2 tile)
        {
            Monster monster = new Monster(name, tile * 64f, 0);
            Game1.player.currentLocation.addCharacter(monster);
            return monster;
        }

        public void MoveTo(NPC character, Vector2 tile, LuaFunction callback = null)
        {
            Point end = new Point((int)tile.X, (int)tile.Y);
            var schedulePath = StardewValley.Pathfinding.PathFindController.findPathForNPCSchedules(character.TilePoint, end, character.currentLocation, 30000);
            character.controller = new StardewValley.Pathfinding.PathFindController(schedulePath, character, Utility.getGameLocationOfCharacter(character));
            if (callback != null)
            {
                character.controller.endBehaviorFunction = (Character c, GameLocation l) =>
                {
                    api.lua.CallSafe(callback, c, l);
                };
            }
        }

        public bool IsMonster(Character character) { return character is Monster; }
    }
}
