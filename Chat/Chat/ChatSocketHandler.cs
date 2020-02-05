using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public class ChatSocketHandler
    {
        private Socket chatCom;
        IPEndPoint localEP;
        IPEndPoint remoteEP;
        string textReceived;
        public ChatSocketHandler(int localPort, string remoteAddress, int remotePort)
        {
            chatCom = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localEP = new IPEndPoint(IPAddress.Any, localPort);
            IPAddress remoteIP = IPAddress.Parse(remoteAddress);
            remoteEP = new IPEndPoint(remoteIP, remotePort);
            textReceived = string.Empty;
        }

        public void StartReceiving()
        {
            try
            {
                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.SetBuffer(new byte[1024], 0, 1024);
                saea.RemoteEndPoint = remoteEP;
                if (!chatCom.IsBound)
                {
                    chatCom.Bind(localEP);
                }
                saea.Completed += ReceiveCompleteCallback;
                chatCom.ReceiveFromAsync(saea);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void ReceiveCompleteCallback(object sender, SocketAsyncEventArgs e)
        {
            textReceived = Encoding.ASCII.GetString(e.Buffer, 0, e.BytesTransferred);
            Console.WriteLine(
                $"Text received: {textReceived}{Environment.NewLine}" +
                $"Number of bytes received: {e.BytesTransferred}{Environment.NewLine}" +
                $"Received data from: {e.RemoteEndPoint}{Environment.NewLine}"
                );
            StartReceiving();
            return;
        }

        public void SendMessage(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            var dataBuffer = Encoding.ASCII.GetBytes(str);
            saea.SetBuffer(dataBuffer, 0, dataBuffer.Length);
            saea.RemoteEndPoint = remoteEP;
            if (!chatCom.IsBound)
            {
                chatCom.Bind(localEP);
            }
            saea.Completed += SendMessageCompleteCallback;
            chatCom.SendToAsync(saea);
        }

        private void SendMessageCompleteCallback(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine($"Sent to Server:{e.RemoteEndPoint}");
            return;
        }
    }
}
