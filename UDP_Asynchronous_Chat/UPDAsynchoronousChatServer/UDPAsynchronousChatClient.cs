using System;
using System.Diagnostics;
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
        private EndPoint mChatServerEP;

        public object Enconding { get; private set; }

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
            if (Encoding.ASCII.GetString(e.Buffer).Equals("<DISCOVER>"))
            {
                ReceiveTextFromServer(expectedValue: "<CONFIRMED>", IpepReceiverLocal: mIpepLocal);
            }
        }

        private void ReceiveTextFromServer(string expectedValue, IPEndPoint IpepReceiverLocal)
        {
            if(IpepReceiverLocal == null)
            {
                Console.WriteLine("No IPEndPoint specified");
                return;
            }
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            saea.SetBuffer(new byte[1024], 0, 1024);
            saea.RemoteEndPoint = IpepReceiverLocal;
            saea.UserToken = expectedValue;
            saea.Completed += ReceiveConfirmationCompleted;
            mSockBroadcastSender.ReceiveFromAsync(saea);
        }

        private void ReceiveConfirmationCompleted(object sender, SocketAsyncEventArgs e)
        {
            if(e.BytesTransferred == 0)
            {
                Debug.WriteLine($"Zero bytes received from: {e.RemoteEndPoint}");
            }
            var receivedText = Encoding.ASCII.GetString(e.Buffer, 0, e.BytesTransferred);

            if (receivedText.Equals(e.UserToken))
            {
                Console.WriteLine($"Received confirmation from server. {e.RemoteEndPoint}");

                mChatServerEP = e.RemoteEndPoint;

                ReceiveTextFromServer(string.Empty, mChatServerEP as IPEndPoint);
            }
            else
            {
                Console.WriteLine("Expected text not received");
            }
        }

        public void SendToKnownServer(string str)
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
                saea.RemoteEndPoint = mChatServerEP;
                saea.UserToken = str;
                saea.Completed += SendToKnownServerCallback;

                mSockBroadcastSender.SendToAsync(saea);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void SendToKnownServerCallback(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine($"Sent: {e.UserToken}{Environment.NewLine}Server:{e.RemoteEndPoint}");
        }
    }
}
