using System;
using System.Net.Sockets;

namespace Client
{
    public interface IGame
    {
        #region Properties

        string Name { get; }

        int RequiredPlayers { get; }
        #endregion

        #region Funcions

        bool AddPlayer(TcpClient player);

        void DisconnectClient(TcpClient player);

        void Run();
        #endregion
    }
}
