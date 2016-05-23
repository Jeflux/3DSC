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

        bool firstMinimize = true;

        JoystickHelper joystickHelper;
        string broadcastAddr = "";
        NetHelper netHelper = new NetHelper(PORT);
        
        System.Threading.Thread listenerThread;

        public Form1() {
            InitializeComponent();
            
            joystickHelper = new JoystickHelper();

            btnStopBroadcast.Enabled = false;

            IPAddress dhcpIP = netHelper.getDHCPAddr();
            var addressList = Dns.GetHostByName(Environment.MachineName).AddressList;
            foreach (var a in addressList) {
                if (IPAddressExtensions.IsInSameSubnet(dhcpIP, a, IPAddressExtensions.GetSubnetMask(a))) {
                    cbboxIP.Items.Add(a.ToString());
                    cbboxIP.SelectedIndex = 0;
                    cbboxIP.Refresh();
                }
            }

            listenerThread = new System.Threading.Thread(() => {
                NetHelper.Message message;
                try {
                    while (true) {
                        switch ((message = netHelper.listen()).type) {
                            case NetHelper.Message.Type.CONNECT:
                                break;
                            case NetHelper.Message.Type.INPUT:
                                joystickHelper.parseMessage(message);
                                break;
                        };
                    }
                }
                catch (Exception e) {
                    if (!(e is ThreadAbortException))
                        throw new Exception(e.Message);
                }
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

        private void Form1_Resize(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized && ckMinimizeToTray.CheckState == CheckState.Checked) {
                notifMinimize.Visible = true;
                //if (firstMinimize) {
                //    firstMinimize = false;
                //    notifMinimize.BalloonTipIcon = ToolTipIcon.Info;
                //    notifMinimize.BalloonTipText = "3DSC is still running";
                //    notifMinimize.ShowBalloonTip(500);
                //}
                ShowInTaskbar = false;
                Hide();
            }
        }

        private void notifMinimize_DoubleClick(object sender, EventArgs e) {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifMinimize.Visible = false;
        }
    }
}
