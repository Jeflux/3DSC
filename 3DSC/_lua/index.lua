local app = require("app")
PORT = 25566


app:start()
while true do
	-- Updating screens
	Screen.waitVblankStart()
	Screen.refresh()
	Screen.clear(TOP_SCREEN)
	Screen.clear(BOTTOM_SCREEN)
	
	app:tick()
	
	-- Flipping screen
	Screen.flip()
end