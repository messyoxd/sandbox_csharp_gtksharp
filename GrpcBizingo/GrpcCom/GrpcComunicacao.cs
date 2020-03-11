using System;
using Grpc.Core;

namespace GrpcCom
{
    public class GrpcComunicacao
    {
        private int _portaLocal;
        private int _portaRemota;
        private string _ipRemoto;
        private Server _server;
        private GrpcBizingoClient.GrpcBizingoClientImpl _client;

        Action<bool> _resetRequest;
        Action<string> _appendMessage;
        Action<bool> _setGameOver;
        Action<int, int> _moveAsPecas;

        public GrpcComunicacao(
            Action<bool> resetRequest,
            Action<string> appendMessage,
            Action<bool> setGameOver,
            Action<int, int> moveAsPecas,

            int portaLocal=50052, 
            int portaRemota=50051, 
            string ipRemoto="127.0.0.1"
            )
        {
            _portaLocal = portaLocal;
            _portaRemota = portaRemota;
            _ipRemoto = ipRemoto;

            // janela de dialog que retorna true ou false
            _resetRequest = resetRequest;
            // colocar mensagem no chat
            _appendMessage = appendMessage;
            // controla variavel que inicia ou termina o jogo
            _setGameOver = setGameOver;
            // move as pecas do tabuleiro
            _moveAsPecas = moveAsPecas;
        }



        public void IniciarServidor()
        {
            _server = new Server
            {
                Services = { BizingoRpc.BindService(new GrpcBizingoServer.GrpcBizingoServerImpl(
                    _resetRequest,
                    _appendMessage,
                    _setGameOver,
                    _moveAsPecas
                    )) },
                Ports = { new ServerPort("localhost", _portaLocal, ServerCredentials.Insecure) }
            };
            _server.Start();
        }

        public void AguardarConexaoDoAdversario()
        {
            //
        }

        public void ConectarComAdversario()
        {
            var channel = new Channel($"{_portaRemota}:{_portaRemota}", ChannelCredentials.Insecure);
            _client = new GrpcBizingoClient.GrpcBizingoClientImpl(new BizingoRpc.BizingoRpcClient(channel));
            _client.SendMessage("84eb93488832f124221ab2c1bd802d47");
        }
    }
}
