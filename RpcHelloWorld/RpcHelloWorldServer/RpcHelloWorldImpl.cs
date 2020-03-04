using System;
using System.Threading.Tasks;
using Grpc.Core;
namespace RpcHelloWorld
{
    public class RpcHelloWorldImpl: Comunicao.ComunicaoBase
    {
        public RpcHelloWorldImpl()
        {
        }
        public override Task<Mensagem> SendCoordinates(Casa c, ServerCallContext context)
        {
            Console.WriteLine($"x: {c.X} y: {c.Y}");
            return Task.FromResult(RetornaMensagem(c));
        }
        public Mensagem RetornaMensagem(Casa c)
        {
            return new Mensagem { M = $"x: {c.X} y: {c.Y}" };
        }

    }
}
