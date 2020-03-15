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
        Action _resetJogo;
        Action<string> _appendMessage;
        Action<bool> _setGameOver;
        Action<int, int> _moveAsPecas;
        Action<string, string, int, int, string> _conectarDevolta;
        public GrpcBizingoServerImpl(
            Action<string> resetRequest,
            Action resetJogo,
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
            Console.WriteLine("O adversario pediu para conectar com ele!");
            return Task.FromResult(TratarConectarDevolta(request));
        }
        public Flag TratarConectarDevolta(MensagemConexao request)
        {
            // conectar com adversario
            _conectarDevolta(_ipLocal, _ipRemoto, _portaLocal, _portaRemota, _meuNome);
            // mensagem de quando um player se conecta com outro
            //_appendMessage($"conectou-se com {request.MeuNome} de {request.IpLocal}:{request.PortaLocal}!");
            // iniciar partida
            //_setGameOver(false);
            return new Flag
            {
                G = true
            };
        }
        public override Task<MensagemConexao> Conectar(MensagemConexao request, ServerCallContext context)
        {
            Console.WriteLine("O adversario se conectou conosco!");
            return Task.FromResult(TratarConectar(request));
        }
        public MensagemConexao TratarConectar(MensagemConexao request)
        {
            // mensagem de quando um player se conecta com outro
            //_appendMessage($"{request.MeuNome} conectou-se de {request.IpLocal}!");
            // iniciar partida
            //_setGameOver(false);
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
        public override Task<Mensagem> MandarDialog(Mensagem request, ServerCallContext context)
        {
            Console.WriteLine("O adversario se mandou alguma coisa pro dialog!");
            return Task.FromResult(TratarMandarDialog(request));
        }
        public Mensagem TratarMandarDialog(Mensagem request)
        {
            return new Mensagem { M = request.M };
        }
        public override Task<Mensagem> FecharConexao(Flag request, ServerCallContext context)
        {
            Console.WriteLine("O adversario se desconectou!");
            return Task.FromResult(TratarFecharConexao());
        }
        public Mensagem TratarFecharConexao()
        {
            return new Mensagem { M = "Seu adversario sabe que voce se desconectou" };
        }
        public override Task<Flag> ResetJogo(Flag request, ServerCallContext context)
        {
            Console.WriteLine("O adversario mandou sua resposta quanto a reiniciar a partida");
            return Task.FromResult(TratarResetJogo(request));
        }
        public Flag TratarResetJogo(Flag request)
        {
            if (request.G)
            {
                _resetJogo();
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
            Console.WriteLine("O adversario pediu para reiniciar a partida!");
            return Task.FromResult(TratarResetRequest());
        }
        public Flag TratarResetRequest()
        {
            _resetRequest(_nomeAdversario);
            return new Flag { G = true };
        }
        public override Task<Mensagem> GameOver(Flag request, ServerCallContext context)
        {
            return Task.FromResult(TratarGameOver());
        }
        public Mensagem TratarGameOver()
        {
            Console.WriteLine("O adversario se rendeu!");
            return new Mensagem { M = "Seu adversario sabe que voce se rendeu!" };
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
            Console.WriteLine($"{m.M}");
            _appendMessage($"{_nomeAdversario}: {m.M}");
            return new Mensagem { M = "Mensagem recebida" };
        }
        public override Task<Mensagem> SendCoord(Casa request, ServerCallContext context)
        {
            return Task.FromResult(TratarSendCoord(request));
        }
        public Mensagem TratarSendCoord(Casa c)
        {
            Console.WriteLine($"lolololl x: {c.X} y: {c.Y}");
            _moveAsPecas(c.X, c.Y);
            return new Mensagem { M = $"x: {c.X} y: {c.Y}" };
        }
    }
}
