using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPAsynchronousChatServer
{
    public class UDPAsynchronousChatClient
    {
        Socket mSockBroadcastSender;
        IPEndPoint mIpepBroadcast;
        IPEndPoint mIpepLocal;
        public UDPAsynchronousChatClient(int _localPort, int _remotePort)
        {
            mIpepBroadcast = new IPEndPoint(IPAddress.Broadcast, _remotePort);
            mIpepLocal = new IPEndPoint(IPAddress.Any, _localPort);

            mSockBroadcastSender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mSockBroadcastSender.EnableBroadcast = true;
        }

        public void SendBroadcast(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            try
            {
                if (!mSockBroadcastSender.IsBound)
                {
                    mSockBroadcastSender.Bind(mIpepLocal);
                }
                var dataBytes = Encoding.ASCII.GetBytes(str);

                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.SetBuffer(dataBytes, 0, dataBytes.Length);
                saea.RemoteEndPoint = mIpepBroadcast;

                saea.Completed += SendCompletedCallback;

                mSockBroadcastSender.SendToAsync(saea);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void SendCompletedCallback(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine($"Data sent succesfully to: {e.RemoteEndPoint}");
        }
    }
}
