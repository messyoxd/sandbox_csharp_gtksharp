using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public class ChatSocketHandler
    {
        private Socket chatCom;
        public IPEndPoint localEP;
        public IPEndPoint remoteEP;
        public string textReceived;
        int retryCount;
        private Action<string> updateTextList;

        public ChatSocketHandler(string localAddress,
            int localPort,
            string remoteAddress,
            int remotePort,
            Action<string> UpdateTextList
            )
        {
            chatCom = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localEP = new IPEndPoint(IPAddress.Parse(localAddress), localPort);
            remoteEP = new IPEndPoint(IPAddress.Parse(remoteAddress), remotePort);
            textReceived = string.Empty;
            updateTextList = UpdateTextList;
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
                if (!chatCom.ReceiveFromAsync(saea))
                {
                    Console.WriteLine($"Failed to receive data - socket error: {saea.SocketError}");
                    if (retryCount++ >= 10)
                    {
                        return;
                    }
                    StartReceiving();
                }
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
            if (textReceived.Equals("<DISCOVER>"))
            {
                Console.WriteLine($"received a <DISCOVER> from: {e.RemoteEndPoint}");
                SendMessage("<CONFIRMED>");
            }
            else if (textReceived.Equals("<CONFIRMED>"))
            {
                Console.WriteLine($"received <CONFIRMED> from: {e.RemoteEndPoint}");
            }
            else
            {
                updateTextList(textReceived);
                StartReceiving();
                return;
            }
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

        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "Não foi possivel encontrar um adaptador com ipv4!";
        }
    }
}
