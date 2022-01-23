local Game = { }

function Game:Awake()
    DontDestroyOnLoad(self.gameObject)
end

return Game