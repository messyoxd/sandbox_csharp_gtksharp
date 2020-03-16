using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcBizingoServer
{
    public class GrpcBizingoServerImpl : BizingoRpc.BizingoRpcBase
    {
        private int _portaLocal;
        private int _portaRemota;
        private string _ipRemoto;
        private string _ipLocal;
        private string _meuNome;
        private string _nomeAdversario;
        Action<string> _resetRequest;
        Action<bool> _resetJogo;
        Action<string> _appendMessage;
        Action<bool> _setGameOver;
        Action<int, int> _moveAsPecas;
        Action<string, string, int, int, string> _conectarDevolta;
        public GrpcBizingoServerImpl(
            Action<string> resetRequest,
            Action<bool> resetJogo,
            Action<string> appendMessage,
            Action<bool> setGameOver,
            Action<int, int> moveAsPecas,
            Action<string, string, int, int, string> conectarDevolta,
            string meuNome,
            int portaLocal = 50052,
            int portaRemota = 50051,
            string ipRemoto = "127.0.0.1",
            string ipLocal = "localhost"
            )
        {

            // janela de dialog que retorna true ou false
            _resetRequest = resetRequest;
            _resetJogo = resetJogo;
            // colocar mensagem no chat
            _appendMessage = appendMessage;
            // controla variavel que inicia ou termina o jogo
            _setGameOver = setGameOver;
            // move as pecas do tabuleiro
            _moveAsPecas = moveAsPecas;
            // conecta devolta com o jogador que se conectou
            _conectarDevolta = conectarDevolta;

            // variaveis de conexao
            _portaLocal = portaLocal;
            _portaRemota = portaRemota;
            _ipRemoto = ipRemoto;
            _ipLocal = ipLocal;
            _meuNome = meuNome;

        }
        public override Task<Flag> ConectarDevolta(MensagemConexao request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarConectarDevolta(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Flag TratarConectarDevolta(MensagemConexao request)
        {
            // conectar com adversario
            _conectarDevolta(_ipLocal, _ipRemoto, _portaLocal, _portaRemota, _meuNome);
            return new Flag
            {
                G = true
            };
        }
        public override Task<MensagemConexao> Conectar(MensagemConexao request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarConectar(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public MensagemConexao TratarConectar(MensagemConexao request)
        {
            _portaRemota = request.PortaLocal;
            _ipRemoto = request.IpLocal;
            _nomeAdversario = request.MeuNome;
            return new MensagemConexao
            {
                IpLocal = _ipLocal,
                IpRemoto = _ipRemoto,
                PortaLocal = _portaLocal,
                PortaRemota = _portaRemota,
                MeuNome = _meuNome
            };
        }

        public override Task<Mensagem> FecharConexao(Flag request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarFecharConexao());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Mensagem TratarFecharConexao()
        {
            _appendMessage("Seu adversario se desconectou.");
            _appendMessage("O jogo será finalizado.");
            _setGameOver(true);
            //_resetJogo(true);
            //_appendMessage("Aguardando novo jogador");
            return new Mensagem { M = "Seu adversario sabe que voce se desconectou" };
        }
        public override Task<Flag> ResetJogo(Flag request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarResetJogo(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Flag TratarResetJogo(Flag request)
        {
            if (request.G)
            {
                _resetJogo(false);
                _appendMessage("Reiniciando partida...");
                return new Flag { G = true };
            }
            else
            {
                _appendMessage("Pedido de reiniciar a partida recusado!");
                return new Flag { G = false };
            }

        }

        public override Task<Flag> ResetRequest(Flag request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarResetRequest());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Flag TratarResetRequest()
        {
            _resetRequest(_nomeAdversario);
            return new Flag { G = true };
        }

        public override Task<Mensagem> SendMessage(Mensagem request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarSendMensagem(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Mensagem TratarSendMensagem(Mensagem m)
        {
            _appendMessage($"{_nomeAdversario}: {m.M}");
            return new Mensagem { M = "Mensagem recebida" };
        }
        public override Task<Mensagem> SendCoord(Casa request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(TratarSendCoord(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public Mensagem TratarSendCoord(Casa c)
        {
            _moveAsPecas(c.X, c.Y);
            return new Mensagem { M = $"x: {c.X} y: {c.Y}" };
        }
    }
}
