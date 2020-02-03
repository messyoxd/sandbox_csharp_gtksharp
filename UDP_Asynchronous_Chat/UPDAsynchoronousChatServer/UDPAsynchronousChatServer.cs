using System;
using System.Collections.Generic;
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
        List<EndPoint> mListOfClients;

        public UDPAsynchronousChatServer()
        {
            mSockBroadcastReceiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mIpepLocal = new IPEndPoint(IPAddress.Any, 23000);

            mSockBroadcastReceiver.EnableBroadcast = true;

            mListOfClients = new List<EndPoint>();
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
            if(textReceived == "<DISCOVER>")
            {
                if (!mListOfClients.Contains(e.RemoteEndPoint))
                {
                    mListOfClients.Add(e.RemoteEndPoint);
                    Console.WriteLine("Total clients: " + mListOfClients.Count);
                }
                SendTextToEndPoint("<CONFIRMED>", e.RemoteEndPoint);
            }
            StartReceivingData();
        }

        private void SendTextToEndPoint(string textToSend, EndPoint remoteEndPoint)
        {
            if(string.IsNullOrEmpty(textToSend) || remoteEndPoint == null)
            {
                return;
            }
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            var dataBuffer = Encoding.ASCII.GetBytes(textToSend);
            saea.SetBuffer(dataBuffer, 0, dataBuffer.Length);
            saea.RemoteEndPoint = remoteEndPoint;
            saea.Completed += SendCompletedCallback;
            mSockBroadcastReceiver.SendToAsync(saea);
        }

        private void SendCompletedCallback(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine($"Completed sending text to: {e.RemoteEndPoint}");
        }
    }
}
