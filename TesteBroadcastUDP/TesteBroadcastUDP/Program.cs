using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace TesteBroadcastUDP
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // this is the sender
            Socket sockBroadcaster = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sockBroadcaster.EnableBroadcast = true;
            IPEndPoint broadcastIP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 23000);
            byte[] broadcastBuffer = new byte[] { 0x0D, 0x0A };
            string strBuffer = "";
            try
            {
                while (true)
                {
                    Console.WriteLine("Escreva alguma coisa. Digite 'arreguei' para fechar o programa.");
                    strBuffer = Console.ReadLine();
                    if (strBuffer.Equals("arreguei"))
                    {
                        break;
                    }

                    broadcastBuffer = Encoding.ASCII.GetBytes(strBuffer);

                    sockBroadcaster.SendTo(broadcastBuffer, broadcastIP);

                }
                sockBroadcaster.Shutdown(SocketShutdown.Both);
                sockBroadcaster.Close();
            }catch(Exception e)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
