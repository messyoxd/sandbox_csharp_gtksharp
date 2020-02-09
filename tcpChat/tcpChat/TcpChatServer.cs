using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace tcpChat
{
    public class TcpChatServer
    {

        private TcpListener _listener;
        // mensagens que precisam ser enviadas
        private Queue<string> _message;
        // porta
        public readonly int porta;
        // variavel que controla ate quando o servidor deve funcionar
        private bool _running;

        public readonly int BufferSize = 2 * 1024;

        private TcpClient amiguinho;

        public TcpChatServer(int port)
        {
            _message = new Queue<string>();
            porta = port;

            _listener = new TcpListener(IPAddress.Any, porta);

            _running = false;
        }

        public void Shutdown()
        {
            _running = false;
            Console.WriteLine("O servidor fechou!");
            return;
        }

        public void Run()
        {
            Console.WriteLine($"Iniciando o servidor na porta {porta}");

            _listener.Start();
            _running = true;

            while (_running)
            {
                if (_listener.Pending())
                    _handleNewConnection();
                // Do the rest
                _checkForDisconnects();
                _checkForNewMessages();
                _sendMessages();

                // Use less CPU
                Thread.Sleep(10);
            }

            _listener.Stop();
            Console.WriteLine("O servidor está desligando!");
        }

        private void _checkForDisconnects()
        {
            if (_isDisconnected(amiguinho))
            {
                Console.WriteLine($"Amiguinho em {amiguinho.Client.RemoteEndPoint} desconectou ;-;");
                _cleanupClient(amiguinho);
                amiguinho = null;
            }
            return;
        }

        private void _checkForNewMessages()
        {
            int messageLength = amiguinho.Available;
            if (messageLength > 0)
            {
                // ha mensagens
                byte[] msgBuf = new byte[messageLength];
                amiguinho.GetStream().Read(msgBuf, 0, messageLength);

                string str = Encoding.ASCII.GetString(msgBuf);
                _message.Enqueue($"{amiguinho.Client.RemoteEndPoint}: {str}");
            }
            return;
        }

        private void _sendMessages()
        {
            foreach(string str in _message)
            {
                byte[] msgBuf = Encoding.ASCII.GetBytes(str);

                amiguinho.GetStream().Write(msgBuf, 0, msgBuf.Length);
            }
            _message.Clear();
            return;
        }

        private void _handleNewConnection()
        {
            TcpClient newClient = _listener.AcceptTcpClient();
            NetworkStream netStream = newClient.GetStream();
            netStream.ReadTimeout=10000;
            netStream.WriteTimeout = 10000;
            //newClient.SendBufferSize = BufferSize;
            //newClient.ReceiveBufferSize = BufferSize;

            EndPoint endPoint = newClient.Client.RemoteEndPoint;
            Console.WriteLine($"Cuidadando da conexão com cliente em {endPoint}");

            byte[] msgBuf = new byte[BufferSize];
            int bytesRead = netStream.Read(msgBuf, 0, msgBuf.Length);
            if (bytesRead > 0)
            {
                string str = Encoding.ASCII.GetString(msgBuf, 0, bytesRead);
                if (str.Equals("<DISCOVER>"))
                {
                    amiguinho = newClient;
                    str = "<CONFIRMED>";
                    msgBuf = Encoding.ASCII.GetBytes(str);
                    netStream.Write(msgBuf, 0, msgBuf.Length);
                }
                else if (!str.Equals("<DISCOVER>") || !str.Equals("<CONFIRMED>"))
                {
                    Console.WriteLine($"Mensagem recebida: {str}{Environment.NewLine}Enviada de:{endPoint}");
                }
                else
                {
                    Console.WriteLine("Não intendi o q ele falou");
                }
            }

            return;
        }

        // Checks if a socket has disconnected
        // Adapted from -- http://stackoverflow.com/questions/722240/instantly-detect-client-disconnection-from-server-socket
        private static bool _isDisconnected(TcpClient client)
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

        public void _cleanupClient(TcpClient client)
        {
            client.GetStream().Close();
            client.Close();
            return;
        }
    }
}
