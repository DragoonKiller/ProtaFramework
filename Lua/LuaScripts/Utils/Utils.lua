
--=====================================================================================================================
--=====================================================================================================================

local function ReplaceLib(t, field)
    local original = t[field]
    t[":" .. field .. ":"] = original
    return original
end


--=====================================================================================================================
--=====================================================================================================================

do -- error & assertion
    
    local original = ReplaceLib(_G, "assert")
    _G.assert = function(cond, ...)
        if not cond then
            CS.UnityEngine.Debug.Log.Error("LUA: Assertion failed: ", string.format(...))
        end
        return cond
    end
    
    local original = ReplaceLib(_G, "error")
    _G.error = function(...)
        CS.UnityEngine.Debug.Log.Error("LUA: ", string.format(...))
    end
    
end

--=====================================================================================================================
--=====================================================================================================================

do -- string
    
    local original = ReplaceLib(string, "format")
    string.format = function(...)
        return xpcall(original, error, ...)
    end
    
end

--=====================================================================================================================
--=====================================================================================================================

do -- UnityEngine & Lua Interaction
    
    function GetLuaObject(go)
        local g = go:GetComponent(typeof(CS.Prota.Lua.LuaScript))
        assert(g)
    end
    
end
