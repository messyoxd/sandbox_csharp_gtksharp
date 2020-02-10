using System;

namespace Server
{
    class MainClass
    {
        public static TcpGamesServer gamesServer;

        // For when the user Presses Ctrl-C, this will gracefully shutdown the server
        public static void InterruptHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            gamesServer?.Shutdown();
        }
        public static void Main(string[] args)
        {
            // Some arguments
            string name = "Bad BBS";//args[0];
            int port = 6000;//int.Parse(args[1]);

            // Handler for Ctrl-C presses
            Console.CancelKeyPress += InterruptHandler;

            // Create and run the server
            gamesServer = new TcpGamesServer(name, port);
            gamesServer.Run();
        }
    }
}
