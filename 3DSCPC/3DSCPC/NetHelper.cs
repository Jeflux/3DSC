using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace _3DSCPC
{
    class NetHelper
    {
        UdpClient listener;
        IPEndPoint groupEP;
        Socket socket;
        String connectionMessage = "";

        IPEndPoint remoteIpEndPoint;

        int port;

        public NetHelper(int port) {
            this.port = port;
            listener = new UdpClient(port);
            groupEP = new IPEndPoint(IPAddress.Any, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            connectionMessage = port.ToString();
        }


        public class Message {
            public enum Type {
                CONNECT,
                INPUT,
                UNDEF
            };

            public Type type;
            public int pdx;
            public int pdy;
            public UInt32 btn;
        }

        public void broadcast(string addr) {
            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(addr), port);
            byte[] bytes = Encoding.ASCII.GetBytes(connectionMessage);
            client.Send(bytes, bytes.Length, ip);
            client.Close();

            //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //s.EnableBroadcast = true;
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.255"), port);
            //byte[] m = Encoding.ASCII.GetBytes(connectionMessage);
            //s.SendTo(m, endPoint);
        }
        

        public Message listen() {
            Message ret = new Message();
            string returnData = port.ToString();
            string retData = "";

            Byte[] receiveBytes = { };
            try {
                // Wait for packet of right length
                while (receiveBytes.Length != 8) {
                    receiveBytes = listener.Receive(ref remoteIpEndPoint);
                    returnData = Encoding.ASCII.GetString(receiveBytes);

                    // Only deserialize if correct packet
                    if (receiveBytes.Length != 8)
                        continue;

                    retData = "";
                    Byte[] pdxb = { receiveBytes[0], receiveBytes[1] };
                    int pdx = BitConverter.ToInt16(receiveBytes, 0);

                    Byte[] pdyb = { receiveBytes[2], receiveBytes[3] };
                    int pdy = BitConverter.ToInt16(receiveBytes, 2);

                    Byte[] btnb = { receiveBytes[4], receiveBytes[5], receiveBytes[6], receiveBytes[7] };
                    UInt32 btn = BitConverter.ToUInt32(receiveBytes, 4);

                    retData = pdx.ToString() + "    " + pdy.ToString() + "    " + btn;

                    ret.type = Message.Type.INPUT;
                    ret.pdx = pdx;
                    ret.pdy = pdy;
                    ret.btn = btn;
                }
            }
            catch (Exception e){ }

            return ret;
        }

        public void kill() {
            socket.Close();
            listener.Close();
        }

        public IPAddress getDHCPAddr() {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters) {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                IPAddressCollection addresses = adapterProperties.DhcpServerAddresses;
                if (addresses.Count > 0) {
                    Console.WriteLine(adapter.Description);
                    foreach (IPAddress address in addresses) {
                        return address;
                    }
                }
            }
            return null;
        }
    }
}
