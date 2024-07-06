using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaValley.API.Util
{
    internal class Vector: LuaAPI
    {
        public Vector(APIManager api): base(api, "Vector") { }

        public Vector2 Create(float x, float y) { return new Vector2(x, y); }

        public float Distance(Vector2 first, Vector2 second)
        {
            return Vector2.Distance(first, second);
        }

        public Vector2 GetPos(object o)
        {
            if (o is TerrainFeature feat) { return feat.Tile; }
            if (o is Character ch) { return ch.Tile; }
            if (o is Vector2 vec) { return vec; }
            return Vector2.Zero;
        }

        public int GetDirection(object first, object second)
        {
            Vector2 firstPos = GetPos(first);
            Vector2 secondPos = GetPos(second);
            float xDist = firstPos.X - secondPos.X;
            float yDist = firstPos.Y - secondPos.Y;
            if (Math.Abs(xDist) > Math.Abs(yDist))
            {
                return xDist < 0 ? 1 : 3;
            }
            return yDist < 0 ? 2 : 0;
        }

        public float Distance(object first, object second)
        {
            return Vector2.Distance(GetPos(first), GetPos(second));
        }

        public Vector2 TileToPx(Vector2 tile) { return tile * 64f; }

        public Vector2 Add(Vector2 vector, int x, int y)
        {
            return new Vector2(x, y) + vector;
        }

        public Vector2 Add(Vector2 first, Vector2 second)
        {
            return first + second;
        }
    }
}
