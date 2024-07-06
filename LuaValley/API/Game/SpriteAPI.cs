using Microsoft.Xna.Framework;
using NLua;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Game
{
    public struct SpriteSheet
    {
        public string name;
        public int width;
        public int height;
    }
    internal class SpriteAPI: LuaAPI
    {
        public SpriteAPI(APIManager api) : base(api, "SpriteAPI") { }

        public TemporaryAnimatedSprite Create(string sheet, int x, int y, int w, int h, float animationInterval, int animationLength, int loops, Vector2 pixelPos, bool flipped = false)
        {
            var sourceRect = new Microsoft.Xna.Framework.Rectangle(x, y, w, h);
            TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite(sheet, sourceRect, animationInterval, animationLength, loops, pixelPos, false, flipped);
            GameLocation loc = Game1.player.currentLocation;
            loc.temporarySprites.Add(sprite);
            return sprite;
        }

        public TemporaryAnimatedSprite Create(SpriteSheet sheet, int x, int y, Vector2 tilePos, bool flipped = false)
        {
            var sourceRect = new Microsoft.Xna.Framework.Rectangle(x * sheet.width, y * sheet.height, sheet.width, sheet.height);
            tilePos.Y -= 1;
            TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite(sheet.name, sourceRect, 0, 1, 9999, tilePos * 64f, false, flipped);
            GameLocation loc = Game1.player.currentLocation;
            loc.temporarySprites.Add(sprite);
            ScaleTile(sprite, 1);
            return sprite;
        }

        public SpriteSheet CreateSheet(string sheet, int w, int h)
        {
            return new SpriteSheet(){ name = sheet, width = w, height = h };
        }

        public void Scale(TemporaryAnimatedSprite sprite, float scale)
        {
            sprite.scale = scale;
        }

        public void ScaleTile(TemporaryAnimatedSprite sprite, float scale)
        {
            float mod = 64 / (float)sprite.sourceRect.Width;
            sprite.scale = mod * scale;
        }

        public void SetMovement(TemporaryAnimatedSprite sprite, Vector2 motion)
        {
            sprite.motion = motion;
        }

        public void OnEnd(TemporaryAnimatedSprite sprite, LuaFunction callback)
        {
            sprite.endFunction = (int extraInfo) => {
                api.lua.CallSafe(callback);
            };
        }

        public void End(TemporaryAnimatedSprite sprite) {
            sprite.totalNumberOfLoops = 0;
            sprite.animationLength = 0;
        }

        public void SetRotation(TemporaryAnimatedSprite sprite, float rotation, float rotationRate = 0f)
        {
            sprite.rotation = rotation;
            sprite.rotationChange= rotationRate;
        }

        public void SetPosition(TemporaryAnimatedSprite sprite, Vector2 position)
        {
            sprite.position = position;
        }

        public TemporaryAnimatedSprite HighlightPlacement(Vector2 tile, int state)
        {
            return Create("LooseSprites\\buildingPlacementTiles", state * 64, 0, 64, 64, 1000, 1, 9999, tile * 64f);
        }
    }
}
