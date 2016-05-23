using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace _3DSCPC
{
    class JoystickHelper
    {
        private Keys keys;
        public vJoy joystick;
        public vJoy.JoystickState iReport;
        public uint id = 1;
        int axisMax = 0;
        
        public JoystickHelper() {
            keys = new Keys();
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            long lAxisMax = 0;
            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref lAxisMax);
            axisMax = (int)lAxisMax;

            string error = null;
            if ((error = vjoyhelper.setup(joystick, id)) != null) {
                throw new Exception("Cannot setup joystick");
            }
        }

        public void parseMessage(NetHelper.Message message) {
            keys.update(message.btn);

            int angle = -1;
            if (angle == -1 && keys.isDown(Keys.DRIGHT)) {
                angle = 9000;
                if (keys.isDown(Keys.DDOWN))
                    angle += 4500;
                if (keys.isDown(Keys.DUP))
                    angle -= 4500;
            }

            if (angle == -1 && keys.isDown(Keys.DLEFT)) {
                angle = 27000;
                if (keys.isDown(Keys.DDOWN))
                    angle -= 4500;
                if (keys.isDown(Keys.DUP))
                    angle += 4500;
            }

            if (angle == -1 && keys.isDown(Keys.DUP)) angle = 0;
            if (angle == -1 && keys.isDown(Keys.DDOWN)) angle = 18000;

            joystick.SetContPov(angle, id, 1);

            joystick.SetBtn(keys.isDown(Keys.A), id, 1);
            joystick.SetBtn(keys.isDown(Keys.B), id, 2);
            joystick.SetBtn(keys.isDown(Keys.X), id, 3);
            joystick.SetBtn(keys.isDown(Keys.Y), id, 4);
            joystick.SetBtn(keys.isDown(Keys.L), id, 5);
            joystick.SetBtn(keys.isDown(Keys.R), id, 6);
            joystick.SetBtn(keys.isDown(Keys.SELECT), id, 7);
            joystick.SetBtn(keys.isDown(Keys.START), id, 8);

            float localX = ((message.pdx + 156) / 312.0f);
            float localY = 1 - ((message.pdy + 156) / 312.0f);
            joystick.SetAxis((int)(localX * axisMax), id, HID_USAGES.HID_USAGE_X);
            joystick.SetAxis((int)(localY * axisMax), id, HID_USAGES.HID_USAGE_Y);
        }
    }
}
