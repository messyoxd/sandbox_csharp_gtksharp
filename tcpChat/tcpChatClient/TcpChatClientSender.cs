using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace tcpChatClient
{
    public class TcpChatClientSender
    {
        public readonly string serverAddress;
        public readonly int port;
        private TcpClient _client;
        private bool _running;
        public readonly int BufferSize = 2 * 1024;
        private NetworkStream _msgStream = null;
        private bool _disconnectRequested = false;
        public TcpChatClientSender(string ServerAddress, int Port)
        {
            _client = new TcpClient();
            _client.SendBufferSize = BufferSize;
            _client.ReceiveBufferSize = BufferSize;
            _running = false;

            serverAddress = ServerAddress;
            port = Port;
        }

        public void Connect()
        {
            _client.Connect(serverAddress, port);
            EndPoint endPoint = _client.Client.RemoteEndPoint;

            if (_client.Connected)
            {
                Console.WriteLine($"Conectado com servidor em {endPoint}");
                _msgStream = _client.GetStream();
                byte[] msgBuf = Encoding.ASCII.GetBytes("<DISCOVER>");
                _msgStream.Write(msgBuf, 0, msgBuf.Length);

                _running = true;

            }
            else
            {
                _cleanupNetworkResources();
                Console.WriteLine($"Não foi possivel se conectar com servidor em {endPoint}");
            }
            return;
        }

        public void SendMessages()
        {
            bool wasRunning = _running;
            while (_running)
            {
                Console.Write("Mensage: ");
                string message = Console.ReadLine();
                if (message.ToLower().Equals("exit"))
                {
                    Console.WriteLine("Desconectando...");
                    _running = false;
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    byte[] msgBuf = Encoding.ASCII.GetBytes(message);
                    _msgStream.Write(msgBuf, 0, msgBuf.Length);
                }

                Thread.Sleep(10);

                if (_isDisconnected(_client))
                {
                    _running = false;
                    Console.WriteLine("O servidor se desconectou de nós ;-;");
                }
            }
            _cleanupNetworkResources();
            if (wasRunning)
                Console.WriteLine("Desconectado");
            return;
        }

        private bool _isDisconnected(TcpClient client)
        {
            try
            {
                Socket s = client.Client;
                // checa se o socket é readable e se ha alguma mensagem pendente
                return s.Poll(10 * 1000, SelectMode.SelectRead) && (s.Available == 0);
            }
            catch (SocketException se)
            {
                // We got a socket error, assume it's disconnected
                return true;
            }
        }

        private void _cleanupNetworkResources()
        {
            _msgStream?.Close();
            _msgStream = null;
            _client.Close();
            return;
        }

        public void Disconnect()
        {
            _running = false;
            _disconnectRequested = true;
            return;
        }

        public void ListenForMessages()
        {
            bool wasRunning = _running;
            while(_running)
            {
                int messageLength = _client.Available;
                if(messageLength > 0)
                {
                    byte[] msgBuf = new byte[BufferSize];
                    _msgStream.Read(msgBuf, 0, messageLength);
                    string str = Encoding.ASCII.GetString(msgBuf);
                    Console.WriteLine(str);
                }
                Thread.Sleep(10);
                if (_isDisconnected(_client))
                {
                    _running = false;
                    Console.WriteLine("O servidor se desconectou de nós ;-;");
                }
                _running &= !_disconnectRequested;
            }
            _cleanupNetworkResources();
            if (wasRunning)
                Console.WriteLine("Desconectado.");
            return;
        }
    }
}
