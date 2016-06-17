local app = require("app")
PORT = 25566

local lastScreenPress = false
local nowScreenPressed = false
local touchx = 0
local touchy = 0
local lasttx = 0
local lastty = 0

app:start()
while true do
	-- Updating screens
	Screen.waitVblankStart()
	Screen.refresh()
	Screen.clear(TOP_SCREEN)
	Screen.clear(BOTTOM_SCREEN)
	
	app:tick()
	
	
	lastScreenPress = nowScreenPressed
	nowScreenPressed = Controls.check(Controls.read(), KEY_TOUCH)
	
	lasttx = touchx
	lastty = touchy
	touchx, touchy = Controls.readTouch()
	if lastScreenPress == false and nowScreenPressed == true and touchx ~= 0 and touchy ~= 0 then
		app:touchDown(touchx, touchy)
	end
	
	if lastScreenPress == true and nowScreenPressed == false then
		app:touchUp(lasttx, lastty)
	end
	
	-- Flipping screen
	Screen.flip()
end