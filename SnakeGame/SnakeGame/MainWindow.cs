using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cairo;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    Random rand;

    enum GameBoardFields
    {
        Free,
        Snake,
        Bonus
    };
    enum Directions
    {
        Up,
        Down,
        Left,
        Right
    };

    struct SnakeCoordinates
    {
        public int x;
        public int y;
    }

    List<ImageSurface> images;

    GameBoardFields[,] gameBoardField;
    SnakeCoordinates[] snakeXY;
    int bonusCount;
    int snakeLength;
    bool isStarted;
    Directions direction;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        gameBoardField = new GameBoardFields[11, 11];
        snakeXY = new SnakeCoordinates[100];
        rand = new Random();
        string currentPath = Regex.Split(AppDomain.CurrentDomain.BaseDirectory, "bin")[0];
        var strs = Directory.GetFiles(currentPath + "images/").OrderBy(f => f);

        images = new List<ImageSurface>();
        /*
            0 -> bonus1.png
            1 -> bonus2.png
            2 -> bonus3.png
            3 -> bonus4.png
            4 -> snake_body.png
            5 -> snake_head.png
            6 -> wall.png
        */
        foreach (string str in strs)
        {
            images.Add(new ImageSurface(str));
        }
        isStarted = false;
        picGameBoard.ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 255));
        ClockStart();

    }



    void ClockStart()
    {
        if (!isStarted)
        {
            Console.WriteLine("era pra ta funcionando");
            initBoardDrawing();
            isStarted = true;
        }
        Console.WriteLine("era pra ta funcionando2");
        GLib.Timeout.Add(1000, new GLib.TimeoutHandler(DrawBoard));
    }

    bool DrawBoard()
    {
        Console.WriteLine("n ta pronto");
        return true;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    private void initBoardDrawing()
    {
        //picGameBoard.ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 255));
        picGameBoard.GdkWindow.Clear();
        Cairo.Context ct = Gdk.CairoHelper.Create(picGameBoard.GdkWindow);
        for (int i = 1; i < 12; i++)
        {
            //top 
            ct.SetSourceSurface(images[6], i * 35, 0);
            ct.Paint();
            //bottom
            ct.SetSourceSurface(images[6], i * 35, 11 * 35);
            ct.Paint();

        }
        for (int i = 0; i < 12; i++)
        {
            //left
            ct.SetSourceSurface(images[6], 0, i * 35);
            ct.Paint();
            //right
            ct.SetSourceSurface(images[6], 11 * 35, i * 35);
            ct.Paint();
        }
        //snake head position
        snakeXY[0].x = 5;
        snakeXY[0].y = 5;
        //snake body position
        snakeXY[1].x = 5;
        snakeXY[1].y = 6;
        snakeXY[2].x = 5;
        snakeXY[2].y = 7;
        //snake head
        ct.SetSourceSurface(images[5], snakeXY[0].x * 35, snakeXY[0].y * 35);
        ct.Paint();
        //snake body
        ct.SetSourceSurface(images[4], snakeXY[1].x * 35, snakeXY[1].y * 35);
        ct.Paint();
        ct.SetSourceSurface(images[4], snakeXY[2].x * 35, snakeXY[2].y * 35);
        ct.Paint();

        //snake's head position on the gameboard -> x=5,y=5
        gameBoardField[5, 5] = GameBoardFields.Snake;
        //snake's body parts position on the gameboard
        gameBoardField[5, 6] = GameBoardFields.Snake;
        gameBoardField[5, 7] = GameBoardFields.Snake;

        // direction
        direction = Directions.Up;

        // snake's length
        snakeLength = 3;

        bonusCount = 0;
        if (bonusCount == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                Bonus();
                bonusCount++;
            }
        }
        ct.GetTarget().Dispose();
    }

    private void Bonus()
    {
        int x, y;
        Cairo.Context ct = Gdk.CairoHelper.Create(picGameBoard.GdkWindow);
        var imgIndex = rand.Next(0, 4);
        do
        {
            x = rand.Next(1, 10);
            y = rand.Next(1, 10);
        } while (gameBoardField[x, y] != GameBoardFields.Free);

        /*
        gameBoardField[x, y] = GameBoardFields.Bonus;
        */
        ct.SetSourceSurface(images[imgIndex], x * 35, y * 35);
        ct.Paint();
        return;
    }
    [GLib.ConnectBefore]
    protected void OnKeyPressed(object o, KeyPressEventArgs args)
    {
        Console.WriteLine(args.Event.Key);
    }

    void OnExpose(object o, ExposeEventArgs args)
    {

    }
}
