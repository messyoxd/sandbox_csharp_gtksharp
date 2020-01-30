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
            IPEndPoint ipepSender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)ipepSender;
            byte[] receiverBuffer = new byte[512];
            string strBuffer = "";
            int nCountedBytes=0;
            try
            {
                sockBroadcastReceiver.Bind(ipepLocal);
                while (true)
                {
                    nCountedBytes = sockBroadcastReceiver.ReceiveFrom(receiverBuffer, ref epSender);
                    strBuffer = Encoding.ASCII.GetString(receiverBuffer,0, nCountedBytes);
                    Console.WriteLine(strBuffer);
                    Console.WriteLine("Received from: "+epSender.ToString());
                    if (strBuffer.Equals("echo")){
                        sockBroadcastReceiver.SendTo(receiverBuffer, 0, nCountedBytes, SocketFlags.None, epSender);
                        Console.WriteLine("Text echoed back...");
                    }
                    Array.Clear(receiverBuffer, 0, receiverBuffer.Length);
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
