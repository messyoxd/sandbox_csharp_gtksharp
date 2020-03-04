using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcBizingoServer
{
    public class GrpcBizingoServerImpl : BizingoRpc.BizingoRpcBase
    {
        public GrpcBizingoServerImpl()
        {
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
        public override Task<Mensagem> ResetRequest(Flag request, ServerCallContext context)
        {
            Console.WriteLine("O adversario pediu para reiniciar a partida!");
            return Task.FromResult(TratarResetRequest());
        }
        public Mensagem TratarResetRequest()
        {
            return new Mensagem { M = "Seu adversario sabe que voce pediu para reiniciar a partida" };
        }
        public override Task<Mensagem> GameOver(Flag request, ServerCallContext context)
        {
            return Task.FromResult(GameOver());
        }
        public Mensagem GameOver()
        {
            Console.WriteLine("O adversario se rendeu!");
            return new Mensagem { M = "Seu adversario sabe que voce se rendeu!" };
        }
        public override Task<Mensagem> SendMessage(Mensagem request, ServerCallContext context)
        {
            return Task.FromResult(CheckMensagem(request));
        }
        public Mensagem CheckMensagem(Mensagem m)
        {
            Console.WriteLine($"{m.M}");
            return new Mensagem { M = "Mensagem recebida" };
        }
        public override Task<Mensagem> SendCoord(Casa request, ServerCallContext context)
        {
            return Task.FromResult(CheckCasa(request));
        }
        public Mensagem CheckCasa(Casa c)
        {
            Console.WriteLine($"x: {c.X} y: {c.Y}");
            return new Mensagem { M = $"x: {c.X} y: {c.Y}" };
        }
    }
}
