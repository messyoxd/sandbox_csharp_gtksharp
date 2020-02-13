﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cairo;
using Gtk;
using Gdk;

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
        Casas casa_selecionada;
        Casas[,] casas;
        int x_selecionado;
        int y_selecionado;
        public BizingoTabuleiro() :
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
            casa_selecionada = casas[0, 0];
            x_selecionado = 0;
            y_selecionado = 0;
        }

        private Triangulo initTrianguloVermelho(int x, int y)
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

        private Triangulo initTrianguloBranco(int x, int y)
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

        private Triangulo initTrianguloVazio()
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
                        //Console.WriteLine($"aux: {aux} num:{num}");
                    }
                    for (int j = 0; j < triangulos; j++)
                    {
                        casas[(8 - aux) + (2 * j), i].t = initTrianguloVermelho(40 * (num + j), 40 * (i + 1));
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
                            casas[(8 - aux) + (2 * j), i].t = initTrianguloVermelho(40 * (num + j) - 20, 40 * (i + 1));
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
                            casas[(8 - aux) + (2 * j), i].t = initTrianguloVermelho(40 * (num + j) + 20, 40 * (i + 1));
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
                        Console.WriteLine($"aux: {aux} num:{num}");
                    }
                    for (int j = 0; j < triangulos; j++)
                    {
                        casas[(9 - aux) + (2 * j), i].t = initTrianguloBranco(40 * (num + j) + 20, 40 * i);
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
                            casas[(9 - aux) + (2 * j), i].t = initTrianguloBranco(40 * (num + j), 40 * i);
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
                            casas[(9 - aux) + (2 * j), i].t = initTrianguloBranco(40 * (num + j), 40 * i);
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
                        casas[w, k].t = initTrianguloVazio();
                    }
                    //Console.WriteLine($"linha: {k} coluna:{w} casa:{casas[w, k].casa} x:{casas[w, k].t.a[0]} y:{casas[w, k].t.a[1]}");

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

        protected void OnTabuleiroExposeEvent(object o, Gtk.ExposeEventArgs args)
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
                        //Console.WriteLine($"{40 * (num + j)} {40 * (i)}");
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
                            //Console.WriteLine($"{40 * (num + j) - 20} {40 * (i)}");
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
                            //Console.WriteLine($"{40 * (num + j) + 20} {40 * (i)}");
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

                            //Console.WriteLine($"x: {n + (p * 2) } y: {m}");

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

        protected void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }

        protected void OnDaTabuleiroButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            int x, y;
            ModifierType state;
            args.Event.Window.GetPointer(out x, out y, out state);
            //Console.WriteLine($"x: {x} y:{y} state: {state}");

            GetCasaTabuleiro(x, y);

            Console.WriteLine($"x: {x_selecionado} | y: {y_selecionado} |casa:{casa_selecionada.casa} |peça: {casa_selecionada.peca} | x1:{casa_selecionada.t.a[0]} y1:{casa_selecionada.t.a[1]} | x2:{casa_selecionada.t.b[0]} y2:{casa_selecionada.t.b[1]} | x3:{casa_selecionada.t.c[0]} y3:{casa_selecionada.t.c[1]}|");

            if (casa_selecionada.peca != TabuleiroPecas.vazio)
            {
                encerrarSelecao();
                pecaSelecionada();
            }
            else
            {
                encerrarSelecao();
            }
        }

        private void encerrarSelecao()
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
                //e se x não pertense a nenhum triangulo?
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
                    x_selecionado = x_axis;
                    y_selecionado = y_axis;
                    casa_selecionada = casas[x_axis, y_axis];
                }
                else
                {
                    x_selecionado = 0;
                    y_selecionado = 0;
                    casa_selecionada = casas[0, 0];
                    return;
                }

                return;
            }
            casa_selecionada = casas[0, 0];
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

        private void pecaSelecionada()
        {
            Cairo.Context ct = Gdk.CairoHelper.Create(daTabuleiro.GdkWindow);
            //Console.WriteLine($"x: {x_selecionado} y:{y_selecionado}");
            //Console.WriteLine($"x selecionado: {casas[x_selecionado, y_selecionado].t.a[0]} Y selecionado: {casas[x_selecionado, y_selecionado].t.a[1]}");
            if (x_selecionado > 1)
            {
                if (Check_movimento_valido(x_selecionado - 2, y_selecionado))
                {
                    Console.WriteLine($"x selecionado: {casas[x_selecionado - 2, y_selecionado].t.a[0]} Y selecionado: {casas[x_selecionado - 2, y_selecionado].t.a[1]}");
                    if (casa_selecionada.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado - 2, y_selecionado].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado - 2, y_selecionado].t.a[0], casas[x_selecionado - 2, y_selecionado].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada.casa == TabuleiroCasas.vermelho)
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
                    Console.WriteLine($"x selecionado: {casas[x_selecionado + 2, y_selecionado].t.a[0]} Y selecionado: {casas[x_selecionado + 2, y_selecionado].t.a[1]}");
                    if (casa_selecionada.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado + 2, y_selecionado].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado + 2, y_selecionado].t.a[0], casas[x_selecionado + 2, y_selecionado].t.a[1]);
                        ct.Paint();

                    }
                    else if (casa_selecionada.casa == TabuleiroCasas.vermelho)
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
                    Console.WriteLine($"x: {x_selecionado - 1} x selecionado: {casas[x_selecionado - 1, y_selecionado + 1].t.a[0]} Y selecionado: {casas[x_selecionado - 1, y_selecionado + 1].t.a[1]}");
                    if (casa_selecionada.casa == TabuleiroCasas.branco)
                    {
                        Console.WriteLine(casas[x_selecionado - 1, y_selecionado + 1].t.a[0]);
                        casas[x_selecionado - 1, y_selecionado + 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado - 1, y_selecionado + 1].t.a[0], casas[x_selecionado - 1, y_selecionado + 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada.casa == TabuleiroCasas.vermelho)
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
                    Console.WriteLine($"x selecionado: {casas[x_selecionado + 1, y_selecionado + 1].t.a[0]} Y selecionado: {casas[x_selecionado + 1, y_selecionado + 1].t.a[1]}");
                    if (casa_selecionada.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado + 1, y_selecionado + 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado + 1, y_selecionado + 1].t.a[0], casas[x_selecionado + 1, y_selecionado + 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada.casa == TabuleiroCasas.vermelho)
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
                    Console.WriteLine($"x selecionado: {casas[x_selecionado - 1, y_selecionado - 1].t.a[0]} Y selecionado: {casas[x_selecionado - 1, y_selecionado - 1].t.a[1]}");
                    if (casa_selecionada.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado - 1, y_selecionado - 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado - 1, y_selecionado - 1].t.a[0], casas[x_selecionado - 1, y_selecionado - 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada.casa == TabuleiroCasas.vermelho)
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
                    Console.WriteLine($"x selecionado: {casas[x_selecionado + 1, y_selecionado - 1].t.a[0]} Y selecionado: {casas[x_selecionado + 1, y_selecionado - 1].t.a[1]}");
                    if (casa_selecionada.casa == TabuleiroCasas.branco)
                    {
                        casas[x_selecionado + 1, y_selecionado - 1].casa = TabuleiroCasas.branco_selecionado;
                        ct.SetSourceSurface(images[(int)Imagens.triangulo_branco_selecionado], casas[x_selecionado + 1, y_selecionado - 1].t.a[0], casas[x_selecionado + 1, y_selecionado - 1].t.a[1]);
                        ct.Paint();
                    }
                    else if (casa_selecionada.casa == TabuleiroCasas.vermelho)
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
            if (casa_selecionada.casa == TabuleiroCasas.vazio)
                return false;
            //se for uma casa da mesma cor e não tiver uma peça nela
            if (casas[x, y].casa == casa_selecionada.casa && casas[x, y].peca == TabuleiroPecas.vazio)
                return true;
            return false;
        }
    }
}
