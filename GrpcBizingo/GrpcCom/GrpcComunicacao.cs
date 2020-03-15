using System;
using System.Threading;
using Grpc.Core;

namespace GrpcCom
{
    public class GrpcComunicacao
    {
        private int _portaLocal;
        private int _portaRemota;
        private string _ipRemoto;
        private string _ipLocal;
        private string _meuNome;
        private Server _server;
        private GrpcBizingoClient.GrpcBizingoClientImpl _client;

        Action<string> _resetRequest;
        Action _resetJogo;
        Action<string> _appendMessage;
        Action<bool> _setGameOver;
        Action<int, int> _moveAsPecas;

        public GrpcComunicacao(
            Action<string> resetRequest,
            Action resetJogo,
            Action<string> appendMessage,
            Action<bool> setGameOver,
            Action<int, int> moveAsPecas,

            string meuNome,
            int portaLocal = 50052,
            int portaRemota = 50051,
            string ipRemoto = "127.0.0.1",
            string ipLocal = "localhost"
            )
        {
            _portaLocal = portaLocal;
            _portaRemota = portaRemota;
            _ipRemoto = ipRemoto;
            _ipLocal = ipLocal;
            _meuNome = meuNome;
            // janela de dialog que retorna true ou false
            _resetRequest = resetRequest;
            _resetJogo = resetJogo;
            // colocar mensagem no chat
            _appendMessage = appendMessage;
            // controla variavel que inicia ou termina o jogo
            _setGameOver = setGameOver;
            // move as pecas do tabuleiro
            _moveAsPecas = moveAsPecas;
        }

        /*
        *   Essa funcao sera chamada quando o jogador iniciar
        *   o servidor e quiser aguardar alguem se conectar com ele,
        *   bastando instanciar o servidor e esperar pela conexao.
        */
        public void iniciarConexao()
        {
            Thread servidor = new Thread(() => IniciarServidor());
            servidor.Start();
        }
        private void IniciarServidor()
        {
            try
            {
                _server = new Server
                {

                    Services = { BizingoRpc.BindService(new GrpcBizingoServer.GrpcBizingoServerImpl(
                    _resetRequest,
                    _resetJogo,
                    _appendMessage,
                    _setGameOver,
                    _moveAsPecas,
                    ConectarDevolta,
                    _meuNome,
                    _portaLocal,
                    _portaRemota,
                    _ipRemoto
                    )) },
                    Ports = { new ServerPort(_ipLocal, _portaLocal, ServerCredentials.Insecure) }
                    //Ports = { new ServerPort("localhost", 50052, ServerCredentials.Insecure) }
                };
                _server.Start();
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void ConectarDevolta(
            string ipLocal,
            string ipRemoto,
            int portaLocal,
            int portaRemota,
            string meuNome)
        {
            //Thread.Sleep(1000);
            try
            {
                // setando essas variaveis, pois o jogador que inicia a partida
                // não às tem antes do jogador adversario se conectar
                _ipRemoto = ipRemoto;
                _portaRemota = portaRemota;

                // criar canal de conexao com o outro jogador
                var channel = new Channel($"{_ipRemoto}:{_portaRemota}", ChannelCredentials.Insecure);
                // conectar com o outro jogador
                _client = new GrpcBizingoClient.GrpcBizingoClientImpl(new BizingoRpc.BizingoRpcClient(channel));
                // chamada rpc para testar a conexao
                MensagemConexao resposta = _client.Conectar(ipLocal, ipRemoto, portaLocal, portaRemota, meuNome);
                // coloca mensagem no chat para avisar que a conexao foi sucedida
                _appendMessage($"{resposta.MeuNome} conectou-se de {resposta.IpLocal}:{resposta.PortaLocal}!");
                // comeca o jogo
                _setGameOver(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /*
        *   Essa funcao sera usada quando algum jogador tentar se conectar
        *   com outro jogador. Ela tambem inicia o processo de conexao do jogador
        *   que iniciou a partida com o jogador que entrou depois
        */
        public void ConectarComAdversario()
        {
            try
            {
                // criar canal de conexao com o outro jogador
                var channel = new Channel($"{_ipRemoto}:{_portaRemota}", ChannelCredentials.Insecure);
                // conectar com o outro jogador
                _client = new GrpcBizingoClient.GrpcBizingoClientImpl(new BizingoRpc.BizingoRpcClient(channel));
                // chamada rpc para testar a conexao
                MensagemConexao resposta = _client.Conectar(_ipLocal, _ipRemoto, _portaLocal, _portaRemota, _meuNome);
                // comecar o proprio servidor para que o outro jogador tambem se conecte
                Thread t = new Thread(() => IniciarServidor());
                t.Start();
                //fazer chamada rpc que faz com que o outro jogador se conecte
                Console.WriteLine("Fazendo o adversario se conectar devolta");
                Flag f = _client.ConectarDevolta(resposta.IpLocal, resposta.IpRemoto, resposta.PortaLocal, resposta.PortaRemota, resposta.MeuNome);
                // mensagem de quando um player se conecta com outro
                _appendMessage($"conectou-se com jogador {resposta.MeuNome} de {resposta.IpLocal}:{resposta.PortaLocal}!");
                // iniciar partida
                _setGameOver(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public void EnviarMensagemPeloChat(string mensagem)
        {
            try
            {
                var response = _client.SendMessage(mensagem);
                if (response.Equals("Mensagem recebida"))
                {
                    _appendMessage($"{_meuNome}: {mensagem}");
                }
                else
                {
                    _appendMessage(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void EnviarJogada(int x, int y)
        {
            try
            {
                var response = _client.SendCoord(x, y);
                if (response.Equals("ok"))
                {
                    Console.WriteLine("ok");
                }
                else
                {
                    _appendMessage(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void PedirParaReiniciar()
        {
            try
            {
                var response = _client.ResetRequest();
                if (response)
                {
                    _appendMessage("Pedido de reiniciar partida mandado. Aguardando resposta...");
                }
                else
                {
                    Console.WriteLine("Erro na comunicação");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void MandarDecisaoReset(bool g)
        {
            try
            {
                var response = _client.MandarDecisaoReset(g);
                if (response)
                {
                    _appendMessage("Reiniciando partida...");
                    _resetJogo();
                }
                else
                {
                    _appendMessage("Pedido de reiniciar a partida recusado!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
