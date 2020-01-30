using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace TesteBroadcastUPDReceiver
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Socket sockBroadcastReceiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipepLocal = new IPEndPoint(IPAddress.Any, 23000);

            byte[] receiverBuffer = new byte[512];
            string strBuffer = "";
            int nCountedBytes=0;
            try
            {
                sockBroadcastReceiver.Bind(ipepLocal);
                while (true)
                {
                    nCountedBytes = sockBroadcastReceiver.Receive(receiverBuffer);
                    strBuffer = Encoding.ASCII.GetString(receiverBuffer,0, nCountedBytes);
                    Console.WriteLine(strBuffer);
                    Array.Clear(receiverBuffer, 0, receiverBuffer.Length);
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
