local Time = CS.UnityEngine.Time

--=================================================================================================
--=================================================================================================

Timer = { }

function Timer:Awake()
    Timer.instance = self
    self.t = { }
    self.a = { }
end

function Timer:Update()
    for timer in pairs(self.a) do
        self.t[timer] = true
        self.a[timer] = nil
    end
    
    collectgarbage("stop")
    
    local curTime = Time.time
    for timer in pairs(self.t) do
        
        -- 超时检查.
        if timer.t >= curTime and timer.rest > 0 then
            
            -- guard.
            -- 如果 guard 检查不通过, 也算作触发一次, 但是不调用回调函数.
            if timer.g and not IsNull(timer.g) then
                xpcall(timer.f, error, timer)
            end
            
            -- 回调函数中可能设置 timer.rest = 0.
            if timer.rest > 0 then
                timer.rest = timer.rest - 1
                timer.t = timer.t + timer.d
            end
        end
        
        -- 剩余次数检查. 次数为 0 时删除.
        if timer.rest == 0 then
            self.t[timer] = nil
        end
    end
    
    collectgarbage("start")
end

--=================================================================================================
--=================================================================================================

local TimerInstance = { }

-- 设置重复触发次数.
function TimerInstance:Repeat(times)
    self.rest = times or -1
end

-- Timer 下次出发时间.
function TimerInstance:Timeout(t)
    self.t = t + Time.time
end

-- Timer 每触发一次, Timeout 会加上这个数.
function TimerInstance:Interval(d)
    self.d = d
end

-- 设置 GameObject 保护.
function TimerInstance:Guard(g)
    self.g = g
end

-- 设置触发回调.
function TimerInstance:Callback(f)
    self.f = f
end

-- 把自己标记为待删除.
function TimerInstance:Destroy()
    self.rest = 0
end

local TimerInstanceMeta = { __index = TimerInstance }

--=================================================================================================
--=================================================================================================

function Timer:Normal(t, f)
    local timer = setmetatable({ }, TimerInstanceMeta)
    t.t = Time.time + t
    t.d = 0
    t.rest = 1
    t.f = f
    t.real = false
    TimerMgr.a[timer] = true
    return timer
end

function Timer:Repeat(t, f)
    local t = Timer:New(t, f)
    t.rest = -1
    t.d = t
    return t
end


function Timer:WaitFor(c, f)
    local t
    t = Timer:Repeat(0, function()
        local noError, res = xpcall(c, error)
        if noError and res then
            t.rest = 0
            xpcall(f, error)
        end
    end)
    return t
end


return Timer
