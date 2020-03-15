﻿using System;
using Grpc.Core;
namespace GrpcBizingoClient
{
    public class GrpcBizingoClientImpl
    {
        readonly BizingoRpc.BizingoRpcClient client;

        public GrpcBizingoClientImpl(BizingoRpc.BizingoRpcClient c)
        {
            this.client = c;
        }
        public Flag ConectarDevolta(
            string ipLocal,
            string ipRemoto,
            int portaLocal,
            int portaRemota,
            string meuNome
            )
        {
            try
            {
                MensagemConexao c = new MensagemConexao
                {
                    IpLocal = ipLocal,
                    IpRemoto = ipRemoto,
                    PortaLocal = portaLocal,
                    PortaRemota = portaRemota,
                    MeuNome = meuNome
                };
                Flag f = client.ConectarDevolta(c);
                return f;
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public MensagemConexao Conectar(
            string ipLocal,
            string ipRemoto,
            int portaLocal,
            int portaRemota,
            string meuNome
            )
        {
            try
            {
                MensagemConexao c = new MensagemConexao
                {
                    IpLocal = ipLocal,
                    IpRemoto = ipRemoto,
                    PortaLocal = portaLocal,
                    PortaRemota = portaRemota,
                    MeuNome = meuNome
                };
                MensagemConexao m = client.Conectar(c);
                Console.WriteLine($"conectou-se com {m.IpLocal} | {m.IpRemoto} | {m.PortaLocal} | {m.PortaRemota} | {m.MeuNome}");
                return m;
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void MandarDialog()
        {
            try
            {
                Mensagem c = new Mensagem { M = "Teste dialog" };
                Mensagem m = client.MandarDialog(c);
                if (m.M.Length > 0)
                {
                    Console.WriteLine(m.M);
                }
                else
                {
                    Console.WriteLine("Nao funcionou");
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void FecharConexao()
        {
            try
            {
                Flag c = new Flag { G = true };
                Mensagem m = client.FecharConexao(c);
                if (m.M.Length > 0)
                {
                    Console.WriteLine(m.M);
                }
                else
                {
                    Console.WriteLine("Nao funcionou");
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool ResetRequest()
        {
            try
            {
                Flag c = new Flag { G = true };
                Flag m = client.ResetRequest(c);
                if (m.G)
                {
                    Console.WriteLine(m.G);
                    return m.G;
                }
                else
                {
                    Console.WriteLine("False");
                    return m.G;
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool MandarDecisaoReset(bool g)
        {
            try
            {
                Flag c = new Flag { G = g };
                Flag m = client.ResetJogo(c);
                if (m.G)
                {
                    Console.WriteLine("Aceito!");
                    return m.G;
                }
                else
                {
                    Console.WriteLine("Recusado!");
                    return m.G;
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void GameOver()
        {
            try
            {
                Flag c = new Flag { G = true };
                Mensagem m = client.GameOver(c);
                if (m.M.Length > 0)
                {
                    Console.WriteLine(m.M);
                }
                else
                {
                    Console.WriteLine("Nao funcionou");
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string SendMessage(string s)
        {
            try
            {
                Mensagem c = new Mensagem { M = s };
                Mensagem m = client.SendMessage(c);
                if (m.M.Length > 0)
                {
                    Console.WriteLine(m.M);
                    return m.M;
                }
                else
                {
                    Console.WriteLine("Mensagem Vazia!");
                    return "Erro ao enviar a mensagem!!";
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string SendCoord(int x, int y)
        {
            try
            {
                Casa c = new Casa { X = x, Y = y };
                Mensagem m = client.SendCoord(c);
                if (m.M.Length > 0)
                {
                    return "ok";
                }
                else
                {
                    return "Ocorreu algum erro ao mandar a jogada!";
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
