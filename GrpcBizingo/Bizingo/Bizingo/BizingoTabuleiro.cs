using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cairo;
using Gtk;
using Gdk;
using GrpcCom;

namespace Bizingo
{
    public partial class BizingoTabuleiro : Gtk.Window
    {

        List<ImageSurface> images;

        enum Imagens
        {
            captao_time1,
            captao_time2,
            peça_time_1,
            peça_time_2,
            triangulo_branco,
            triangulo_branco_selecionado,
            triangulo_vermelho,
            triangulo_vermelho_selecionado
        }

        enum TabuleiroCasas
        {
            vazio,
            vermelho,
            vermelho_selecionado,
            branco,
            branco_selecionado
        }

        enum TabuleiroPecas
        {
            vazio,
            peca_time_1,
            peca_time_2,
            captao_time_1,
            captao_time_2
        }
        struct Casas
        {
            public Triangulo t;
            public TabuleiroCasas casa;
            public TabuleiroPecas peca;
        }
        struct Triangulo
        {
            public int[] a;
            public int[] b;
            public int[] c;
        }

        // essa matriz guarda onde estao as pecas no tabuleiro
        Casas casa_selecionada_atual;
        Casas ultima_casa_selecionada;
        Casas[,] casas;
        int x_selecionado;
        int y_selecionado;
        int ultimo_x_selecionado;
        int ultimo_y_selecionado;


        int num_pecas_jogador_1;
        int num_pecas_jogador_2;
        private int _portaLocal;
        private int _portaRemota;
        private string _enderecoRemoto;
        private string _enderecoLocal;
        private string _apelido;
        private int _jogador;
        int turno;
        int player;

        bool gameOver = true;
        //Comunicacao com;
        GrpcComunicacao com;

        public BizingoTabuleiro(int portaLocal, string enderecoLocal, int jogador, string apelido, string enderecoRemoto="127.0.0.1", int portaRemota=50051) :
                base(Gtk.WindowType.Toplevel)
        {
            // constroi os widgets e a tela
            this.Build();
            // pegar o path das imagens
            string currentPath = Regex.Split(AppDomain.CurrentDomain.BaseDirectory, "bin")[0];
            var strs = Directory.GetFiles(currentPath + "images/").OrderBy(f => f);

            // carregar imagens numa lista (acessar usando o enum imagens)
            images = new List<ImageSurface>();
            foreach (string str in strs)
            {
                images.Add(new ImageSurface(str));
            }
            // pinta o widget drawArea de preto
            daTabuleiro.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));

            // inicializacao de variaveis
            casas = new Casas[21, 11];
            PreencheVariaveisTabuleiro();
            casa_selecionada_atual = casas[0, 0];
            ultima_casa_selecionada = casas[0, 0];
            x_selecionado = 0;
            y_selecionado = 0;
            ultimo_x_selecionado = 0;
            ultimo_y_selecionado = 0;

            turno = 0;
            lbTurno.Text = (turno + 1).ToString();

            num_pecas_jogador_1 = 18;
            num_pecas_jogador_2 = 18;

            _portaLocal = portaLocal;
            _portaRemota = portaRemota;
            _enderecoRemoto = enderecoRemoto;
            _enderecoLocal = enderecoLocal;
            _apelido = apelido;
            _jogador = jogador;

            //Comunicacao Grpc
            com = new GrpcComunicacao(
                ResetRequest,
                resetJogo,
                AppendMessage,
                SetGameOver,
                MoveAsPecas,
                _apelido,
                _portaLocal,
                _portaRemota,
                _enderecoRemoto,
                _enderecoLocal
                );
            if (jogador == 1)
            {
                // quando se inicia o jogo
                player = 1;
                com.iniciarConexao();
                AppendMessage("Aguardando algum jogador para iniciar partida");
            }
            else
            {
                player = 2;
                //quando conecta-se com alguem
                com.ConectarComAdversario();
            }
        }

        public void SetGameOver(bool g)
        {
            gameOver = g;
            if (gameOver == false) {
                if ((turno % 2 == 0 && _jogador == 1) || (turno % 2 == 1 && _jogador == 2))
                    lVez.Text = "Sua Vez De Jogar!";
                else if ((turno % 2 == 0 && _jogador == 2) || (turno % 2 == 1 && _jogador == 1))
                    lVez.Text = "Vez Do Adversario!";
            }
            else
            {
                lVez.Text = "Fim do Jogo!";
            }
        }

        private void AddTurno()
        {
            turno++;
            lbTurno.Text = (turno + 1).ToString();
            if ((turno % 2 == 0 && _jogador == 1) || (turno % 2 == 1 && _jogador == 2))
                lVez.Text = "Sua Vez De Jogar!";
            else if ((turno % 2 == 0 && _jogador == 2) || (turno % 2 == 1 && _jogador == 1))
                lVez.Text = "Vez Do Adversario!";
            else
                lVez.Text = "";
        }

        private int DecrementaNumPecas(int jogador)
        {
            if (jogador == 1)
            {
                if (num_pecas_jogador_1 > 3)
                {
                    num_pecas_jogador_1--;
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (num_pecas_jogador_2 > 3)
                {
                    num_pecas_jogador_2--;
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private void GameOver(int jogadorGanhador)
        {
            SetGameOver(true);
            if (player == jogadorGanhador)
            {
                Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal,
                                  MessageType.Info,
                                  ButtonsType.Ok,
                                  "Voce perdeu!");
                dialog.Run();
                dialog.Hide();
            }
            else
            {
                Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal,
                                  MessageType.Info,
                                  ButtonsType.Ok,
                                  "Voce ganhou!");
                dialog.Run();
                dialog.Hide();
            }
        }

        public void ConexaoFechada(int jogador)
        {
            gameOver = true;
            Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal | DialogFlags.DestroyWithParent,
                                  MessageType.Info,
                                  ButtonsType.Ok,
                                  $"Jogador adversario saiu da jogo!");
            dialog.Run();
            dialog.Hide();
        }

        private Triangulo InitTrianguloVermelho(int x, int y)
        {
            Triangulo t = new Triangulo();
            t.a = new int[2];
            t.b = new int[2];
            t.c = new int[2];

            int triangulo_base = 40;
            int triangulo_altura = 40;

            t.a[0] = x;
            t.a[1] = y;
            //vertice b novo ponto
            t.b[0] = x + (triangulo_base / 2);
            t.b[1] = y - triangulo_altura;
            //vertice c original
            t.c[0] = x + triangulo_base;
            t.c[1] = y;

            return t;
        }

        private Triangulo InitTrianguloBranco(int x, int y)
        {
            Triangulo t = new Triangulo();
            t.a = new int[2];
            t.b = new int[2];
            t.c = new int[2];

            int triangulo_base = 40;
            int triangulo_altura = 40;

            t.a[0] = x;
            t.a[1] = y;
            //vertice b novo ponto
            t.b[0] = x + (triangulo_base / 2);
            t.b[1] = y + triangulo_altura;
            //vertice c original
            t.c[0] = x + triangulo_base;
            t.c[1] = y;

            return t;
        }

        private Triangulo InitTrianguloVazio()
        {
            Triangulo t = new Triangulo();
            t.a = new int[2];
            t.b = new int[2];
            t.c = new int[2];

            t.a[0] = 0;
            t.a[1] = 0;
            //vertice b novo ponto
            t.b[0] = 0;
            t.b[1] = 0;
            //vertice c original
            t.c[0] = 0;
            t.c[1] = 0;

            return t;
        }

        private void PreencheVariaveisTabuleiro()
        {
            // desenhar triangulos vermelhos

            // num+1 é o numero de triangulos de espaço que devem ser dados para começar a desenhar os primeiros
            // triangulos de cima para baixo
            int num = 4;
            // começa com 3 triangulos vermelhos
            int triangulos = 3;

            // variavel que irá incrementar e ajudar as coisas
            int aux = 0;

            for (int i = 0; i < 11; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == 10)
                    {
                        num++;
                    }
                    for (int j = 0; j < triangulos; j++)
                    {
                        casas[(8 - aux) + (2 * j), i].t = InitTrianguloVermelho(40 * (num + j), 40 * (i + 1));
                        casas[(8 - aux) + (2 * j), i].casa = TabuleiroCasas.vermelho;
                        casas[(8 - aux) + (2 * j), i].peca = TabuleiroPecas.vazio;
                    }
                }
                else
                {
                    if (i < 9)
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            casas[(8 - aux) + (2 * j), i].t = InitTrianguloVermelho(40 * (num + j) - 20, 40 * (i + 1));
                            casas[(8 - aux) + (2 * j), i].casa = TabuleiroCasas.vermelho;
                            casas[(8 - aux) + (2 * j), i].peca = TabuleiroPecas.vazio;
                        }
                        if (num > 0)
                            num--;
                    }
                    else
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            casas[(8 - aux) + (2 * j), i].t = InitTrianguloVermelho(40 * (num + j) + 20, 40 * (i + 1));
                            casas[(8 - aux) + (2 * j), i].casa = TabuleiroCasas.vermelho;
                            casas[(8 - aux) + (2 * j), i].peca = TabuleiroPecas.vazio;
                        }
                    }

                }
                if (i >= 8)
                {
                    triangulos--;
                }
                else
                {
                    triangulos++;
                }

                if (i >= 8)
                {
                    // aux vai para 7 e depois para 6
                    aux--;
                }
                else
                {
                    aux++;
                }
            }

            // desenhar triangulos brancos

            // num+1 é o numero de triangulos de espaço que devem ser dados para começar a desenhar os primeiros
            // triangulos de cima para baixo
            num = 4;
            // começa com 2 triangulos brancos
            triangulos = 2;

            // variavel que irá incrementar e ajudar as coisas
            aux = 0;

            for (int i = 0; i < 11; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == 10)
                    {
                        //num++;
                    }
                    for (int j = 0; j < triangulos; j++)
                    {
                        casas[(9 - aux) + (2 * j), i].t = InitTrianguloBranco(40 * (num + j) + 20, 40 * i);
                        casas[(9 - aux) + (2 * j), i].casa = TabuleiroCasas.branco;
                        casas[(9 - aux) + (2 * j), i].peca = TabuleiroPecas.vazio;
                    }
                }
                else
                {
                    if (i < 9)
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            casas[(9 - aux) + (2 * j), i].t = InitTrianguloBranco(40 * (num + j), 40 * i);
                            casas[(9 - aux) + (2 * j), i].casa = TabuleiroCasas.branco;
                            casas[(9 - aux) + (2 * j), i].peca = TabuleiroPecas.vazio;
                        }
                        if (num > 0)
                            num--;
                    }
                    else
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            casas[(9 - aux) + (2 * j), i].t = InitTrianguloBranco(40 * (num + j), 40 * i);
                            casas[(9 - aux) + (2 * j), i].casa = TabuleiroCasas.branco;
                            casas[(9 - aux) + (2 * j), i].peca = TabuleiroPecas.vazio;
                        }
                    }

                }
                if (i >= 9)
                {
                    triangulos--;
                }
                else
                {
                    triangulos++;
                }

                if (i >= 9)
                {
                    // aux vai para 8 e depois para 7( o 7 não será usado)
                    aux--;
                }
                else
                {
                    aux++;
                }
            }

            for (int k = 0; k < 11; k++)
            {
                for (int w = 0; w < 21; w++)
                {
                    if (casas[w, k].casa == TabuleiroCasas.vazio)
                    {
                        casas[w, k].peca = TabuleiroPecas.vazio;
                        casas[w, k].t = InitTrianguloVazio();
                    }

                }
            }

            //colocando as peças
            int m = 2;
            int n = 8;
            int num_de_pecas_na_linha = 0;
            int aux2 = m;
            while (m < 10)
            {
                if (m < 6)
                {
                    num_de_pecas_na_linha = m + 1;
                    if (m == 5)
                    {
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            if (p == 1 || p == 4)
                                casas[n + (p * 2), aux2].peca = TabuleiroPecas.captao_time_1;
                            else
                                casas[n + (p * 2), aux2].peca = TabuleiroPecas.peca_time_1;
                        }
                        m++;
                        aux2++;
                        n--;
                    }
                    else
                    {
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            casas[n + (p * 2), aux2].peca = TabuleiroPecas.peca_time_1;
                        }
                        m++;
                        aux2++;
                        n--;
                    }
                }
                else
                {
                    //m=6 aux2=6 n=4
                    if (m == 6)
                    {
                        m++;
                        aux2++;
                    }
                    else if (m == 7)
                    {
                        num_de_pecas_na_linha = m;
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            if (p == 1 || p == 5)
                                casas[n + (p * 2), aux2].peca = TabuleiroPecas.captao_time_2;
                            else
                                casas[n + (p * 2), aux2].peca = TabuleiroPecas.peca_time_2;
                        }
                        m++;
                        aux2--;
                        n++;
                    }
                    else
                    {
                        num_de_pecas_na_linha = aux2;
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            casas[n + (p * 2), m].peca = TabuleiroPecas.peca_time_2;
                        }
                        m++;
                        aux2--;
                        n++;
                    }
                }

            }

        }

        private void DesenhaTabuleiro()
        {
            // limpar o widget drawingArea de qualquer imagem
            daTabuleiro.GdkWindow.Clear();
            // cria objeto que ira desenhar na drawingArea
            Cairo.Context ct = Gdk.CairoHelper.Create(daTabuleiro.GdkWindow);

            // desenhar triangulos vermelhos
            int num = 4;
            // começa com 3 triangulos vermelhos
            int triangulos = 3;

            // variavel que irá incrementar e ajudar as coisas
            int aux = 0;

            for (int i = 0; i < 11; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == 10)
                        num++;
                    for (int j = 0; j < triangulos; j++)
                    {
                        if (casas[(8 - aux) + (2 * j), i].casa == TabuleiroCasas.vermelho_selecionado)
                            ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], 40 * (num + j), 40 * i);
                        else if (casas[(8 - aux) + (2 * j), i].casa == TabuleiroCasas.vermelho)
                            ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho], 40 * (num + j), 40 * i);
                        ct.Paint();
                    }
                }
                else
                {
                    if (i < 9)
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            if (casas[(8 - aux) + (2 * j), i].casa == TabuleiroCasas.vermelho_selecionado)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], 40 * (num + j) - 20, 40 * i);
                            else if (casas[(8 - aux) + (2 * j), i].casa == TabuleiroCasas.vermelho)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho], 40 * (num + j) - 20, 40 * i);
                            ct.Paint();
                        }
                        if (num > 0)
                            num--;
                    }
                    else
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            if (casas[(8 - aux) + (2 * j), i].casa == TabuleiroCasas.vermelho_selecionado)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], 40 * (num + j) + 20, 40 * i);
                            else if (casas[(8 - aux) + (2 * j), i].casa == TabuleiroCasas.vermelho)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho], 40 * (num + j) + 20, 40 * i);
                            ct.Paint();
                        }
                    }

                }
                if (i >= 8)
                {
                    triangulos--;
                }
                else
                {
                    triangulos++;
                }

                if (i >= 8)
                {
                    // aux vai para 7 e depois para 6
                    aux--;
                }
                else
                {
                    aux++;
                }
            }

            // desenhar triangulos brancos
            num = 4;
            // começa com 3 triangulos vermelhos
            triangulos = 2;

            // variavel que irá incrementar e ajudar as coisas
            aux = 0;

            for (int i = 0; i < 11; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < triangulos; j++)
                    {
                        if (casas[(9 - aux) + (2 * j), i].casa == TabuleiroCasas.branco_selecionado)
                            ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], 40 * (num + j) + 20, 40 * i);
                        else if (casas[(9 - aux) + (2 * j), i].casa == TabuleiroCasas.branco)
                            ct.SetSourceSurface(images[(int)Imagens.triangulo_branco], 40 * (num + j) + 20, 40 * i);
                        ct.Paint();
                    }
                }
                else
                {
                    if (i < 9)
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            if (casas[(9 - aux) + (2 * j), i].casa == TabuleiroCasas.branco_selecionado)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], 40 * (num + j), 40 * i);
                            else if (casas[(9 - aux) + (2 * j), i].casa == TabuleiroCasas.branco)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_branco], 40 * (num + j), 40 * i);
                            ct.Paint();
                        }
                        if (num > 0)
                            num--;
                    }
                    else
                    {
                        for (int j = 0; j < triangulos; j++)
                        {
                            if (casas[(9 - aux) + (2 * j), i].casa == TabuleiroCasas.branco_selecionado)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], 40 * (num + j), 40 * i);
                            else if (casas[(9 - aux) + (2 * j), i].casa == TabuleiroCasas.branco)
                                ct.SetSourceSurface(images[(int)Imagens.triangulo_branco], 40 * (num + j), 40 * i);
                            ct.Paint();
                        }
                    }

                }
                if (i >= 9)
                {
                    triangulos--;
                }
                else
                {
                    triangulos++;
                }

                if (i >= 9)
                {
                    // aux vai para 8 e depois para 7( o 7 não será usado)
                    aux--;
                }
                else
                {
                    aux++;
                }
            }

            //colocando as peças
            int m = 2;
            int n = 8;
            int num_de_pecas_na_linha = 0;
            int aux2 = m;
            while (m < 10)
            {
                if (m < 6)
                {
                    num_de_pecas_na_linha = m + 1;
                    if (m == 5)
                    {
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            if (p == 1 || p == 4)
                            {
                                ct.SetSourceSurface(images[(int)Imagens.captao_time1], casas[n + (p * 2), aux2].t.a[0] + 13, casas[n + (p * 2), aux2].t.a[1] - 20);
                                ct.Paint();
                            }
                            else
                            {
                                ct.SetSourceSurface(images[(int)Imagens.peça_time_1], casas[n + (p * 2), aux2].t.a[0] + 13, casas[n + (p * 2), aux2].t.a[1] - 20);
                                ct.Paint();
                            }
                        }
                        m++;
                        aux2++;
                        n--;
                    }
                    else
                    {
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            ct.SetSourceSurface(images[(int)Imagens.peça_time_1], casas[n + (p * 2), aux2].t.a[0] + 13, casas[n + (p * 2), aux2].t.a[1] - 20);
                            ct.Paint();
                        }
                        m++;
                        aux2++;
                        n--;
                    }
                }
                else
                {
                    //m=6 aux2=6 n=4
                    if (m == 6)
                    {
                        m++;
                        aux2++;
                    }
                    else if (m == 7)
                    {
                        num_de_pecas_na_linha = m;
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            if (p == 1 || p == 5)
                            {
                                ct.SetSourceSurface(images[(int)Imagens.captao_time2], casas[n + (p * 2), aux2].t.a[0] + 13, casas[n + (p * 2), aux2].t.a[1] + 8);
                                ct.Paint();
                            }
                            else
                            {
                                ct.SetSourceSurface(images[(int)Imagens.peça_time_2], casas[n + (p * 2), aux2].t.a[0] + 13, casas[n + (p * 2), aux2].t.a[1] + 8);
                                ct.Paint();
                            }
                        }
                        m++;
                        aux2--;
                        n++;
                    }
                    else if (m > 7 && m < 10)
                    {
                        num_de_pecas_na_linha = aux2;
                        for (int p = 0; p < num_de_pecas_na_linha; p++)
                        {
                            ct.SetSourceSurface(images[(int)Imagens.peça_time_2], casas[n + (p * 2), m].t.a[0] + 13, casas[n + (p * 2), m].t.a[1] + 8);
                            ct.Paint();
                        }
                        m++;
                        aux2--;
                        n++;
                    }
                }

            }
        }

        protected void OnTabuleiroExposeEvent(object o, Gtk.ExposeEventArgs args)
        {
            DesenhaTabuleiro();
        }

        protected void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            com.Desconectar();
            Application.Quit();
            args.RetVal = true;
            this.Destroy();
        }

        protected void OnDaTabuleiroButtonPressEvent(object o, ButtonPressEventArgs args)
        {

            int x, y;
            ModifierType state;
            args.Event.Window.GetPointer(out x, out y, out state);
            if (!gameOver)
            {
                //turno do player 1
                if (turno % 2 == 0 && player == 1 && x != 0 && y != 0)
                    MoveAsPecas(x, y);
                // turno do player 2
                else if(turno % 2 == 1 && player == 2 && x != 0 && y != 0)
                    MoveAsPecas(x, y);
                x = 0;
                y = 0;
            }
        }

        private void TurnoPlayerUm(int x,int y)
        {
            Cairo.Context ct = Gdk.CairoHelper.Create(daTabuleiro.GdkWindow);

            // pega a casa que foi clicada
            GetCasaTabuleiro(x, y);

            // mostrar caminhos possiveis da peça
            if (
                casa_selecionada_atual.peca != TabuleiroPecas.vazio 
                && 
                casa_selecionada_atual.casa == TabuleiroCasas.vermelho
                )
            {
                // se foi clicado numa peça vermelha então deve-se 
                // pintar devolta casas selecionadas de vermelho
                // e pintar de vermelho acinzentado as novas casas
                EncerrarSelecao();
                PecaSelecionada();
            }
            else if (
                casa_selecionada_atual.casa == TabuleiroCasas.vermelho_selecionado
                )
            {
                // a casa atual é a que a peça deve se deslocar para
                // como ela esta selecionada deve-se pinta-la de branco
                casas[x_selecionado, y_selecionado].casa = TabuleiroCasas.vermelho;
                ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho], casas[x_selecionado, y_selecionado].t.a[0], casas[x_selecionado, y_selecionado].t.a[1] - 40);
                ct.Paint();
                // coloca a peça na casa atual
                casas[x_selecionado, y_selecionado].peca = casas[ultimo_x_selecionado, ultimo_y_selecionado].peca;
                if (casas[ultimo_x_selecionado, ultimo_y_selecionado].peca == TabuleiroPecas.captao_time_1)
                    ct.SetSourceSurface(images[(int)Imagens.captao_time1], casas[x_selecionado, y_selecionado].t.a[0] + 13, casas[x_selecionado, y_selecionado].t.a[1] - 20);
                else if (casas[ultimo_x_selecionado, ultimo_y_selecionado].peca == TabuleiroPecas.peca_time_1)
                    ct.SetSourceSurface(images[(int)Imagens.peça_time_1], casas[x_selecionado, y_selecionado].t.a[0] + 13, casas[x_selecionado, y_selecionado].t.a[1] - 20);
                ct.Paint();
                // a ultima casa selecionada agora deve estar sem peças
                casas[ultimo_x_selecionado, ultimo_y_selecionado].peca = TabuleiroPecas.vazio;
                ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho], casas[ultimo_x_selecionado, ultimo_y_selecionado].t.a[0], casas[ultimo_x_selecionado, ultimo_y_selecionado].t.a[1] - 40);
                ct.Paint();

                casa_selecionada_atual = casas[0, 0];
                ultima_casa_selecionada = casas[0, 0];

                EncerrarSelecao();
                List<int> pecaEliminada;
                pecaEliminada = CheckPecaVermelhaCercada();

                if (pecaEliminada.Any())
                {
                    for (int i = 0; i < pecaEliminada.Count(); i += 2)
                    {
                        ExcluiPeca(pecaEliminada[i], pecaEliminada[i + 1]);
                    }
                }
                pecaEliminada.Clear();
                pecaEliminada = CheckVermelhaEliminou();
                if (pecaEliminada.Any())
                {
                    for (int i = 0; i < pecaEliminada.Count(); i += 2)
                    {
                        ExcluiPeca(pecaEliminada[i], pecaEliminada[i + 1]);
                    }
                }

                pecaEliminada.Clear();
                AddTurno();
            }
            else
            {
                EncerrarSelecao();
            }
        }

        private void TurnoPlayerDois(int x, int y)
        {
            Cairo.Context ct = Gdk.CairoHelper.Create(daTabuleiro.GdkWindow);
            GetCasaTabuleiro(x, y);
            if (
                casa_selecionada_atual.peca != TabuleiroPecas.vazio 
                && 
                casa_selecionada_atual.casa == TabuleiroCasas.branco
                )
            {
                EncerrarSelecao();
                PecaSelecionada();
            }
            else if (
                casa_selecionada_atual.casa == TabuleiroCasas.branco_selecionado
                )
            {
                //a casa atual é a que a peça deve se deslocar para
                // como ela esta selecionada deve-se pinta-la de branco
                casas[x_selecionado, y_selecionado].casa = TabuleiroCasas.branco;
                ct.SetSourceSurface(images[(int)Imagens.triangulo_branco], casas[x_selecionado, y_selecionado].t.a[0], casas[x_selecionado, y_selecionado].t.a[1]);
                ct.Paint();
                // coloca a peça na casa atual
                casas[x_selecionado, y_selecionado].peca = casas[ultimo_x_selecionado, ultimo_y_selecionado].peca;
                if (casas[ultimo_x_selecionado, ultimo_y_selecionado].peca == TabuleiroPecas.captao_time_2)
                    ct.SetSourceSurface(images[(int)Imagens.captao_time2], casas[x_selecionado, y_selecionado].t.a[0] + 13, casas[x_selecionado, y_selecionado].t.a[1] + 8);
                else if (casas[ultimo_x_selecionado, ultimo_y_selecionado].peca == TabuleiroPecas.peca_time_2)
                    ct.SetSourceSurface(images[(int)Imagens.peça_time_2], casas[x_selecionado, y_selecionado].t.a[0] + 13, casas[x_selecionado, y_selecionado].t.a[1] + 8);
                ct.Paint();
                // a ultima casa selecionada agora deve estar sem peças
                casas[ultimo_x_selecionado, ultimo_y_selecionado].peca = TabuleiroPecas.vazio;
                ct.SetSourceSurface(images[(int)Imagens.triangulo_branco], casas[ultimo_x_selecionado, ultimo_y_selecionado].t.a[0], casas[ultimo_x_selecionado, ultimo_y_selecionado].t.a[1]);
                ct.Paint();

                casa_selecionada_atual = casas[0, 0];
                ultima_casa_selecionada = casas[0, 0];

                EncerrarSelecao();

                List<int> pecaEliminada;
                pecaEliminada = CheckPecaBrancaCercada();

                if (pecaEliminada.Any())
                {
                    for (int i = 0; i < pecaEliminada.Count(); i += 2)
                    {
                        ExcluiPeca(pecaEliminada[i], pecaEliminada[i + 1]);
                    }
                }
                pecaEliminada.Clear();
                pecaEliminada = CheckBrancaEliminou();
                if (pecaEliminada.Any())
                {
                    for (int i = 0; i < pecaEliminada.Count(); i += 2)
                    {
                        ExcluiPeca(pecaEliminada[i], pecaEliminada[i + 1]);
                    }
                }

                pecaEliminada.Clear();
                AddTurno();
            }
            else
            {
                EncerrarSelecao();
            }
        }

        public void MoveAsPecas(int x, int y)
        {

            // se for o turno do primeiro jogador
            if (turno % 2 == 0 && player == 1)
            {
                TurnoPlayerUm(x, y);
                com.EnviarJogada(x, y);
            }
            else if (turno % 2 == 0 && player == 2)
            {
                // player 2 está vendo o que o player 1 esta fazendo
                TurnoPlayerUm(x, y);
            }
            else if (turno % 2 == 1 && player == 2)
            {
                TurnoPlayerDois(x, y);
                com.EnviarJogada(x, y);
            }

            else if (turno % 2 == 1 && player == 1)
            {
                // player 1 está vendo o que o player 2 esta fazendo
                TurnoPlayerDois(x, y);
            }
        }

        private bool Capitao1Cercado3(int x, int y)
        {
            if (
                (
                    casas[x, y].peca == TabuleiroPecas.captao_time_2 &&
                    casas[x + 2, y].peca == TabuleiroPecas.peca_time_2 &&
                    casas[x + 1, y + 1].peca == TabuleiroPecas.peca_time_2
                )
                ||
                (
                    casas[x, y].peca == TabuleiroPecas.peca_time_2 &&
                    casas[x + 2, y].peca == TabuleiroPecas.captao_time_2 &&
                    casas[x + 1, y + 1].peca == TabuleiroPecas.peca_time_2
                )
                ||
                (
                    casas[x, y].peca == TabuleiroPecas.peca_time_2 &&
                    casas[x + 2, y].peca == TabuleiroPecas.peca_time_2 &&
                    casas[x + 1, y + 1].peca == TabuleiroPecas.captao_time_2
                )
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool Capitao2Cercado3(int x, int y)
        {
            if (
                (
                    casas[x, y].peca == TabuleiroPecas.captao_time_1 &&
                    casas[x + 2, y].peca == TabuleiroPecas.peca_time_1 &&
                    casas[x + 1, y - 1].peca == TabuleiroPecas.peca_time_1
                )
                ||
                (
                    casas[x, y].peca == TabuleiroPecas.peca_time_1 &&
                    casas[x + 2, y].peca == TabuleiroPecas.captao_time_1 &&
                    casas[x + 1, y - 1].peca == TabuleiroPecas.peca_time_1
                )
                ||
                (
                    casas[x, y].peca == TabuleiroPecas.peca_time_1 &&
                    casas[x + 2, y].peca == TabuleiroPecas.peca_time_1 &&
                    casas[x + 1, y - 1].peca == TabuleiroPecas.captao_time_1
                )
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool PecaVermelhaCercado3(int x, int y)
        {
            if (
                casas[x, y].peca != TabuleiroPecas.vazio &&
                casas[x + 2, y].peca != TabuleiroPecas.vazio &&
                casas[x + 1, y + 1].peca != TabuleiroPecas.vazio
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool PecaBrancaCercado3(int x, int y)
        {
            if (
                casas[x, y].peca != TabuleiroPecas.vazio &&
                casas[x + 2, y].peca != TabuleiroPecas.vazio &&
                casas[x + 1, y - 1].peca != TabuleiroPecas.vazio
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckCasaVermelhaBorda(int x, int y)
        {
            if (y > 8 || y < 0 || x > 21 || x < 0)
            {
                return false;
            }
            else if (x + y == 8 || x - y == 12)
                return true;
            else
                return false;
        }

        private bool CheckCasaBrancaBorda(int x, int y)
        {
            if (y < 8 || y > 10 || x > 21 || x < 0)
            {
                return false;
            }
            else if (y == 9)
            {
                if (x + (y - 1) == 8 || x - (y - 1) == 12)
                    return true;
                else
                    return false;
            }
            else
            {
                //y == 10
                if (x + (y - 3) == 8 || x - (y - 3) == 12)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private List<int> CheckBrancaEliminou()
        {
            //checar se a peça branca cercou alguem
            List<int> pecaEliminada = new List<int>();

            // se a casa atual não é de borda, então há pelo menos uma casa a direita ou esquerda,
            // que será uma casa adversaria. Porém se essa casa adversaria é de borda, então
            // bastam duas peças para cerca-la

            // checar se peça a direita é de borda
            if (CheckCasaVermelhaBorda(x_selecionado + 1, y_selecionado))
            {
                // se for, então há duas possibilidades para cerco. Ou a casa a direita foi cercada
                // ou a casa à cima foi cercada. Mas antes disso, checar se há peças nelas

                if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                    {
                        // se a peça da borda da direita é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_2 ||
                            casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_2
                            )
                        {
                            pecaEliminada.Add(x_selecionado + 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                    {
                        // se a peça da borda da direita é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_2
                            )
                            ||
                            (
                                casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                            )
                        {
                            pecaEliminada.Add(x_selecionado + 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    return pecaEliminada;
                }
                else if (casas[x_selecionado, y_selecionado - 1].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1)
                    {
                        // se a peça da borda da direita é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado - 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_2 ||
                            casas[x_selecionado - 1, y_selecionado - 1].peca == TabuleiroPecas.captao_time_2
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado - 1);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1)
                    {
                        // se a peça da borda da direita é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado - 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_2
                            )
                            ||
                            (
                                casas[x_selecionado - 1, y_selecionado - 1].peca == TabuleiroPecas.captao_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado - 1);
                        }
                    }
                    return pecaEliminada;
                }
                else
                {
                    return pecaEliminada;
                }
            }
            // checar se peça a esquerda é de borda
            else if (CheckCasaVermelhaBorda(x_selecionado - 1, y_selecionado))
            {
                // se for, então há duas possibilidades para cerco. Ou a casa a esquerda foi cercada
                // ou a casa à cima foi cercada. Mas antes disso, checar se há peças nelas
                if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                    {
                        // se a peça da borda da direita é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_2 ||
                            casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_2
                            )
                        {
                            pecaEliminada.Add(x_selecionado - 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                    {
                        // se a peça da borda da direita é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_2
                            )
                            ||
                            (
                                casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                            )
                        {
                            pecaEliminada.Add(x_selecionado - 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    return pecaEliminada;
                }
                else if (casas[x_selecionado, y_selecionado - 1].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1)
                    {
                        // se a peça da borda da direita é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_2 ||
                            casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.captao_time_2
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado - 1);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1)
                    {
                        // se a peça da borda da direita é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_2
                            )
                            ||
                            (
                                casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.captao_time_2
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado - 1);
                        }
                    }
                    return pecaEliminada;
                }
                else
                {
                    return pecaEliminada;
                }
            }

            else
            {


                // se não esta perto de bordas, checar se há alguma peça adversaria em volta.
                // (supondo que dá para eliminar mais de uma peça adversaria numa só jogada)

                // checar se é borda
                if (CheckCasaBrancaBorda(x_selecionado, y_selecionado))
                {
                    //esquerda
                    if ((x_selecionado == 0 && y_selecionado == 9)
                    ||
                    (x_selecionado == 1 && y_selecionado == 10))
                    {
                        if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio)
                        {
                            // checar qual peça é
                            if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                            {
                                //checar as condições 
                                if (PecaVermelhaCercado3(x_selecionado, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado + 1);
                                    pecaEliminada.Add(y_selecionado);
                                }
                            }
                            else if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                            {
                                //condições
                                if (Capitao1Cercado3(x_selecionado, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado + 1);
                                    pecaEliminada.Add(y_selecionado);
                                }
                            }
                        }
                    }
                    //direita
                    else if ((x_selecionado == 20 && y_selecionado == 9)
                    ||
                    (x_selecionado == 19 && y_selecionado == 10))
                    {
                        if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio)
                        {
                            if(casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                            {
                                if (PecaVermelhaCercado3(x_selecionado - 2, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado - 1);
                                    pecaEliminada.Add(y_selecionado);
                                }
                            }
                            else if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                            {
                                if (Capitao1Cercado3(x_selecionado - 2, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado - 1);
                                    pecaEliminada.Add(y_selecionado);
                                }

                            }
                        }
                    }
                    return pecaEliminada;
                }
                else
                {
                    //direita
                    if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio)
                    {
                        // checar qual peça é
                        if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                        {
                            //checar as condições 
                            if (PecaVermelhaCercado3(x_selecionado, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado + 1);
                                pecaEliminada.Add(y_selecionado);
                            }
                        }
                        else if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                        {
                            //condições
                            if (Capitao1Cercado3(x_selecionado, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado + 1);
                                pecaEliminada.Add(y_selecionado);
                            }
                        }
                    }
                    // em cima
                    if (casas[x_selecionado, y_selecionado - 1].peca != TabuleiroPecas.vazio)
                    {
                        if (casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1)
                        {
                            if (PecaVermelhaCercado3(x_selecionado - 1, y_selecionado - 1))
                            {
                                pecaEliminada.Add(x_selecionado);
                                pecaEliminada.Add(y_selecionado - 1);
                            }

                        }
                        else if (casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1)
                        {
                            if (Capitao1Cercado3(x_selecionado - 1, y_selecionado - 1))
                            {
                                pecaEliminada.Add(x_selecionado - 1);
                                pecaEliminada.Add(y_selecionado - 1);
                            }
                        }
                    }

                    //esquerda
                    if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio)
                    {
                        if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                        {
                            if (PecaVermelhaCercado3(x_selecionado - 2, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado - 1);
                                pecaEliminada.Add(y_selecionado);
                            }
                        }
                        else if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                        {
                            if (Capitao1Cercado3(x_selecionado - 2, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado - 1);
                                pecaEliminada.Add(y_selecionado);
                            }

                        }
                    }
                    return pecaEliminada;
                }
            }
        }

        private List<int> CheckVermelhaEliminou()
        {
            //checar se a peça branca cercou alguem
            List<int> pecaEliminada = new List<int>();

            // se a casa atual não é de borda, então há pelo menos uma casa a direita ou esquerda,
            // que será uma casa adversaria. Porém se essa casa adversaria é de borda, então
            // bastam duas peças para cerca-la

            // checar se peça a direita é de borda
            if (CheckCasaBrancaBorda(x_selecionado + 1, y_selecionado))
            {
                // se for, então há duas possibilidades para cerco. Ou a casa a direita foi cercada
                // ou a casa à baixo foi cercada. Mas antes disso, checar se há peças nelas

                if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                    {
                        // se a peça da borda da direita é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1 ||
                            casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1
                            )
                        {
                            pecaEliminada.Add(x_selecionado + 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                    {
                        // se a peça da borda da direita é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_1
                            )
                            ||
                            (
                                casas[x_selecionado + 1, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                            )
                        {
                            pecaEliminada.Add(x_selecionado + 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    return pecaEliminada;
                }
                else if (casas[x_selecionado, y_selecionado + 1].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado, y_selecionado + 1].peca == TabuleiroPecas.peca_time_2)
                    {
                        // se a peça abaixo é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_1 ||
                            casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_1
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado + 1);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado + 1].peca == TabuleiroPecas.captao_time_2)
                    {
                        // se a peça abaixo é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_1
                                ||
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_1
                            )
                            ||
                            (
                                casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_1
                                ||
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado + 1);
                        }
                    }
                    return pecaEliminada;
                }
                else
                {
                    return pecaEliminada;
                }
            }
            // checar se peça a esquerda é de borda
            else if (CheckCasaBrancaBorda(x_selecionado - 1, y_selecionado))
            {
                // se for, então há duas possibilidades para cerco. Ou a casa a esquerda foi cercada
                // ou a casa à baixo foi cercada. Mas antes disso, checar se há peças nelas
                if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                    {
                        // se a peça da borda da esquerda é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado - 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1 ||
                            casas[x_selecionado - 1, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1
                            )
                        {
                            pecaEliminada.Add(x_selecionado - 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                    {
                        // se a peça da borda da esquerda é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado - 1, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_1
                            )
                            ||
                            (
                                casas[x_selecionado - 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_1
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                            )
                        {
                            pecaEliminada.Add(x_selecionado - 1);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    return pecaEliminada;
                }
                else if (casas[x_selecionado, y_selecionado + 1].peca != TabuleiroPecas.vazio)
                {
                    if (casas[x_selecionado, y_selecionado + 1].peca == TabuleiroPecas.peca_time_2)
                    {
                        // se a peça abaixo é uma peça comum, checar se há uma peça
                        // aliada a cercando
                        if (casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_1 ||
                            casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_1
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado + 1);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado + 1].peca == TabuleiroPecas.captao_time_2)
                    {
                        // se a peça abaixo é um capitão, checar se as condições para
                        // eliminação dessa peça foram consedidas

                        if (
                            (
                                casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.peca_time_1
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_1
                            )
                            ||
                            (
                                casas[x_selecionado + 1, y_selecionado + 1].peca == TabuleiroPecas.captao_time_1
                                &&
                                casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                            )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado - 1);
                        }
                    }
                    return pecaEliminada;
                }
                else
                {
                    return pecaEliminada;
                }
            }

            else
            {
                // checar se é borda
                if (CheckCasaVermelhaBorda(x_selecionado, y_selecionado))
                {
                    //esquerda
                    if (x_selecionado + y_selecionado == 8)
                    {
                        if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio)
                        {
                            // checar qual peça é
                            if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                            {
                                //checar as condições 
                                if (PecaBrancaCercado3(x_selecionado, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado + 1);
                                    pecaEliminada.Add(y_selecionado);
                                }
                            }
                            else if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                            {
                                //condições
                                if (Capitao2Cercado3(x_selecionado, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado + 1);
                                    pecaEliminada.Add(y_selecionado);
                                }
                            }
                        }
                    }
                    //direita
                    else if (x_selecionado - y_selecionado == 12)
                    {
                        if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio)
                        {
                            if(casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                            {
                                if (PecaBrancaCercado3(x_selecionado - 2, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado - 1);
                                    pecaEliminada.Add(y_selecionado);
                                }
                            }
                            else if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                            {
                                if (Capitao1Cercado3(x_selecionado - 2, y_selecionado))
                                {
                                    pecaEliminada.Add(x_selecionado - 1);
                                    pecaEliminada.Add(y_selecionado);
                                }
                            }
                        }
                    }
                    return pecaEliminada;
                }
                else
                {
                    // se não esta perto de bordas, checar se há alguma peça adversaria em volta.
                    // (supondo que dá para eliminar mais de uma peça adversaria numa só jogada)

                    //direita
                    if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio)
                    {
                        // checar qual peça é
                        if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                        {
                            //checar as condições 
                            if (PecaBrancaCercado3(x_selecionado, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado + 1);
                                pecaEliminada.Add(y_selecionado);
                            }
                        }
                        else if (casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                        {
                            //condições
                            if (Capitao2Cercado3(x_selecionado, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado + 1);
                                pecaEliminada.Add(y_selecionado);
                            }
                        }
                    }
                    // em baixo
                    if (casas[x_selecionado, y_selecionado + 1].peca != TabuleiroPecas.vazio)
                    {
                        if (casas[x_selecionado, y_selecionado + 1].peca == TabuleiroPecas.peca_time_2)
                        {

                            if (PecaBrancaCercado3(x_selecionado - 1 , y_selecionado + 1))
                            {
                                pecaEliminada.Add(x_selecionado);
                                pecaEliminada.Add(y_selecionado + 1);
                            }
                        }
                        else if (casas[x_selecionado, y_selecionado + 1].peca == TabuleiroPecas.captao_time_2)
                        {
                            if (Capitao2Cercado3(x_selecionado - 1, y_selecionado + 1))
                            {
                                pecaEliminada.Add(x_selecionado);
                                pecaEliminada.Add(y_selecionado + 1);
                            }

                        }
                    }

                    //esquerda
                    if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio)
                    {
                        if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                        {

                            if (PecaBrancaCercado3(x_selecionado - 2, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado - 1);
                                pecaEliminada.Add(y_selecionado);
                            }
                        }
                        else if (casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                        {
                            if (Capitao2Cercado3(x_selecionado - 2, y_selecionado))
                            {
                                pecaEliminada.Add(x_selecionado - 1);
                                pecaEliminada.Add(y_selecionado);
                            }
                        }
                    }
                    return pecaEliminada;
                }
            }
        }

        private List<int> CheckPecaBrancaCercada()
        {
            /*
                // essa função será usada no momento em que qualquer peça do
                // tabuleiro se desloque para verificar se tal peça será eliminada
                // ou se eliminou alguma peça
                retorno:
                List<int> com os endereços x e y da peça que deve ser eliminada
                em decorrencia da ultima atualização da posição das peças do tabuleiro
                List<int> terá length multiplo de 2, pois mais de uma peça pode ser
                eliminada
            */
            List<int> pecaEliminada = new List<int>();
            // se a peça não for de borda

            if (!CheckCasaBrancaBorda(x_selecionado, y_selecionado))
            {
                // checar se a peça foi cercada

                //checar se a peça é um capitão
                if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                {
                    // checar as condições
                    if (Capitao2Cercado3(x_selecionado - 1, y_selecionado))
                    {
                        pecaEliminada.Add(x_selecionado);
                        pecaEliminada.Add(y_selecionado);
                    }
                }
                else if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                {
                    // se for uma peca normal, então basta estar cercada
                    if (PecaBrancaCercado3(x_selecionado - 1, y_selecionado))
                    {
                        // se tem peças cercando então aquela peça tem que ser excluida
                        pecaEliminada.Add(x_selecionado);
                        pecaEliminada.Add(y_selecionado);
                    }
                }
                return pecaEliminada;

            }
            else
            {
                //se for uma casa branca da borda esquerda
                if (
                    (x_selecionado == 0 && y_selecionado == 9)
                    ||
                    (x_selecionado == 1 && y_selecionado == 10)
                  )
                {
                    // checar qual peça é
                    if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                    {
                        //checar condições
                        if (
                            (
                                casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_1 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1
                            )
                            ||
                            (
                                casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_1 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1
                            )
                        )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                    {
                        if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio &&
                            casas[x_selecionado, y_selecionado - 1].peca != TabuleiroPecas.vazio)
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    return pecaEliminada;
                }
                //se for uma casa branca da borda direita
                else if (
                    (x_selecionado == 20 && y_selecionado == 9)
                    ||
                    (x_selecionado == 19 && y_selecionado == 10)
                  )
                {
                    // checar qual peça é
                    if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_2)
                    {
                        //checar condições
                        if (
                            (
                                casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_1 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.peca_time_1
                            )
                            ||
                            (
                                casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_1 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.captao_time_1
                            )
                        )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_2)
                    {
                        if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio &&
                            casas[x_selecionado, y_selecionado - 1].peca != TabuleiroPecas.vazio)
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                }
                return pecaEliminada;
            }
        }

        private List<int> CheckPecaVermelhaCercada()
        {
            /*
                // essa função será usada no momento em que qualquer peça do
                // tabuleiro se desloque para verificar se tal peça será eliminada
                // ou se eliminou alguma peça
                retorno:
                List<int> com os endereços x e y da peça que deve ser eliminada
                em decorrencia da ultima atualização da posição das peças do tabuleiro
                List<int> terá length multiplo de 2, pois mais de uma peça pode ser
                eliminada
            */
            List<int> pecaEliminada = new List<int>();
            // se a peça não for de borda

            if (!CheckCasaVermelhaBorda(x_selecionado, y_selecionado))
            {
                // checar se a peça foi cercada

                //checar se a peça é um capitão
                if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                {
                    // checar as condições
                    if (Capitao1Cercado3(x_selecionado - 1, y_selecionado))
                    {
                        pecaEliminada.Add(x_selecionado);
                        pecaEliminada.Add(y_selecionado);
                    }
                }
                else if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                {
                    // se for uma peca normal, então basta estar cercada
                    if (PecaVermelhaCercado3(x_selecionado - 1, y_selecionado))
                    {
                        // se tem peças cercando então aquela peça tem que ser excluida
                        pecaEliminada.Add(x_selecionado);
                        pecaEliminada.Add(y_selecionado);
                    }
                }
                return pecaEliminada;

            }
            else
            {
                //se for uma casa vermelha da borda esquerda
                if (x_selecionado + y_selecionado == 8)
                {
                    // checar qual peça é
                    if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                    {
                        //checar condições
                        if (
                            (
                                casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.captao_time_2 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.peca_time_2
                            )
                            ||
                            (
                                casas[x_selecionado + 1, y_selecionado].peca == TabuleiroPecas.peca_time_2 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.captao_time_2
                            )
                        )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                    {
                        if (casas[x_selecionado + 1, y_selecionado].peca != TabuleiroPecas.vazio &&
                            casas[x_selecionado, y_selecionado - 1].peca != TabuleiroPecas.vazio)
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    return pecaEliminada;
                }
                //se for uma casa branca da borda direita
                else if (x_selecionado - y_selecionado == 12)
                {
                    // checar qual peça é
                    if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.captao_time_1)
                    {
                        //checar condições
                        if (
                            (
                                casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.captao_time_2 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.peca_time_2
                            )
                            ||
                            (
                                casas[x_selecionado - 1, y_selecionado].peca == TabuleiroPecas.peca_time_2 &&
                                casas[x_selecionado, y_selecionado - 1].peca == TabuleiroPecas.captao_time_2
                            )
                        )
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                    else if (casas[x_selecionado, y_selecionado].peca == TabuleiroPecas.peca_time_1)
                    {
                        if (casas[x_selecionado - 1, y_selecionado].peca != TabuleiroPecas.vazio &&
                            casas[x_selecionado, y_selecionado - 1].peca != TabuleiroPecas.vazio)
                        {
                            pecaEliminada.Add(x_selecionado);
                            pecaEliminada.Add(y_selecionado);
                        }
                    }
                }
                return pecaEliminada;
            }
        }

        private void ExcluiPeca(int x, int y)
        {
            Cairo.Context ct = Gdk.CairoHelper.Create(daTabuleiro.GdkWindow);
            // se for uma casa branca com alguma peça
            if (
                casas[x, y].casa == TabuleiroCasas.branco
                &&
                (
                    casas[x, y].peca == TabuleiroPecas.captao_time_2 ||
                    casas[x, y].peca == TabuleiroPecas.peca_time_2
                )
            )
            {
                // se tem peças cercando então aquela peça tem que ser excluida
                if (DecrementaNumPecas(2) == 0)
                {
                    GameOver(2);
                }
                casas[x, y].peca = TabuleiroPecas.vazio;
                ct.SetSourceSurface(images[(int)Imagens.triangulo_branco], casas[x, y].t.a[0], casas[x, y].t.a[1]);
                ct.Paint();

            }
            else if (casas[x, y].casa == TabuleiroCasas.vermelho &&
                (
                casas[x, y].peca == TabuleiroPecas.captao_time_1 ||
                casas[x, y].peca == TabuleiroPecas.peca_time_1
                )
                )
            {
                // se tem peças cercando então aquela peça tem que ser excluida
                if (DecrementaNumPecas(1) == 0)
                {
                    GameOver(1);
                }
                casas[x, y].peca = TabuleiroPecas.vazio;
                ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho], casas[x, y].t.a[0], casas[x, y].t.a[1] - 40);
                ct.Paint();

            }
        }

        private void EncerrarSelecao()
        {
            Cairo.Context ct = Gdk.CairoHelper.Create(daTabuleiro.GdkWindow);
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 21; j++)
                {
                    if (casas[j, i].casa == TabuleiroCasas.branco_selecionado)
                    {
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco], casas[j, i].t.a[0], casas[j, i].t.a[1]);
                        ct.Paint();
                        casas[j, i].casa = TabuleiroCasas.branco;
                    }
                    else if (casas[j, i].casa == TabuleiroCasas.vermelho_selecionado)
                    {
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho], casas[j, i].t.a[0], casas[j, i].t.a[1] - 40);
                        ct.Paint();
                        casas[j, i].casa = TabuleiroCasas.vermelho;
                    }

                }
            }
        }

        protected void GetCasaTabuleiro(int x, int y)
        {
            int y_axis = (int)y / 40;
            int numero_de_triangulos_por_linha = 0;
            if (y_axis < 9)
                numero_de_triangulos_por_linha = 5 + (2 * y_axis);
            else
            {
                if (y_axis == 9)
                    numero_de_triangulos_por_linha = 5 + (2 * (y_axis - 1));
                else if (y_axis == 10)
                    numero_de_triangulos_por_linha = 5 + (2 * (y_axis - 3));
            }
            int k = 0;
            int menor_area = int.MaxValue;
            int x_axis = 0;
            int aux;
            if (y_axis >= 0 && y_axis <= 10)
            {
                for (int i = 0; i < numero_de_triangulos_por_linha; i++)
                {
                    if (y_axis == 8 || y_axis == 9)
                        k = i;
                    else if (y_axis == 10)
                    {
                        k = i + 1;
                    }
                    else
                    {
                        k = ((8 - y_axis) + i);
                    }
                    Casas t = casas[k, y_axis];
                    aux = TriangleCollision(x, y, t);
                    if (aux < menor_area)
                    {
                        menor_area = aux;
                        x_axis = k;
                    }
                }
                if (TriangleCollision(x, y, casas[x_axis, y_axis]) == 800)
                {
                    // se for o turno do primeiro jogador e ele tiver clicado numa
                    // casa da cor vermelha ou vermelha selecionada, então permitir
                    // que aquela casa seja selecionada
                    if ((casas[x_axis, y_axis].casa == TabuleiroCasas.vermelho ||
                        casas[x_axis, y_axis].casa == TabuleiroCasas.vermelho_selecionado
                        ) && turno % 2 == 0)
                    {
                        ultimo_x_selecionado = x_selecionado;
                        ultimo_y_selecionado = y_selecionado;
                        x_selecionado = x_axis;
                        y_selecionado = y_axis;
                        ultima_casa_selecionada = casa_selecionada_atual;
                        casa_selecionada_atual = casas[x_axis, y_axis];
                    }
                    // se for o turno do segundo jogador e ele tiver clicado numa
                    // casa da cor branca ou branca selecionada, então permitir
                    // que aquela casa seja selecionada
                    else if ((casas[x_axis, y_axis].casa == TabuleiroCasas.branco ||
                        casas[x_axis, y_axis].casa == TabuleiroCasas.branco_selecionado
                        ) && turno % 2 == 1)
                    {
                        ultimo_x_selecionado = x_selecionado;
                        ultimo_y_selecionado = y_selecionado;
                        x_selecionado = x_axis;
                        y_selecionado = y_axis;
                        ultima_casa_selecionada = casa_selecionada_atual;
                        casa_selecionada_atual = casas[x_axis, y_axis];
                    }

                }
                else
                {
                    x_selecionado = 0;
                    y_selecionado = 0;
                    ultimo_x_selecionado = 0;
                    ultimo_y_selecionado = 0;
                    ultima_casa_selecionada = casas[0, 0];
                    casa_selecionada_atual = casas[0, 0];
                    return;
                }

                return;
            }
            casa_selecionada_atual = casas[0, 0];
            ultima_casa_selecionada = casas[0, 0];
            return;
        }

        private int TriangleCollision(int x, int y, Casas cs)
        {
            int triangulo_base = 40;
            int triangulo_altura = 40;

            int[] a = { x, y };
            int[] b = { x + (triangulo_base / 2), y + triangulo_altura };
            int[] c = { x + triangulo_base, y };

            int area_triangulo_original = Math.Abs((a[0] * (b[1] - c[1]) + b[0] * (c[1] - a[1]) + c[0] * (a[1] - b[1])) / 2);

            Triangulo t1, t2, t3;

            t1 = new Triangulo();
            t1.a = new int[2];
            t1.b = new int[2];
            t1.c = new int[2];
            //vertice a original
            t1.a[0] = cs.t.a[0];
            t1.a[1] = cs.t.a[1];
            //vertice b novo ponto
            t1.b[0] = x;
            t1.b[1] = y;
            //vertice c original
            t1.c[0] = cs.t.c[0];
            t1.c[1] = cs.t.c[1];

            int area_t1 = Math.Abs((t1.a[0] * (t1.b[1] - t1.c[1]) + t1.b[0] * (t1.c[1] - t1.a[1]) + t1.c[0] * (t1.a[1] - t1.b[1])) / 2);

            t2 = new Triangulo();
            t2.a = new int[2];
            t2.b = new int[2];
            t2.c = new int[2];
            //vertice a novo ponto
            t2.a[0] = x;
            t2.a[1] = y;
            //vertice b original
            t2.b[0] = cs.t.b[0];
            t2.b[1] = cs.t.b[1];
            //vertice c original
            t2.c[0] = cs.t.c[0];
            t2.c[1] = cs.t.c[1];

            int area_t2 = Math.Abs((t2.a[0] * (t2.b[1] - t2.c[1]) + t2.b[0] * (t2.c[1] - t2.a[1]) + t2.c[0] * (t2.a[1] - t2.b[1])) / 2);

            t3 = new Triangulo();
            t3.a = new int[2];
            t3.b = new int[2];
            t3.c = new int[2];
            //vertice a original
            t3.a[0] = cs.t.a[0];
            t3.a[1] = cs.t.a[1];
            //vertice b original
            t3.b[0] = cs.t.b[0];
            t3.b[1] = cs.t.b[1];
            //vertice c novo ponto
            t3.c[0] = x;
            t3.c[1] = y;

            int area_t3 = Math.Abs((t3.a[0] * (t3.b[1] - t3.c[1]) + t3.b[0] * (t3.c[1] - t3.a[1]) + t3.c[0] * (t3.a[1] - t3.b[1])) / 2);

            return area_t1 + area_t2 + area_t3;
        }

        private void PecaSelecionada()
        {
            Cairo.Context ct = Gdk.CairoHelper.Create(daTabuleiro.GdkWindow);
            if (x_selecionado > 1)
            {
                if (Check_movimento_valido(x_selecionado - 2, y_selecionado))
                {
                    if (casa_selecionada_atual.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado - 2, y_selecionado].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado - 2, y_selecionado].t.a[0], casas[x_selecionado - 2, y_selecionado].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada_atual.casa == TabuleiroCasas.vermelho)
                    {
                        casas[x_selecionado - 2, y_selecionado].casa = TabuleiroCasas.vermelho_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], casas[x_selecionado - 2, y_selecionado].t.a[0], casas[x_selecionado - 2, y_selecionado].t.a[1] - 40);
                        ct.Paint();
                    }
                }
            }
            if (x_selecionado < 19)
            {

                if (Check_movimento_valido(x_selecionado + 2, y_selecionado))
                {
                    if (casa_selecionada_atual.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado + 2, y_selecionado].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado + 2, y_selecionado].t.a[0], casas[x_selecionado + 2, y_selecionado].t.a[1]);
                        ct.Paint();

                    }
                    else if (casa_selecionada_atual.casa == TabuleiroCasas.vermelho)
                    {
                        casas[x_selecionado + 2, y_selecionado].casa = TabuleiroCasas.vermelho_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], casas[x_selecionado + 2, y_selecionado].t.a[0], casas[x_selecionado + 2, y_selecionado].t.a[1] - 40);
                        ct.Paint();
                    }
                }
            }
            if (x_selecionado > 0 && y_selecionado < 10)
            {

                if (Check_movimento_valido(x_selecionado - 1, y_selecionado + 1))
                {
                    if (casa_selecionada_atual.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado - 1, y_selecionado + 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado - 1, y_selecionado + 1].t.a[0], casas[x_selecionado - 1, y_selecionado + 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada_atual.casa == TabuleiroCasas.vermelho)
                    {
                        casas[x_selecionado - 1, y_selecionado + 1].casa = TabuleiroCasas.vermelho_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], casas[x_selecionado - 1, y_selecionado + 1].t.a[0], casas[x_selecionado - 1, y_selecionado + 1].t.a[1] - 40);
                        ct.Paint();
                    }
                }
            }
            if (x_selecionado < 20 && y_selecionado < 10)
            {
                if (Check_movimento_valido(x_selecionado + 1, y_selecionado + 1))
                {
                    if (casa_selecionada_atual.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado + 1, y_selecionado + 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado + 1, y_selecionado + 1].t.a[0], casas[x_selecionado + 1, y_selecionado + 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada_atual.casa == TabuleiroCasas.vermelho)
                    {
                        casas[x_selecionado + 1, y_selecionado + 1].casa = TabuleiroCasas.vermelho_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], casas[x_selecionado + 1, y_selecionado + 1].t.a[0], casas[x_selecionado + 1, y_selecionado + 1].t.a[1] - 40);
                        ct.Paint();
                    }
                }
            }
            if (x_selecionado > 0 && y_selecionado > 0)
            {

                if (Check_movimento_valido(x_selecionado - 1, y_selecionado - 1))
                {
                    if (casa_selecionada_atual.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado - 1, y_selecionado - 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado - 1, y_selecionado - 1].t.a[0], casas[x_selecionado - 1, y_selecionado - 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada_atual.casa == TabuleiroCasas.vermelho)
                    {
                        casas[x_selecionado - 1, y_selecionado - 1].casa = TabuleiroCasas.vermelho_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], casas[x_selecionado - 1, y_selecionado - 1].t.a[0], casas[x_selecionado - 1, y_selecionado - 1].t.a[1] - 40);
                        ct.Paint();
                    }
                }
            }
            if (x_selecionado < 20 && y_selecionado > 0)
            {
                if (Check_movimento_valido(x_selecionado + 1, y_selecionado - 1))
                {
                    if (casa_selecionada_atual.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado + 1, y_selecionado - 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado + 1, y_selecionado - 1].t.a[0], casas[x_selecionado + 1, y_selecionado - 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada_atual.casa == TabuleiroCasas.vermelho)
                    {
                        casas[x_selecionado + 1, y_selecionado - 1].casa = TabuleiroCasas.vermelho_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_vermelho_selecionado], casas[x_selecionado + 1, y_selecionado - 1].t.a[0], casas[x_selecionado + 1, y_selecionado - 1].t.a[1] - 40);
                        ct.Paint();
                    }
                }
            }
        }

        private bool Check_movimento_valido(int x, int y)
        {
            // se não for uma casa valida
            if (casa_selecionada_atual.casa == TabuleiroCasas.vazio)
                return false;
            //se for uma casa da mesma cor e não tiver uma peça nela
            if (casas[x, y].casa == casa_selecionada_atual.casa && casas[x, y].peca == TabuleiroPecas.vazio)
                return true;
            return false;
        }

        protected void OnBtnResetClicked(object sender, EventArgs e)
        {
            com.PedirParaReiniciar();
        }

        private void resetJogo(bool inverterJogadores)
        {
            PreencheVariaveisTabuleiro();
            DesenhaTabuleiro();
            gameOver = false;
            turno = 0;
            lbTurno.Text = (turno + 1).ToString();
            if (inverterJogadores)
            {
                Console.WriteLine("Invertendo jogadores");
                if (_jogador == 1)
                    _jogador = 2;
                else
                    _jogador = 1;
            }
            if ((turno % 2 == 0 && _jogador == 1) || (turno % 2 == 1 && _jogador == 2))
                lVez.Text = "Sua Vez De Jogar!";
            else if ((turno % 2 == 0 && _jogador == 2) || (turno % 2 == 1 && _jogador == 1))
                lVez.Text = "Vez Do Adversario!";

            casa_selecionada_atual = casas[0, 0];
            ultima_casa_selecionada = casas[0, 0];
            x_selecionado = 0;
            y_selecionado = 0;
            ultimo_x_selecionado = 0;
            ultimo_y_selecionado = 0;
        }

        private void MensagemDialog(string mensagem)
        {
            Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal,
                                  MessageType.Info,
                                  ButtonsType.Ok,
                                  mensagem);
            dialog.Run();
            dialog.Hide();
        }


        private void ResetRequest(string nomeAdversario)
        {
            var reset = new ResetRequest(com.MandarDecisaoReset, nomeAdversario);
        }

        protected void OnBtnSendMessageClicked(object sender, EventArgs e)
        {
            if(eMensagem.Text != "")
            {
                com.EnviarMensagemPeloChat(eMensagem.Text);
                eMensagem.Text = "";
            }
        }

        private void AppendMessage(string message)
        {
            Gtk.Label l = new Gtk.Label(message);
            l.UseMarkup = true;
            vbox2.PackStart(l, false, false, 0);
            ShowAll();
        }

    }
}
