Event = { }

function Event:Awake()
    self.callbackList = { }
    self.queue = { }
    self.from = 1
    self.to = 0
    self.maxProcessPerFrame = 1000
    self.processing = false
end

function Event:Notify(name, ...)
    local info = {
        type = 0,
        name = name,
        data = { ... },
    }
    self.to = self.to + 1
    self.queue[self.to] = info
    self:ProcessEvents()
end

function Event:AddCallback(name, f)
    local info = {
        type = 1,
        name = name,
        f = f,
    }
    self.to = self.to + 1
    self.queue[self.to] = info
    self:ProcessEvents()
end

function Event:RemoveCallback(name, f)
    local info = {
        type = 2,
        name = name,
        f = f,
    }
    self.to = self.to + 1
    self.queue[self.to] = info
    self:ProcessEvents()
end

function Event:ProcessEvents()
    if self.processing then return end
    self.processing = true
    
    local n = 0
    for i = 1, self.maxProcessPerFrame do
        local info = self.queue[self.from]
        if info == nil then break end
        self.queue[self.from] = nil     -- 拿出来了就要执行; 执行了就要删除.
        self.from = self.from + 1
        
        local name = info.name
        local type = info.type
        local callbackList = self.callbackList[name]
        if type == 0 then           -- Notify
            if callbackList then
                for f in pairs(callbackList) do
                    xpcall(f, error, table.unpack(info.data))
                end
            end
            
        elseif type == 1 then       -- Add
            if not callbackList then
                callbackList = { }
                self.callbackList[name] = callbackList
            end
            callbackList[info.f] = true
            
        elseif type == 2 then       -- Remove
            if not callbackList then return end
            callbackList[info.f] = nil
            
        else
            error("未知操作类型")
        end
    end
    
    self.processing = false
end


return Event
