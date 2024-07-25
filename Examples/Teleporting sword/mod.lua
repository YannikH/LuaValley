isWarping = false

function afterUse()
  LogAPI:Info("Already warping? " .. tostring(isWarping))
  monsters = WorldAPI:GetMonsters()
  player = CharacterAPI:GetPlayer()
  -- if there's no monsters in my location, teleport!
  if (#monsters == 0) then
    prepareTeleport()
    return false
  end
  nearMonsterCount = 0
  for i,monster in ipairs(monsters) do
    dist = Vector:Distance(player, monster)
    if (dist < 5) then
      nearMonsterCount = nearMonsterCount + 1
    end
  end
  LogAPI:Info("Near monsters " .. nearMonsterCount)
  -- if one of the monsters is within 5 tiles, end the script
  if (nearMonsterCount == 0) then
    -- if no monsters were within 5 tiles, teleport anyways!
    prepareTeleport()
    return false
  end
  -- if we've got 5 in 5 tiles, add mild buffs
  if nearMonsterCount > 4 then
    GameAPI:DebugCommand("buff 20")
    GameAPI:DebugCommand("buff 22")
  end
  -- if we've got 5 in 5 tiles, become temporarily invincible
  if nearMonsterCount > 10 then
    GameAPI:DebugCommand("buff 21")
  end
  isWarping = false
end

function prepareTeleport()
  textId = TextAPI:DrawText("Your sword seeks purpose", Vector:Add(Vector:GetPos(player), 0, 2))
  GameAPI:ExecuteAfter(1000, function()
    TextAPI:ClearText()
    teleport()
  end)
end

function teleport()
  randomNum = math.random(1,200)
  if (randomNum % 10 == 0) then randomNum = randomNum + 1 end
  GameAPI:DebugCommand("minelevel " .. randomNum)
end

function BeforeTeleSword()
  if isWarping then return end
  isWarping = true
  GameAPI:ExecuteAfter(100, afterUse)
  return true
end

function saveloaded()
  GameAPI:AddEventHandler("LocationChanged", function() isWarping = false end)
  GameAPI:DebugCommand("item YH.LuaExampleMod_TeleSword")
end