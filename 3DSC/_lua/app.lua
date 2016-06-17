local Application = {}

function Application:start()
	Socket.init()

	self.connected = false	
	self.socket = UDP.createSocket(PORT)
	self.message = ""
	self.hostAddr = ""
	self.playerID = 0
	self.serverSilentCount = 0
end

function Application:tick()
	local pad = Controls.read()
	if Controls.check(pad, KEY_SELECT) and Controls.check(pad, KEY_START) then
		if connected == true then
			Socket.close(self.socket)
		end
		Socket.term()
		System.exit()
	end
	
	Screen.debugPrint(0, 220, "Press start and select to exit", Color.new(255, 255, 255), TOP_SCREEN)
	
	-- Listen for broadcast if not connected
	if self.connected == false then
		Screen.debugPrint(0, 0, "Listening for broadcast on port " .. tostring(PORT), Color.new(255, 255, 255), TOP_SCREEN)
	
		local message = ""
		message, self.hostAddr = UDP.receive(self.socket)
		if #message > 0 and message == tostring(PORT) then
			self.connected = true
			UDP.connect(self.socket, self.hostAddr)
			
			Controls.disableScreen(TOP_SCREEN)
			Controls.disableScreen(BOTTOM_SCREEN)
		end
		
		return
	end
	
	Screen.debugPrint(0, 0, "Connected to " .. self.hostAddr .. ":" .. tostring(PORT), Color.new(255, 255, 255), TOP_SCREEN)
	
	local padx, pady, touchx, touchy, buttons
	padx, pady = Controls.readCirclePad()
	touchx, touchy = Controls.readTouch()
	buttons = pad
	
	if self.playerID ~= nil then
		Screen.debugPrint(0, 200, "PlayerID = " .. self.playerID, Color.new(255, 255, 255), TOP_SCREEN)
	end
	
	local count = 0
	local IDMessage = nil
	local loop = true
	local heardFromServer = false
	while loop do
		count = count + 1
		UDP.sendTable(self.socket, {self.playerID or 0, padx, pady, buttons, touchx, touchy})
		IDMessage = UDP.receive(self.socket)
		
		if #IDMessage > 0 then
			heardFromServer = true
		end
		
		if count > 10 then 
			break
		end
		if IDMessage ~= nil and #IDMessage > 0 and IDMessage ~= tostring(PORT) then 
			loop = false 
			self.playerID = tonumber(IDMessage)
		end
	end
	
	if heardFromServer == true then
		self.serverSilentCount = 0
	else
		self.serverSilentCount = self.serverSilentCount + 1
	end
	
	if self.serverSilentCount > 5 then
		self.connected = false
		
		Controls.enableScreen(TOP_SCREEN)
		Controls.enableScreen(BOTTOM_SCREEN)
	end
end

return Application