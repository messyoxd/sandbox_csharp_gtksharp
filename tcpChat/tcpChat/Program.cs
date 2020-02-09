using System;

namespace tcpChat
{
    class MainClass
    {

        public static TcpChatServer chat;

        protected static void InterruptHandler(object sender, ConsoleCancelEventArgs args)
        {
            if (chat != null)
            {
                chat.Shutdown();
            }
            args.Cancel = true;
        }

        public static void Main(string[] args)
        {
            int port = 6000;

            chat = new TcpChatServer(port);

            Console.CancelKeyPress += InterruptHandler;

            chat.Run();
        }
    }
}
