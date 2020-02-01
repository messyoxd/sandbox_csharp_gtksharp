using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPAsynchronousChatServer
{
    public class UDPAsynchronousChatServer
    {
        Socket mSockBroadcastReceiver;
        IPEndPoint mIpepLocal;
        private int retryCount;

        public UDPAsynchronousChatServer()
        {
            mSockBroadcastReceiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mIpepLocal = new IPEndPoint(IPAddress.Any, 23000);

            mSockBroadcastReceiver.EnableBroadcast = true;
        }

        public void StartReceivingData()
        {
            try
            {
                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.SetBuffer(new byte[1024], 0, 1024);
                saea.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                if(!mSockBroadcastReceiver.IsBound){
                    mSockBroadcastReceiver.Bind(mIpepLocal);
                }
                saea.Completed += ReceiveCompleteCallback;
                if (!mSockBroadcastReceiver.ReceiveFromAsync(saea))
                {
                    Console.WriteLine($"Failed to receive data - socket error: {saea.SocketError}");
                    if(retryCount++ >= 10)
                    {
                        return;
                    }
                    else
                    {
                        StartReceivingData();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void ReceiveCompleteCallback(Object sender, SocketAsyncEventArgs e)
        {
            string textReceived = Encoding.ASCII.GetString(e.Buffer, 0, e.BytesTransferred);
            Console.WriteLine(
                $"Text received: {textReceived}{Environment.NewLine}" +
            	$"Number of bytes received: {e.BytesTransferred}{Environment.NewLine}" +
            	$"Received data from: {e.RemoteEndPoint}{Environment.NewLine}"
                );
            StartReceivingData();
        }
    }
}
