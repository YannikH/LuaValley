# LuaValley

A Lua implementation for stardew valley

## Status

This is still an experiment/prototype, it is not yet ready for release, but I've made it publicly visible so people can look at the code.

## Design/goal

The goal of this project is to offer up a safe and easy to use Lua scripting impleentation for StardewValley. Some of the guiding principles are:

- Access to base game objects should be minimal (For example, Game1 should not be available in Lua)
- Lua code should avoid valling functions on base game objects
- Lua code should fail safely
  - All Lua APIs should handle errors without causing crashes to desktop
  - One Lua mod's errors should not break other mods or the base game
- Lua APIs are not object oriented
  - One of the goals of this project is to make modding simpler for users with minimal programming experience
  - Because of this, API functions should take game objects as arguments, instead of adding methods to game object instances.
- Some basic debugging should be available without using visual studio

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

     Var:GetStoreKey(Object store), returns: String
     Var:Set(Object store, String key, Object value)
     Var:Set(String key, Object value)
     Var:Get(Object store, String key, Object fallback(optional)), returns: Object
     Var:Get(String key, Object fallback(optional)), returns: Object

### Vector

     Vector:Create(Single x, Single y), returns: Vector2
     Vector:Distance(Vector2 first, Vector2 second), returns: Single
     Vector:GetPos(Object o), returns: Vector2
     Vector:GetDirection(Object first, Object second), returns: Int32
     Vector:Distance(Object first, Object second), returns: Single
     Vector:TileToPx(Vector2 tile), returns: Vector2
     Vector:Add(Vector2 vector, Int32 x, Int32 y), returns: Vector2
     Vector:Add(Vector2 first, Vector2 second), returns: Vector2

### ChatAPI

     ChatAPI:Error(String text)
     ChatAPI:Info(String text)
     ChatAPI:Info(Object[] arguments)

### LogAPI

     LogAPI:Info(String content)
     LogAPI:Error(String content)
     LogAPI:Warn(String content)
     LogAPI:Inspect(Object e)

### TextAPI

     TextAPI:Localize(String key, Object args), returns: String
     TextAPI:DrawTextBubble(String text, Vector2 tilePos, Boolean requireHover = False, String key(optional)), returns: String
     TextAPI:DrawText(String text, Vector2 tilePos, String key(optional)), returns: String
     TextAPI:removeDrawing(String key)
     TextAPI:ClearText()

### UIAPI

     UIAPI:CreateNumberMenu(String text, LuaFunction callback, Int32 price = -1, Int32 minValue = 0, Int32 maxValue = 99, Int32 defaultNumber = 0)
     UIAPI:CreateNamingMenu(String title, LuaFunction callback, String defaultName(optional))
     UIAPI:CreateTextInputMenu(String title, LuaFunction callback, String defaultName(optional))
     UIAPI:CreateConfirmationMenu(String message, LuaFunction onAccept, LuaFunction onCancel)
     UIAPI:CreateMailMenu(String title, String text)
     UIAPI:CreateChoiceMenu(LuaTable options, LuaFunction chooseAction)
     UIAPI:CreateItemGrabMenu(LuaTable itemsTable, Boolean essential = False)
     UIAPI:CreateItemListMenu(String title, LuaTable itemsTable)
     UIAPI:CreateShopMenu(LuaTable inventory)
     UIAPI:CreateAnimationPreviewTool()
     UIAPI:CloseMenu()
     UIAPI:SetCursor(String cursor = default)

### DialogueAPI

     DialogueAPI:Create(String content)
     DialogueAPI:Create(String content, NPC character, String emotion = Neutral)
     DialogueAPI:Create(String content, LuaTable responses, LuaFunction callback)

### GameAPI

     GameAPI:AddEventHandler(String type, LuaFunction callback, Boolean triggerOnce = False), returns: String
     GameAPI:RemoveEvent(String name)
     GameAPI:Reset()
     GameAPI:DebugCommand(String command), returns: String
     GameAPI:ExecuteWhenFree(LuaFunction function)
     GameAPI:RegisterTrigger(String name)
     GameAPI:RaiseTrigger(String name, Object[] args)
     GameAPI:RegisterAction(String name, LuaFunction function)
     GameAPI:ExecuteAfter(Int32 milliseconds, LuaFunction function)

### MultiplayerAPI

     MultiplayerAPI:OnModMessageReceived(Object sender, ModMessageReceivedEventArgs e)
     MultiplayerAPI:SendMessage(Object content)
     MultiplayerAPI:AddMessageHandler(LuaFunction callback, Boolean triggerOnce = False), returns: String
     MultiplayerAPI:RemoveMessageHandler(String name)
     MultiplayerAPI:ClearHandlers()

### ReflectionAPI

     ReflectionAPI:GetBool(Object target, String fieldName), returns: Boolean
     ReflectionAPI:GetString(Object target, String fieldName), returns: String
     ReflectionAPI:GetInt(Object target, String fieldName), returns: Int32
     ReflectionAPI:GetFloat(Object target, String fieldName), returns: Single
     ReflectionAPI:SetBool(Object target, String fieldName, Boolean value)
     ReflectionAPI:SetString(Object target, String fieldName, String value)
     ReflectionAPI:SetInt(Object target, String fieldName, Int32 value)
     ReflectionAPI:SetFloat(Object target, String fieldName, Single value)
     ReflectionAPI:GetValue(Object target, String fieldName), returns: T
     ReflectionAPI:SetValue(Object target, String fieldName, T value)

### SaveAPI

     SaveAPI:Set(String key, Int32 value)
     SaveAPI:Set(String key, String value)
     SaveAPI:Get(String key), returns: Object

### SpriteAPI

     SpriteAPI:Create(String sheet, Int32 x, Int32 y, Int32 w, Int32 h, Single animationInterval, Int32 animationLength, Int32 loops, Vector2 pixelPos, Boolean flipped = False), returns: TemporaryAnimatedSprite
     SpriteAPI:Create(SpriteSheet sheet, Int32 x, Int32 y, Vector2 tilePos, Boolean flipped = False), returns: TemporaryAnimatedSprite
     SpriteAPI:CreateSheet(String sheet, Int32 w, Int32 h), returns: SpriteSheet
     SpriteAPI:Scale(TemporaryAnimatedSprite sprite, Single scale)
     SpriteAPI:ScaleTile(TemporaryAnimatedSprite sprite, Single scale)
     SpriteAPI:SetMovement(TemporaryAnimatedSprite sprite, Vector2 motion)
     SpriteAPI:OnEnd(TemporaryAnimatedSprite sprite, LuaFunction callback)
     SpriteAPI:End(TemporaryAnimatedSprite sprite)
     SpriteAPI:SetRotation(TemporaryAnimatedSprite sprite, Single rotation, Single rotationRate = 0)
     SpriteAPI:SetPosition(TemporaryAnimatedSprite sprite, Vector2 position)
     SpriteAPI:HighlightPlacement(Vector2 tile, Int32 state), returns: TemporaryAnimatedSprite

### WorldAPI

     WorldAPI:CreateTileActionName(), returns: String
     WorldAPI:GetMouseTile(), returns: Vector2
     WorldAPI:LocationName(), returns: String
     WorldAPI:LocationName(GameLocation loc), returns: String
     WorldAPI:GetLocation(String locationName(optional)), returns: GameLocation
     WorldAPI:GetEmptyTile(Vector2 tile, String locationName(optional)), returns: Vector2
     WorldAPI:GetCharacters(String locationName(optional)), returns: LuaTable
     WorldAPI:GetMonsters(String locationName(optional)), returns: LuaTable
     WorldAPI:IsOpenTile(Vector2 tile, String locationName(optional)), returns: Boolean
     WorldAPI:ForEachBuilding(LuaFunction function, Boolean ignoreConstruction = True)
     WorldAPI:ForEachLocation(LuaFunction function)
     WorldAPI:SetTileClickAction(Vector2 tile, String action, String locationName(optional), Boolean passable = True, Int32 tileIndex = 48)
     WorldAPI:SetTileClickAction(Vector2 tile, LuaFunction action, String locationName(optional), Boolean passable = True, Int32 tileIndex = 48)
     WorldAPI:RegisterTileAction(String name, LuaFunction action)
     WorldAPI:SetTileTouchAction(Vector2 tile, String action, String locationName(optional), Int32 tileIndex = 48)
     WorldAPI:SetMapTileIndex(Vector2 tile, Int32 index, String layer, Int32 tileSheet = 0, String locationName(optional))
     WorldAPI:GetMapTileIndex(Vector2 tile, String layer, String locationName(optional)), returns: Int32

### EventAPI

     EventAPI:HasActiveEvent(), returns: Boolean
     EventAPI:PlayScript(String script, String locationName(optional)), returns: Event
     EventAPI:CreateEvent(), returns: Event
     EventAPI:EndEvent()
     EventAPI:CurrentEvent(), returns: Event
     EventAPI:AddScript(Event evt, String script)
     EventAPI:AllowControl(Boolean canRun = False)
     EventAPI:EndControl(Event evt)
     EventAPI:Play(Event evt, String locationName(optional))

### CharacterAPI

     CharacterAPI:GetNPC(String name), returns: NPC
     CharacterAPI:GetPlayer(), returns: Farmer
     CharacterAPI:Get(String name), returns: Farmer
     CharacterAPI:GetHost(), returns: Farmer
     CharacterAPI:GetInventory(), returns: LuaTable
     CharacterAPI:GetInventory(Farmer farmer), returns: LuaTable
     CharacterAPI:OnTile(Int32 x, Int32 y, String locationName(optional)), returns: Character
     CharacterAPI:GetTilePos(Character character), returns: Vector2
     CharacterAPI:GetPlayerTilePos(), returns: Vector2
     CharacterAPI:GetLocationName(Character character), returns: String
     CharacterAPI:Say(NPC character, String text, String emotion = neutral)
     CharacterAPI:Say(NPC character, LuaTable lines, String emotion = Neutral)
     CharacterAPI:Warp(Vector2 tile, Character character, String locationName(optional))
     CharacterAPI:ForEachCharacter(LuaFunction function, Boolean includeEventActors = True)
     CharacterAPI:ForEachVillager(LuaFunction function, Boolean includeEventActors = True)
     CharacterAPI:CreateNPC(String spritePath, Vector2 tile), returns: NPC
     CharacterAPI:ResizeSprite(Character character, Vector2 size)
     CharacterAPI:PlayAnimation(Character character, LuaTable framesTable)
     CharacterAPI:ClearAnimation(Character character)
     CharacterAPI:EndAnimation(Character character)
     CharacterAPI:FaceDirection(Character character, Int32 direction)
     CharacterAPI:CreateMonster(String name, Vector2 tile), returns: Monster
     CharacterAPI:MoveTo(NPC character, Vector2 tile, LuaFunction callback(optional))
     CharacterAPI:IsMonster(Character character), returns: Boolean

### ObjectAPI

     ObjectAPI:Create(String id), returns: Object
     ObjectAPI:AddToInventory(Object obj)
     ObjectAPI:AddToInventory(Farmer f, Object obj)
     ObjectAPI:Drop(Object obj, Vector2 pos, String locationName(optional))
     ObjectAPI:Remove(Object obj, String locationName(optional))
     ObjectAPI:SetSprite(Object obj, Int32 id)
     ObjectAPI:Place(Object obj, Vector2 tilePos, String locationName(optional))
     ObjectAPI:SetPos(Object obj, Vector2 tilePos)
     ObjectAPI:SetInvisible(Object obj, Boolean invisible)
     ObjectAPI:FromTile(Int32 x, Int32 y, String locationName(optional)), returns: Object
     ObjectAPI:ActiveObject(), returns: Object
     ObjectAPI:GetItems(Object obj), returns: LuaTable
     ObjectAPI:ForEachCrop(LuaFunction function)
     ObjectAPI:ForEachItem(LuaFunction function)
     ObjectAPI:ForEachItemIn(String locationName, LuaFunction function)

### TerrainFeatureAPI

     TerrainFeatureAPI:GetTerrainFeatures(String locationName(optional)), returns: LuaTable
     TerrainFeatureAPI:IsTree(TerrainFeature feature), returns: Boolean
     TerrainFeatureAPI:IsFruitTree(TerrainFeature feature), returns: Boolean
     TerrainFeatureAPI:IsGiantCrop(TerrainFeature feature), returns: Boolean
     TerrainFeatureAPI:ApplyDamage(TerrainFeature t, Int32 damage)
     TerrainFeatureAPI:PerformUseAction(TerrainFeature t)
     TerrainFeatureAPI:CreateHoeDirt(Vector2 tile, String locationName(optional)), returns: HoeDirt
     TerrainFeatureAPI:CreateFruitTree(Vector2 tile, String id, Int32 growthStage = 0, String locationName(optional)), returns: FruitTree
     TerrainFeatureAPI:CreateTree(Vector2 tile, String id, Int32 growthStage = 0, Boolean isGreenRainTemporaryTree = False, String locationName(optional)), returns: Tree
     TerrainFeatureAPI:CreateBush(Vector2 tile, Int32 size, Int32 datePlantedOverride = -1, String locationName(optional)), returns: Bush
     TerrainFeatureAPI:CreateCosmeticPlant(Vector2 tile, Int32 id, String locationName(optional)), returns: CosmeticPlant
     TerrainFeatureAPI:CreateFlooring(Vector2 tile, String id, String locationName(optional)), returns: Flooring
     TerrainFeatureAPI:CreateGiantCrop(Vector2 tile, String id, String locationName(optional)), returns: GiantCrop
     TerrainFeatureAPI:CreateCosmeticPlant(Vector2 tile, Int32 id, Int32 numWeeds = 0, String locationName(optional)), returns: Grass
     TerrainFeatureAPI:AddToLocation(Vector2 tile, TerrainFeature feature, String locationName(optional)), returns: TerrainFeature

### ToolAPI

     ToolAPI:ActiveTool(), returns: Tool
     ToolAPI:OverrideToolFunction(String itemId, LuaFunction useFunction)
     ToolAPI:AddBeforeUseFunction(String itemId, LuaFunction beforeFunction)
     ToolAPI:DisableTool(String itemId)
     ToolAPI:RestoreTool(String itemId)
     ToolAPI:UseTool()
     ToolAPI:Reset()
