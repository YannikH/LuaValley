using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLua;
using LuaValley.API.Game;

namespace LuaValley.API.Entities
{
    internal class TerrainFeatureAPI : LuaAPI
    {
        public TerrainFeatureAPI(APIManager api) : base(api, "TerrainFeatureAPI") { }

        public LuaTable GetTerrainFeatures(string locationName = null)
        {
            GameLocation loc = api.GetAPI<WorldAPI>().GetLocation(locationName);
            var features = loc.terrainFeatures.Values.ToArray();
            return api.lua.ToTable(features);
        }

        public bool IsTree(TerrainFeature feature) { return feature is Tree; }
        public bool IsFruitTree(TerrainFeature feature) { return feature is FruitTree; }
        public bool IsGiantCrop(TerrainFeature feature) { return feature is GiantCrop; }

        public void ApplyDamage(TerrainFeature t, int damage)
        {
            try
            {
                t.performToolAction(null, damage, t.Tile);
            }
            catch (Exception e) { api.mod.Monitor.Log("Failed to apply damage to terrain feature", StardewModdingAPI.LogLevel.Error); }
        }

        public void PerformUseAction(TerrainFeature t)
        {
            t.performUseAction(t.Tile);
        }

        public HoeDirt CreateHoeDirt(Vector2 tile, string locationName = null)
        {
            return (HoeDirt)AddToLocation(tile, new HoeDirt(), locationName);
        }

        public FruitTree CreateFruitTree(Vector2 tile, string id, int growthStage = 0, string locationName = null)
        {
            return (FruitTree)AddToLocation(tile, new FruitTree(id, growthStage), locationName);
        }

        public Tree CreateTree(Vector2 tile, string id, int growthStage = 0, bool isGreenRainTemporaryTree = false, string locationName = null)
        {
            return (Tree)AddToLocation(tile, new Tree(id, growthStage, isGreenRainTemporaryTree), locationName);
        }

        public Bush CreateBush(Vector2 tile, int size, int datePlantedOverride = -1, string locationName = null)
        {
            GameLocation loc = api.GetAPI<WorldAPI>().GetLocation(locationName);
            Bush bush = new Bush(tile, size, loc, datePlantedOverride);
            loc.terrainFeatures.Add(tile, bush);
            return bush;
        }

        public CosmeticPlant CreateCosmeticPlant(Vector2 tile, int id, string locationName = null)
        {
            return (CosmeticPlant)AddToLocation(tile, new CosmeticPlant(id), locationName);
        }

        public Flooring CreateFlooring(Vector2 tile, string id, string locationName = null)
        {
            return (Flooring)AddToLocation(tile, new Flooring(id), locationName);
        }

        public GiantCrop CreateGiantCrop(Vector2 tile, string id, string locationName = null)
        {
            return (GiantCrop)AddToLocation(tile, new GiantCrop(id, tile), locationName);
        }

        public Grass CreateCosmeticPlant(Vector2 tile, int id, int numWeeds = 0, string locationName = null)
        {
            return (Grass)AddToLocation(tile, new Grass(id, numWeeds), locationName);
        }

        public TerrainFeature AddToLocation(Vector2 tile, TerrainFeature feature, string locationName = null)
        {
            GameLocation loc = api.GetAPI<WorldAPI>().GetLocation(locationName);
            try
            {
                loc.terrainFeatures.Add(tile, feature);
            }
            catch
            {
                api.mod.Monitor.Log("Failed to add object to location", StardewModdingAPI.LogLevel.Error);
            }
            return feature;
        }
    }
}
