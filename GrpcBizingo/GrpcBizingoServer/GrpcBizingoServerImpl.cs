using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcBizingoServer
{
    public class GrpcBizingoServerImpl : BizingoRpc.BizingoRpcBase
    {
        Action<bool> _resetRequest;
        Action<string> _appendMessage;
        Action<bool> _setGameOver;
        Action<int, int> _moveAsPecas;
        public GrpcBizingoServerImpl(
            Action<bool> resetRequest,
            Action<string> appendMessage,
            Action<bool> setGameOver,
            Action<int, int> moveAsPecas)
        {
            // janela de dialog que retorna true ou false
            _resetRequest = resetRequest;
            // colocar mensagem no chat
            _appendMessage = appendMessage;
            // controla variavel que inicia ou termina o jogo
            _setGameOver = setGameOver;
            // move as pecas do tabuleiro
            _moveAsPecas = moveAsPecas;

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
        public override Task<Flag> ResetRequest(Flag request, ServerCallContext context)
        {
            Console.WriteLine("O adversario pediu para reiniciar a partida!");
            return Task.FromResult(TratarResetRequest());
        }
        public Flag TratarResetRequest()
        {
            bool decisao = false;
            _resetRequest(decisao);
            return new Flag { G = decisao };
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
            return Task.FromResult(TratarSendMensagem(request));
        }
        public Mensagem TratarSendMensagem(Mensagem m)
        {
            Console.WriteLine($"{m.M}");
            switch (m.M)
            {
                case "84eb93488832f124221ab2c1bd802d47":
                    // mensagem de quando um player se conecta com outro
                    _appendMessage("Player conectou-se!");
                    // iniciar partida
                    _setGameOver(false);
                    break;
                default:
                    _appendMessage(m.M);
                    break;
            }
            return new Mensagem { M = "Mensagem recebida" };
        }
        public override Task<Mensagem> SendCoord(Casa request, ServerCallContext context)
        {
            return Task.FromResult(TratarSendCoord(request));
        }
        public Mensagem TratarSendCoord(Casa c)
        {
            Console.WriteLine($"x: {c.X} y: {c.Y}");
            _moveAsPecas(c.X, c.Y);
            return new Mensagem { M = $"x: {c.X} y: {c.Y}" };
        }
    }
}
