using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Bizingo
{
    public class Comunicacao
    {
        private TcpListener _listener;
        private TcpClient _adversario = new TcpClient();
        private int _port;
        private string _adversarioAddress;
        private int players;
        bool running = true;
        List<Task> t = new List<Task>();
        List<string> mensagens = new List<string>();
        private readonly Dictionary<string, Func<string, Task>> _commandHandlers;

        Action<string> appendMessage;
        Action<int, int> moveAsPecas;
        Action<bool> setGameOver;
        Action<int> conexaoFechada;
        Action<int> gameOver;
        Action resetRequest;
        Action<int> resetJogo;
        Action<string> mensagemDialog;
        private int portaLocal;

        public Comunicacao(int porta, 
            Action<string> AppendMessage, 
            Action<int,int> MoveAsPecas,
            Action<bool> SetGameOver,
            Action<int> ConexaoFechada,
            Action<int> GameOver,
            Action ResetRequest,
            Action<int> ResetJogo,
            Action<string> MensagemDialog
            )
        {
            _port = porta;
            _listener = new TcpListener(IPAddress.Any, _port);

            players = 0;
            _commandHandlers = new Dictionary<string, Func<string, Task>>();
            _commandHandlers["tchau"] = _handleCloseConnection;
            _commandHandlers["comando"] = _handleCommand;
            _commandHandlers["mensagem"] = _handleMessage;
            _commandHandlers["gameOver"] = _handleGameOver;
            _commandHandlers["reset"] = _handleReset;

            appendMessage = AppendMessage;
            moveAsPecas = MoveAsPecas;
            setGameOver = SetGameOver;
            conexaoFechada = ConexaoFechada;
            gameOver = GameOver;
            resetRequest = ResetRequest;
            resetJogo = ResetJogo;
            mensagemDialog = MensagemDialog;
        }

        public Comunicacao(int portaLocal, Action<string> appendMessage, Action<int, int> moveAsPecas, Action<bool> setGameOver, Action<int> conexaoFechada, Action<int> gameOver, Action resetRequest, Action<string> mensagemDialog)
        {
            this.portaLocal = portaLocal;
            this.appendMessage = appendMessage;
            this.moveAsPecas = moveAsPecas;
            this.setGameOver = setGameOver;
            this.conexaoFechada = conexaoFechada;
            this.gameOver = gameOver;
            this.resetRequest = resetRequest;
            this.mensagemDialog = mensagemDialog;
        }

        public async void ResetSend(string mensagem)
        {

            Pacote pct = new Pacote(comando: "reset", mensagem: mensagem);
            await MandarPacote(pct);
        }

        private Task _handleReset(string arg)
        {
            //ativar a janela de aceitar reset
            if (arg.Equals("reset plz"))
                resetRequest();
            else if (arg.Equals("0"))
            {
                //aceitou o pedido
                resetJogo(1);
            }
            else if (arg.Equals("2") || arg.Equals("1"))
            {
                //negou o pedido
                //gameOver(int.Parse(arg));
                mensagemDialog($"Pedido negado pelo player {arg}!");
            }
            return Task.FromResult(0);
        }


        private Task _handleGameOver(string arg)
        {
            gameOver(int.Parse(arg));
            Disconnect();
            return Task.FromResult(0);
        }

        private Task _handleMessage(string arg)
        {
            // lidar com mensagens
            appendMessage($"{_adversario.Client.RemoteEndPoint} -> {arg}");
            return Task.FromResult(0);
        }

        private async Task _handleCommand(string arg)
        {
            int x = int.Parse(arg.Split(',')[0]);
            int y = int.Parse(arg.Split(',')[1]);

            //Console.Write($"{arg} x: {x} y: {y}{Environment.NewLine}");

            moveAsPecas(x, y);
        }

        private Task _handleCloseConnection(string arg)
        {
            // o adversario está se desconectando e mandou uma
            // mensagem avisando
            Disconnect();
            conexaoFechada(int.Parse(arg));
            return Task.FromResult(0);
        }

        public void Disconnect()
        {
            running = false;
            _listener.Stop();
            FecharConexao();
        }

        private void FecharConexao()
        {
            _adversario.GetStream()?.Close();
            _adversario.Close();
        }

        public async void Comecar()
        {
            players = 1;
            _listener.Start();
            //Console.WriteLine("Começamos a ouvir");
            await PlayerConectaComVoce();

        }

        public async void FimDeJogo(int jogador)
        {
            Pacote pct = new Pacote(comando: "gameOver", mensagem: jogador.ToString());
            await MandarPacote(pct);
        }

        public async void JogadaSend(int x, int y)
        {

            Pacote pct = new Pacote(comando: "comando", mensagem: x.ToString() + "," + y.ToString());
            await MandarPacote(pct);
        }

        public async void ArregarSend(int jogador)
        {
            Pacote pct = new Pacote(comando: "tchau", mensagem: jogador.ToString());
            await MandarPacote(pct);
        }

        public async void ChatSender(string mensagem)
        {
            Pacote pct = new Pacote(comando: "mensagem", mensagem: mensagem);
            await MandarPacote(pct);
        }

        private void Receiver()
        {
            while (running)
            {
                // tentar receber pacotes a cada 10 ms
                t.Add(ReceberPacote());
                Thread.Sleep(10);
            }
        }

        public void Run()
        {
            Thread receiver = new Thread(new ThreadStart(Receiver));
            receiver.Start();
            /*
            while (true)
            {
                //checar se player adversario desconectou

                Thread.Sleep(10);
            }*/
        }

        public void ConectaComPlayer(string enderecoAdversario)
        {
            try
            {
                _adversario.Connect(enderecoAdversario, _port);
            }
            catch(SocketException e)
            {
                Console.WriteLine($"Erro: {e.Message}");
            }
            if (_adversario.Connected)
            {
                _adversarioAddress = _adversario.Client.RemoteEndPoint.ToString();
                appendMessage($"Conectado com {_adversario.Client.RemoteEndPoint}");
                setGameOver(false);
                Run();
            }
            else
            {
                // A conexão falhou :(
                FecharConexao();
                Console.WriteLine($"Não foi possivel conectar com o adversario em {_adversario.Client.RemoteEndPoint}:{_port}");
            }
        }

        private async Task PlayerConectaComVoce()
        {
            _listener.Start();
            _adversario = await _listener.AcceptTcpClientAsync();
            _adversarioAddress = _adversario.Client.RemoteEndPoint.ToString();
            Console.WriteLine($"conectado com {_adversario.Client.RemoteEndPoint}");
            appendMessage($"player conectado de {_adversario.Client.RemoteEndPoint}");
            players = 2;
            setGameOver(false);
            Run();

        }

        private async Task MandarPacote(Pacote pacote)
        {
            try
            {
                // transformar o pacote em bytes
                byte[] jsonBuffer = Encoding.UTF8.GetBytes(pacote.ToJson());
                // pegar o tamanho do buffer do json
                byte[] bufferLength = BitConverter.GetBytes(Convert.ToUInt16(jsonBuffer.Length));
                // juntar os buffers
                byte[] mensagem = new byte[bufferLength.Length + jsonBuffer.Length];
                bufferLength.CopyTo(mensagem, 0);
                jsonBuffer.CopyTo(mensagem, bufferLength.Length);

                await _adversario.GetStream().WriteAsync(mensagem, 0, mensagem.Length);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro na comunicao!: {e.Message}");
            }
        }

        private async Task ReceberPacote()
        {
            try
            {
                // se houver alguma mensagem mandada
                if (_adversario.Available > 0)
                {
                    Pacote pct;
                    NetworkStream mensagem = _adversario.GetStream();
                    // pegar o tamanho do json
                    byte[] bufferLength = new byte[2];
                    await mensagem.ReadAsync(bufferLength, 0, 2);
                    ushort jsonLength = BitConverter.ToUInt16(bufferLength, 0);

                    // pegar o json
                    byte[] bufferJson = new byte[jsonLength];
                    await mensagem.ReadAsync(bufferJson, 0, bufferJson.Length);
                    string jsonString = Encoding.UTF8.GetString(bufferJson);
                    pct = Pacote.FromJson(jsonString);

                    try
                    {
                        await _commandHandlers[pct.Comando](pct.Mensagem);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Erro: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro na comunicao!: {e.Message}");
            }
        }


    }
}
