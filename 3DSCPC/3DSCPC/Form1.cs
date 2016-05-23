using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyInterfaceWrap;

namespace _3DSCPC
{
    public partial class Form1 : Form
    {
        public const int PORT = 25566;

        string statusMessage = "";

        string broadcastAddr = "";
        NetHelper netHelper = new NetHelper(PORT);
        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 1;
        int axisMax = 0;
        System.Threading.Thread listenerThread;

        public Form1() {
            InitializeComponent();

            joystick = new vJoy();
            iReport = new vJoy.JoystickState();
            long lAxisMax = 0;
            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref lAxisMax);
            axisMax = (int)lAxisMax;

            btnStopBroadcast.Enabled = false;

            string error = null;
            if ((error = vjoyhelper.setup(joystick, id)) != null) {
                int a = 0;
            }

            var addressList = System.Net.Dns.GetHostByName(Environment.MachineName).AddressList;
            for (int i = 0; i < addressList.Length; i++) {
                cbboxIP.Items.Add(addressList[i].ToString());
            }

            listenerThread = new System.Threading.Thread(() => {
                NetHelper.Message message;
                try {
                    while (true) {
                        switch ((message = netHelper.listen()).type) {
                            case NetHelper.Message.Type.CONNECT:
                                break;
                            case NetHelper.Message.Type.INPUT:
                                Keys.update(message.btn);

                                int angle = -1;
                                if (angle == -1 && Keys.isDown(Keys.DRIGHT)) {
                                    angle = 9000;
                                    if (Keys.isDown(Keys.DDOWN))
                                        angle += 4500;
                                    if (Keys.isDown(Keys.DUP))
                                        angle -= 4500;
                                }

                                if (angle == -1 && Keys.isDown(Keys.DLEFT)) {
                                    angle = 27000;
                                    if (Keys.isDown(Keys.DDOWN))
                                        angle -= 4500;
                                    if (Keys.isDown(Keys.DUP))
                                        angle += 4500;
                                }

                                if (angle == -1 && Keys.isDown(Keys.DUP)) angle = 0;
                                if (angle == -1 && Keys.isDown(Keys.DDOWN)) angle = 18000;

                                joystick.SetContPov(angle, id, 1);

                                joystick.SetBtn(Keys.isDown(Keys.A), id, 1);
                                joystick.SetBtn(Keys.isDown(Keys.B), id, 2);
                                joystick.SetBtn(Keys.isDown(Keys.X), id, 3);
                                joystick.SetBtn(Keys.isDown(Keys.Y), id, 4);
                                joystick.SetBtn(Keys.isDown(Keys.L), id, 5);
                                joystick.SetBtn(Keys.isDown(Keys.R), id, 6);
                                joystick.SetBtn(Keys.isDown(Keys.SELECT), id, 7);
                                joystick.SetBtn(Keys.isDown(Keys.START), id, 8);

                                float localX = ((message.pdx + 156) / 312.0f);
                                float localY = 1 - ((message.pdy + 156) / 312.0f);
                                joystick.SetAxis((int)(localX * axisMax), id, HID_USAGES.HID_USAGE_X);
                                joystick.SetAxis((int)(localY * axisMax), id, HID_USAGES.HID_USAGE_Y);

                                break;
                        };
                    }
                }
                catch (Exception e) { int a = 0; }
            });
            listenerThread.Start();
        }

        private void btnExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void broadcastTimer_Tick(object sender, EventArgs e) {
            netHelper.broadcast(broadcastAddr);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            netHelper.kill();
            listenerThread.Abort();
        }

        private void btnBroadcast_Click(object sender, EventArgs e) {
            if (cbboxIP.SelectedIndex < 0)
                MessageBox.Show("You must select an address to broadcast from");
            else {
                string addr = cbboxIP.Items[cbboxIP.SelectedIndex].ToString();
                broadcastAddr = IPAddressExtensions.GetBroadcastAddress(IPAddress.Parse(addr), IPAddressExtensions.GetSubnetMask(IPAddress.Parse(addr))).ToString();
                broadcastTimer.Start();

                cbboxIP.Enabled = false;
                btnBroadcast.Enabled = false;
                btnStopBroadcast.Enabled = true;
            }
        }

        private void btnStopBroadcast_Click(object sender, EventArgs e) {
            broadcastTimer.Stop();
            cbboxIP.Enabled = true;
            btnBroadcast.Enabled = true;
            btnStopBroadcast.Enabled = false;
        }
    }
}
