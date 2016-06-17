local Application = {}

function Application:start()
	self.mouseButton = require("button"):new()
	self.mouseButton:setPosition(0, 0)
	self.mouseButton:setSize(70, 32)
	self.mouseButton:setText("Mouse")
	self.mouseMode = false
	
	self.backlightOn = true
	
	self.exitButton = require("button"):new()
	self.exitButton:setPosition(0, 240-33)
	self.exitButton:setSize(70, 32)
	self.exitButton:setText("Exit")
	
	self.network = require("networkcomponent")
	self.network:start()
	self.network:setSend(true, true, false)

	self.wasConnected = false
end

function Application:tick()
	local pad = Controls.read()
	
	self.network:tick()
	
	if self.network:isConnected() then
		Screen.debugPrint(0, 0, "Connected to " .. self.network.hostAddr, Color.new(255, 255, 255), TOP_SCREEN)
		Screen.debugPrint(0, 20, "Player: " .. self.network.playerID, Color.new(255, 255, 255), TOP_SCREEN)
	else
		Screen.debugPrint(0, 0, "Listening for broadcast on port " .. tostring(PORT), Color.new(255, 255, 255), TOP_SCREEN)
	end
	
	Screen.debugPrint(0, 220, "Touch screen to toggle back light", Color.new(255, 255, 255), TOP_SCREEN)
	
	if self.network:isConnected() and not self.wasConnected then
		self.backlightOn = false
		self.wasConnected = true
		Controls.disableScreen(TOP_SCREEN)
		Controls.disableScreen(BOTTOM_SCREEN)
	end
	
	self.mouseButton:draw()
	if not self.mouseMode then
		self.exitButton:draw()	
	end
end

function Application:touchDown(touchx, touchy)
	self.mouseButton:down(touchx, touchy)
	self.exitButton:down(touchx, touchy)
end

function Application:touchUp(touchx, touchy)
	if self.mouseButton:up(touchx, touchy) then
		self.network.sendTouch = not self.mouseMode
		self.mouseMode = not self.mouseMode
	
		if self.mouseMode then
			self.mouseButton:setText("<-")
			self.mouseButton:setSize(20, 20)
		else
			self.mouseButton:setText("Mouse")
			self.mouseButton:setSize(70, 32)
		end
		
		return
	end
	
	if self.exitButton:up(touchx, touchy) then
		self.network:dispose()
		System.exit()
		return
	end
	
	if not self.mouseMode then
		if self.backlightOn then 
			Controls.disableScreen(TOP_SCREEN)
			Controls.disableScreen(BOTTOM_SCREEN)
		else
			Controls.enableScreen(TOP_SCREEN)
			Controls.enableScreen(BOTTOM_SCREEN)
		end
		self.backlightOn = not self.backlightOn
	end
end

return Application