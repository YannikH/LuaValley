# LuaValley

A Lua implementation for stardew valley

## Status

This is still an experiment/prototype, it is not yet ready for release, but I've made it publicly visible so people can look at the code.

## Design

The goal of this project is to offer up a safe and easy to use Lua scripting impleentation for StardewValley. Some of the guiding principles are:

- Access to base game objects should be minimal (For example, Game1 should not be available in Lua)
- Lua code should avoid valling functions on base game objects
- Lua code should fail safely
  - All Lua APIs should handle errors without causing crashes to desktop
  - One Lua mod's errors should not break other mods or the base game
- Lua APIs are not object oriented
  - One of the goals of this project is to make modding simpler for users with minimal programming experience
  - Because of this, API functions should take game objects as arguments, instead of adding methods to game object instances.

## Setup

Currently, lua mods should be implemented as content packs. The goal is to move this over to dependencies, so you can use lua inside contentpack mods.

**Manifest.json**

The current setup to enable lua on a mod is:

```json
  "ContentPackFor": {
    "UniqueID": "YH.LuaValley"
  }
```

**mod.lua**

The entry point for any mod is the `mod.lua`, this file should be in the same folder as your `manifest.json`, by default, LuaValley will attempt to call two functions in your mod.lua file.

```lua
function gamestart()
  LogAPI:Info("This code runs when the game is first started")
end

function saveloaded()
  LogAPI:Info("This code runs when you load a game")
end
```

## Examples

### Creating a tree whenever a player clicks somewhere

```lua
function onButtonPressed(args)
  if (args.Button:ToString() == "MouseLeft") then
    mouseTile = WorldAPI:GetMouseTile()
    TerrainFeatureAPI:CreateTree(mouseTile, "8")
  end
end

function gamestart()
  GameAPI:AddEventHandler("ButtonPressed", onButtonPressed)
end
```

### Disabling the scythe, and making the default pickaxe only work every other swing

```lua
counter = 0
function onBeforeCopperPickaxe()
  canUse = (counter % 2 == 0)
  counter = counter + 1;
  if not canUse then
    ChatAPI:Info("Sorry, you can only use your tool every other click!")
  end
  return canUse -- if a before use function returns false, it cancels the tool use
end

function onCopperPickaxeUse()
  ChatAPI:Info("Copper pickaxe is being used!")
    ToolAPI:UseTool()
end

function gamestart()
  ToolAPI:DisableTool("47");
  ToolAPI:AddBeforeUseFunction("Pickaxe", onBeforeCopperPickaxe)
  ToolAPI:OverrideToolFunction("Pickaxe", onCopperPickaxeUse)
end
```

## APIs

Below is an auto-generated list of currently implemented API functions.
The ReflectionAPI and MultiplayerAPI act as wrappers for SMAPI APIs and have not been tested.

### Var

     Var:GetStoreKey(System.Object store), returns: System.String
     Var:Set(System.Object store, System.String key, System.Object value)
     Var:Set(System.String key, System.Object value)
     Var:Get(System.Object store, System.String key, System.Object fallback(optional)), returns: System.Object
     Var:Get(System.String key, System.Object fallback(optional)), returns: System.Object

### Vector

     Vector:Create(System.Single x, System.Single y), returns: Microsoft.Xna.Framework.Vector2
     Vector:Distance(Microsoft.Xna.Framework.Vector2 first, Microsoft.Xna.Framework.Vector2 second), returns: System.Single
     Vector:GetPos(System.Object o), returns: Microsoft.Xna.Framework.Vector2
     Vector:GetDirection(System.Object first, System.Object second), returns: System.Int32
     Vector:Distance(System.Object first, System.Object second), returns: System.Single
     Vector:TileToPx(Microsoft.Xna.Framework.Vector2 tile), returns: Microsoft.Xna.Framework.Vector2
     Vector:Add(Microsoft.Xna.Framework.Vector2 vector, System.Int32 x, System.Int32 y), returns: Microsoft.Xna.Framework.Vector2
     Vector:Add(Microsoft.Xna.Framework.Vector2 first, Microsoft.Xna.Framework.Vector2 second), returns: Microsoft.Xna.Framework.Vector2

### ChatAPI

     ChatAPI:Info(System.Object[] arguments)
     ChatAPI:Error(System.String text)
     ChatAPI:Info(System.String text)

### LogAPI

     LogAPI:Info(System.String content)
     LogAPI:Error(System.String content)
     LogAPI:Warn(System.String content)
     LogAPI:Inspect(System.Object e)
     LogAPI:Console()

### TextAPI

     TextAPI:Localize(System.String key, System.Object args), returns: System.String
     TextAPI:DrawTextBubble(System.String text, Microsoft.Xna.Framework.Vector2 tilePos, System.Boolean requireHover = False, System.String key(optional)), returns: System.String
     TextAPI:DrawText(System.String text, Microsoft.Xna.Framework.Vector2 tilePos, System.String key(optional)), returns: System.String
     TextAPI:removeDrawing(System.String key)
     TextAPI:ClearText()

### UIAPI

     UIAPI:CreateNumberMenu(System.String text, NLua.LuaFunction callback, System.Int32 price = -1, System.Int32 minValue = 0, System.Int32 maxValue = 99, System.Int32 defaultNumber = 0)
     UIAPI:CreateNamingMenu(System.String title, NLua.LuaFunction callback, System.String defaultName(optional))
     UIAPI:CreateTextInputMenu(System.String title, NLua.LuaFunction callback, System.String defaultName(optional))
     UIAPI:CreateConfirmationMenu(System.String message, NLua.LuaFunction onAccept, NLua.LuaFunction onCancel)
     UIAPI:CreateMailMenu(System.String title, System.String text)
     UIAPI:CreateChoiceMenu(NLua.LuaTable options, NLua.LuaFunction chooseAction)
     UIAPI:CreateItemGrabMenu(NLua.LuaTable itemsTable, System.Boolean essential = False)
     UIAPI:CreateItemListMenu(System.String title, NLua.LuaTable itemsTable)
     UIAPI:CreateShopMenu(NLua.LuaTable inventory)
     UIAPI:CreateAnimationPreviewTool()
     UIAPI:CloseMenu()

### DialogueAPI

     DialogueAPI:Create(System.String content)
     DialogueAPI:Create(System.String content, StardewValley.NPC character, System.String emotion = Neutral)
     DialogueAPI:Create(System.String content, NLua.LuaTable responses, NLua.LuaFunction callback)

### GameAPI

     GameAPI:AddEventHandler(System.String type, NLua.LuaFunction callback, System.Boolean triggerOnce = False), returns: System.String
     GameAPI:CleanupEvents(System.Object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
     GameAPI:RemoveEvent(System.String name)
     GameAPI:Reset()
     GameAPI:SetCursor(System.String cursor = default)
     GameAPI:DebugCommand(System.String command), returns: System.String
     GameAPI:ExecuteWhenFree(NLua.LuaFunction function)
     GameAPI:RegisterTrigger(System.String name)
     GameAPI:RaiseTrigger(System.String name, System.Object[] args)
     GameAPI:RegisterAction(System.String name, NLua.LuaFunction function)
     GameAPI:RunFunctionQueue(System.Object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
     GameAPI:ExecuteAfter(System.Int32 milliseconds, NLua.LuaFunction function)

### MultiplayerAPI

     MultiplayerAPI:SendMessage(System.Object content)
     MultiplayerAPI:OnModMessageReceived(System.Object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
     MultiplayerAPI:AddMessageHandler(NLua.LuaFunction callback, System.Boolean triggerOnce = False), returns: System.String
     MultiplayerAPI:RemoveMessageHandler(System.String name)
     MultiplayerAPI:ClearHandlers()

### ReflectionAPI

     ReflectionAPI:GetBool(System.Object target, System.String fieldName), returns: System.Boolean
     ReflectionAPI:GetString(System.Object target, System.String fieldName), returns: System.String
     ReflectionAPI:GetInt(System.Object target, System.String fieldName), returns: System.Int32
     ReflectionAPI:GetFloat(System.Object target, System.String fieldName), returns: System.Single
     ReflectionAPI:SetBool(System.Object target, System.String fieldName, System.Boolean value)
     ReflectionAPI:SetString(System.Object target, System.String fieldName, System.String value)
     ReflectionAPI:SetInt(System.Object target, System.String fieldName, System.Int32 value)
     ReflectionAPI:SetFloat(System.Object target, System.String fieldName, System.Single value)
     ReflectionAPI:GetValue(System.Object target, System.String fieldName), returns: T
     ReflectionAPI:SetValue(System.Object target, System.String fieldName, T value)

### SaveAPI

     SaveAPI:Set(System.String key, System.Int32 value)
     SaveAPI:Set(System.String key, System.String value)
     SaveAPI:Get(System.String key), returns: System.Object

### SpriteAPI

     SpriteAPI:Create(System.String sheet, System.Int32 x, System.Int32 y, System.Int32 w, System.Int32 h, System.Single animationInterval, System.Int32 animationLength, System.Int32 loops, Microsoft.Xna.Framework.Vector2 pixelPos, System.Boolean flipped = False), returns: StardewValley.TemporaryAnimatedSprite
     SpriteAPI:Create(LuaValley.API.Game.SpriteSheet sheet, System.Int32 x, System.Int32 y, Microsoft.Xna.Framework.Vector2 tilePos, System.Boolean flipped = False), returns: StardewValley.TemporaryAnimatedSprite
     SpriteAPI:CreateSheet(System.String sheet, System.Int32 w, System.Int32 h), returns: LuaValley.API.Game.SpriteSheet
     SpriteAPI:Scale(StardewValley.TemporaryAnimatedSprite sprite, System.Single scale)
     SpriteAPI:ScaleTile(StardewValley.TemporaryAnimatedSprite sprite, System.Single scale)
     SpriteAPI:SetMovement(StardewValley.TemporaryAnimatedSprite sprite, Microsoft.Xna.Framework.Vector2 motion)
     SpriteAPI:OnEnd(StardewValley.TemporaryAnimatedSprite sprite, NLua.LuaFunction callback)
     SpriteAPI:End(StardewValley.TemporaryAnimatedSprite sprite)
     SpriteAPI:SetRotation(StardewValley.TemporaryAnimatedSprite sprite, System.Single rotation, System.Single rotationRate = 0)
     SpriteAPI:SetPosition(StardewValley.TemporaryAnimatedSprite sprite, Microsoft.Xna.Framework.Vector2 position)
     SpriteAPI:HighlightPlacement(Microsoft.Xna.Framework.Vector2 tile, System.Int32 state), returns: StardewValley.TemporaryAnimatedSprite

### WorldAPI

     WorldAPI:CreateTileActionName(), returns: System.String
     WorldAPI:GetMouseTile(), returns: Microsoft.Xna.Framework.Vector2
     WorldAPI:LocationName(), returns: System.String
     WorldAPI:LocationName(StardewValley.GameLocation loc), returns: System.String
     WorldAPI:GetLocation(System.String locationName(optional)), returns: StardewValley.GameLocation
     WorldAPI:GetEmptyTile(Microsoft.Xna.Framework.Vector2 tile, System.String locationName(optional)), returns: Microsoft.Xna.Framework.Vector2
     WorldAPI:IsOpenTile(Microsoft.Xna.Framework.Vector2 tile, System.String locationName(optional)), returns: System.Boolean
     WorldAPI:ForEachBuilding(NLua.LuaFunction function, System.Boolean ignoreConstruction = True)
     WorldAPI:ForEachLocation(NLua.LuaFunction function)
     WorldAPI:SetTileClickAction(Microsoft.Xna.Framework.Vector2 tile, System.String action, System.String locationName(optional), System.Boolean passable = True, System.Int32 tileIndex = 48)
     WorldAPI:SetTileClickAction(Microsoft.Xna.Framework.Vector2 tile, NLua.LuaFunction action, System.String locationName(optional), System.Boolean passable = True, System.Int32 tileIndex = 48)
     WorldAPI:RegisterTileAction(System.String name, NLua.LuaFunction action)
     WorldAPI:SetTileTouchAction(Microsoft.Xna.Framework.Vector2 tile, System.String action, System.String locationName(optional), System.Int32 tileIndex = 48)
     WorldAPI:SetMapTileIndex(Microsoft.Xna.Framework.Vector2 tile, System.Int32 index, System.String layer, System.Int32 tileSheet = 0, System.String locationName(optional))
     WorldAPI:GetMapTileIndex(Microsoft.Xna.Framework.Vector2 tile, System.String layer, System.String locationName(optional)), returns: System.Int32

### EventAPI

     EventAPI:HasActiveEvent(), returns: System.Boolean
     EventAPI:PlayScript(System.String script, System.String locationName(optional)), returns: StardewValley.Event
     EventAPI:CreateEvent(), returns: StardewValley.Event
     EventAPI:EndEvent()
     EventAPI:CurrentEvent(), returns: StardewValley.Event
     EventAPI:AddScript(StardewValley.Event evt, System.String script)
     EventAPI:AllowControl(System.Boolean canRun = False)
     EventAPI:EndControl(StardewValley.Event evt)
     EventAPI:Play(StardewValley.Event evt, System.String locationName(optional))

### CharacterAPI

     CharacterAPI:GetNPC(System.String name), returns: StardewValley.NPC
     CharacterAPI:GetPlayer(), returns: StardewValley.Farmer
     CharacterAPI:Get(System.String name), returns: StardewValley.Farmer
     CharacterAPI:GetHost(), returns: StardewValley.Farmer
     CharacterAPI:GetInventory(), returns: NLua.LuaTable
     CharacterAPI:GetInventory(StardewValley.Farmer farmer), returns: NLua.LuaTable
     CharacterAPI:OnTile(System.Int32 x, System.Int32 y, System.String locationName(optional)), returns: StardewValley.Character
     CharacterAPI:GetTilePos(StardewValley.Character character), returns: Microsoft.Xna.Framework.Vector2
     CharacterAPI:GetPlayerTilePos(), returns: Microsoft.Xna.Framework.Vector2
     CharacterAPI:GetLocationName(StardewValley.Character character), returns: System.String
     CharacterAPI:Say(StardewValley.NPC character, System.String text, System.String emotion = neutral)
     CharacterAPI:Say(StardewValley.NPC character, NLua.LuaTable lines, System.String emotion = Neutral)
     CharacterAPI:Warp(Microsoft.Xna.Framework.Vector2 tile, StardewValley.Character character, System.String locationName(optional))
     CharacterAPI:ForEachCharacter(NLua.LuaFunction function, System.Boolean includeEventActors = True)
     CharacterAPI:ForEachVillager(NLua.LuaFunction function, System.Boolean includeEventActors = True)
     CharacterAPI:CreateNPC(System.String spritePath, Microsoft.Xna.Framework.Vector2 tile), returns: StardewValley.NPC
     CharacterAPI:ResizeSprite(StardewValley.Character character, Microsoft.Xna.Framework.Vector2 size)
     CharacterAPI:PlayAnimation(StardewValley.Character character, NLua.LuaTable framesTable)
     CharacterAPI:ClearAnimation(StardewValley.Character character)
     CharacterAPI:EndAnimation(StardewValley.Character character)
     CharacterAPI:FaceDirection(StardewValley.Character character, System.Int32 direction)
     CharacterAPI:CreateMonster(System.String name, Microsoft.Xna.Framework.Vector2 tile), returns: StardewValley.Monsters.Monster
     CharacterAPI:MoveTo(StardewValley.NPC character, Microsoft.Xna.Framework.Vector2 tile, NLua.LuaFunction callback(optional))
     CharacterAPI:SetUsingTool(System.Boolean val)
     CharacterAPI:DisableTool()

### ObjectAPI

     ObjectAPI:Create(System.String id), returns: StardewValley.Object
     ObjectAPI:AddToInventory(StardewValley.Object obj)
     ObjectAPI:AddToInventory(StardewValley.Farmer f, StardewValley.Object obj)
     ObjectAPI:Drop(StardewValley.Object obj, Microsoft.Xna.Framework.Vector2 pos, System.String locationName(optional))
     ObjectAPI:Remove(StardewValley.Object obj, System.String locationName(optional))
     ObjectAPI:SetSprite(StardewValley.Object obj, System.Int32 id)
     ObjectAPI:Place(StardewValley.Object obj, Microsoft.Xna.Framework.Vector2 tilePos, System.String locationName(optional))
     ObjectAPI:SetPos(StardewValley.Object obj, Microsoft.Xna.Framework.Vector2 tilePos)
     ObjectAPI:SetInvisible(StardewValley.Object obj, System.Boolean invisible)
     ObjectAPI:FromTile(System.Int32 x, System.Int32 y, System.String locationName(optional)), returns: StardewValley.Object
     ObjectAPI:ActiveObject(), returns: StardewValley.Object
     ObjectAPI:GetItems(StardewValley.Object obj), returns: NLua.LuaTable
     ObjectAPI:ForEachCrop(NLua.LuaFunction function)
     ObjectAPI:ForEachItem(NLua.LuaFunction function)
     ObjectAPI:ForEachItemIn(System.String locationName, NLua.LuaFunction function)

### TerrainFeatureAPI

     TerrainFeatureAPI:GetTerrainFeatures(System.String locationName(optional)), returns: NLua.LuaTable
     TerrainFeatureAPI:IsTree(StardewValley.TerrainFeatures.TerrainFeature feature), returns: System.Boolean
     TerrainFeatureAPI:IsFruitTree(StardewValley.TerrainFeatures.TerrainFeature feature), returns: System.Boolean
     TerrainFeatureAPI:IsGiantCrop(StardewValley.TerrainFeatures.TerrainFeature feature), returns: System.Boolean
     TerrainFeatureAPI:ApplyDamage(StardewValley.TerrainFeatures.TerrainFeature t, System.Int32 damage)
     TerrainFeatureAPI:PerformUseAction(StardewValley.TerrainFeatures.TerrainFeature t)
     TerrainFeatureAPI:CreateHoeDirt(Microsoft.Xna.Framework.Vector2 tile, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.HoeDirt
     TerrainFeatureAPI:CreateFruitTree(Microsoft.Xna.Framework.Vector2 tile, System.String id, System.Int32 growthStage = 0, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.FruitTree
     TerrainFeatureAPI:CreateTree(Microsoft.Xna.Framework.Vector2 tile, System.String id, System.Int32 growthStage = 0, System.Boolean isGreenRainTemporaryTree = False, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.Tree
     TerrainFeatureAPI:CreateBush(Microsoft.Xna.Framework.Vector2 tile, System.Int32 size, System.Int32 datePlantedOverride = -1, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.Bush
     TerrainFeatureAPI:CreateCosmeticPlant(Microsoft.Xna.Framework.Vector2 tile, System.Int32 id, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.CosmeticPlant
     TerrainFeatureAPI:CreateFlooring(Microsoft.Xna.Framework.Vector2 tile, System.String id, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.Flooring
     TerrainFeatureAPI:CreateGiantCrop(Microsoft.Xna.Framework.Vector2 tile, System.String id, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.GiantCrop
     TerrainFeatureAPI:CreateCosmeticPlant(Microsoft.Xna.Framework.Vector2 tile, System.Int32 id, System.Int32 numWeeds = 0, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.Grass
     TerrainFeatureAPI:AddToLocation(Microsoft.Xna.Framework.Vector2 tile, StardewValley.TerrainFeatures.TerrainFeature feature, System.String locationName(optional)), returns: StardewValley.TerrainFeatures.TerrainFeature

### ToolAPI

     ToolAPI:OverrideToolFunction(System.String itemId, NLua.LuaFunction useFunction)
     ToolAPI:AddBeforeUseFunction(System.String itemId, NLua.LuaFunction beforeFunction)
     ToolAPI:DisableTool(System.String itemId)
     ToolAPI:RestoreTool(System.String itemId)
     ToolAPI:UseTool()
     ToolAPI:Reset()
