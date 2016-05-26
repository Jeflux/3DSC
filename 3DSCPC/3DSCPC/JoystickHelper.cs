using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

using System.Windows.Forms; // Screen (sorry!)
using System.Runtime.InteropServices; // DllImport-attribute

namespace _3DSCPC
{
    class JoystickHelper
    {
        List<int> availJoy = new List<int>();
        List<int> connectedJoy = new List<int>();

        private Keys keys;
        public vJoy joystick;
        public vJoy.JoystickState iReport;
        public uint id = 1;
        int axisMax = 0;
        public int numberOfDevices;

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        
        public JoystickHelper() {
            keys = new Keys();
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            long lAxisMax = 0;
            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref lAxisMax);
            axisMax = (int)lAxisMax;

            string error = null;

            int nDevices = 0;
            for (uint i = 1; i <= 16; i++) {
                if ((error = vjoyhelper.setup(joystick, i)) != null) {
                    // Assume no more devices are set up correctly
                    break;
                }
                nDevices++;
            }
            for (int i = 1; i <= nDevices; i++)
                availJoy.Add(i);
            numberOfDevices = nDevices;
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

            joystick.SetContPov(angle, message.ID, 1);

            joystick.SetBtn(keys.isDown(Keys.A), message.ID, 1);
            joystick.SetBtn(keys.isDown(Keys.B), message.ID, 2);
            joystick.SetBtn(keys.isDown(Keys.X), message.ID, 3);
            joystick.SetBtn(keys.isDown(Keys.Y), message.ID, 4);
            joystick.SetBtn(keys.isDown(Keys.L), message.ID, 5);
            joystick.SetBtn(keys.isDown(Keys.R), message.ID, 6);
            joystick.SetBtn(keys.isDown(Keys.SELECT), message.ID, 7);
            joystick.SetBtn(keys.isDown(Keys.START), message.ID, 8);

            float localX = ((message.pdx + 156) / 312.0f);
            float localY = 1 - ((message.pdy + 156) / 312.0f);
            joystick.SetAxis((int)(localX * axisMax), message.ID, HID_USAGES.HID_USAGE_X);
            joystick.SetAxis((int)(localY * axisMax), message.ID, HID_USAGES.HID_USAGE_Y);

            // Touch & Cursor interaction
            if (keys.isDown(Keys.TOUCH)) {
                int xx = message.touch_px;
                int yy = message.touch_py;

                float rx = xx / 320.0f;
                float ry = yy / 240.0f;

                int screen_w = Screen.PrimaryScreen.Bounds.Width;
                int screen_h = Screen.PrimaryScreen.Bounds.Width;

                int x = (int)(rx * screen_w);
                int y = (int)(ry * screen_h);

                SetCursorPos(x, y);
            }
        }

        public int connectJoystick() {
            if (availJoy.Count < 0)
                return 0;

            int i = availJoy[0];
            availJoy.RemoveAt(0);
            connectedJoy.Add(i);
            return i;
        }

        public void disconnectJoystick(int ID) {
            availJoy.Add(ID);
            connectedJoy.Remove(ID);
            availJoy.Sort();
        }

        public int[] getConnectedDevices() {
            return connectedJoy.ToArray();
        }
    }
}
