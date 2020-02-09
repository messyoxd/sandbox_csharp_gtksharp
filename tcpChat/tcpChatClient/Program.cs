using System;
using System.Threading;

namespace tcpChatClient
{
    class MainClass
    {
        public static TcpChatClientSender messager;
        protected static void InterruptHandler(object sender, ConsoleCancelEventArgs args)
        {
            messager.Disconnect();
            args.Cancel = true;
        }
        public static void Main(string[] args)
        {
            string host = "localhost";
            int port = 6000;
            TcpChatClientSender messager = new TcpChatClientSender(host, port);
            Console.CancelKeyPress += InterruptHandler;
            messager.Connect();
            Thread sendMessagesThread = new Thread(new ThreadStart(messager.SendMessages));
            Thread receiveMessagesThread = new Thread(new ThreadStart(messager.ListenForMessages));
            sendMessagesThread.Start();
            receiveMessagesThread.Start();
            // messager.SendMessages();
        }
    }
}
