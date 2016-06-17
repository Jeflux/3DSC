local Button = {}

function Button:new(o)
	local o = o or {}
	setmetatable(o, Button)
	self.__index = Button
	
		o.pressed = false
		o.x = 0
		o.y = 0
		o.w = 32
		o.h = 32
		o.text = "button"
		
		o.normalColor = Color.new(255, 255, 255)
		o.pressedColor = Color.new(100, 100, 100)
		
		o.screen = BOTTOM_SCREEN
		
	return o
end

function Button:setColor(normal, pressed)
	self.normalColor = normal
	self.pressedColor = pressed
end

function Button:setPosition(x, y)
	self.x = x
	self.y = y
end

function Button:setText(t)
	self.text = t
end

function Button:setSize(w, h)
	self.w = w
	self.h = h
end

function Button:draw()
	if self.pressed then
		Screen.fillEmptyRect(self.x, self.x + self.w, self.y, self.y + self.h, self.pressedColor, self.screen)
	else
		Screen.fillEmptyRect(self.x, self.x + self.w, self.y, self.y + self.h, self.normalColor, self.screen)
	end
	
	Screen.debugPrint(self.x, self.y, self.text, Color.new(255, 255, 255), self.screen)
end

function Button:down(touchx, touchy)
	if touchx >= self.x and touchx <= self.x + self.w and
		touchy >= self.y and touchy <= self.y + self.h then
			self.pressed = true
		end
end

function Button:up(touchx, touchy)
	local ret = false
	
	if touchx >= self.x and touchx <= self.x + self.w and
		touchy >= self.y and touchy <= self.y + self.h then
			if self.pressed then
				ret = true
			end
		end
		
	self.pressed = false
		
	return ret
end

return Button