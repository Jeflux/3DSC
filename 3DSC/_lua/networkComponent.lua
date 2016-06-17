local NetworkComponent = {}

function NetworkComponent:start()
	Socket.init()

	self.connected = false	
	self.socket = UDP.createSocket(PORT)
	self.message = ""
	self.hostAddr = ""
	self.playerID = 0
	self.serverSilentCount = 0
	
	self.sendButtons = true
	self.sendPads = true
	self.sendTouch = true
end

function NetworkComponent:isConnected()
	return self.connected
end

function NetworkComponent:setSend(buttons, pads, touch)
	self.sendButtons = buttons
	self.sendPads = pads
	self.sendTouch = touch
end

function NetworkComponent:tick()
	-- Listen for broadcast if not connected
	if self.connected == false then	
		local message = ""
		message, self.hostAddr = UDP.receive(self.socket)
		if #message > 0 and message == tostring(PORT) then
			self.connected = true
			UDP.connect(self.socket, self.hostAddr)
		end

		return
	end
	
	local padx, pady, touchx, touchy, buttons
	if self.sendPads then
		padx, pady = Controls.readCirclePad()
	else
		padx = 0
		pady = 0
	end
	
	touchx, touchy = Controls.readTouch()
	buttons = Controls.read()
	
	if not self.sendTouch then
		buttons = buttons & 0xFF0FFFFF
	end
	if not self.sendButtons then
		buttons = buttons & 0x00F00000
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
	end
end

function NetworkComponent:dispose()
		if connected == true then
			Socket.close(self.socket)
		end
		Socket.term()
end

return NetworkComponent