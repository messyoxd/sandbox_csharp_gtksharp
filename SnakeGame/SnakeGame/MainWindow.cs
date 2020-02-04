using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cairo;
using Gdk;
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
    uint timeTick;
    bool isGameOver;
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
        isGameOver = false;
        picGameBoard.ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 255));

        //snake head position
        snakeXY[0].x = 5;
        snakeXY[0].y = 5;
        //snake body position
        snakeXY[1].x = 5;
        snakeXY[1].y = 6;
        snakeXY[2].x = 5;
        snakeXY[2].y = 7;

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

        timeTick = 500;

        ClockStart();

    }



    void ClockStart()
    {
        /*
        if (!isStarted)
        {
            BoardDrawing();
            isStarted = true;
        }*/
        GLib.Timeout.Add(timeTick, new GLib.TimeoutHandler(DrawBoard));

        //GLib.Timeout.Add(1000, new GLib.TimeoutHandler(Bonus));
    }

    bool DrawBoard()
    {
        if (!isGameOver)
        {
            Cairo.Context ct = Gdk.CairoHelper.Create(picGameBoard.GdkWindow);

            // fazer a ultima parte do corpo da snake desaparecer
            ct.Rectangle(snakeXY[snakeLength - 1].x * 35, snakeXY[snakeLength - 1].y * 35, 35, 35);
            //ct.StrokePreserve();
            ct.SetSourceRGB(1, 1, 1);
            ct.Fill();
            gameBoardField[snakeXY[snakeLength - 1].x, snakeXY[snakeLength - 1].y] = GameBoardFields.Free;

            //move snake's body parts
            for (int i = snakeLength; i >= 1; i--)
            {
                snakeXY[i].x = snakeXY[i - 1].x;
                snakeXY[i].y = snakeXY[i - 1].y;
            }

            //draw new body part position
            ct.SetSourceSurface(images[4], snakeXY[0].x * 35, snakeXY[0].y * 35);
            ct.Paint();

            if (direction == Directions.Up)
            {
                snakeXY[0].y = snakeXY[0].y - 1;
            }
            else if (direction == Directions.Down)
            {
                snakeXY[0].y = snakeXY[0].y + 1;
            }
            else if (direction == Directions.Left)
            {
                snakeXY[0].x = snakeXY[0].x - 1;
            }
            else if (direction == Directions.Right)
            {
                snakeXY[0].x = snakeXY[0].x + 1;
            }
            else
            {
                Environment.Exit(0);
            }
            Console.WriteLine($"x: {snakeXY[0].x} y: {snakeXY[0].y}");
            //check if it hits a wall
            if (snakeXY[0].x > 10 || snakeXY[0].x < 1 || snakeXY[0].y > 10 || snakeXY[0].y < 1)
            {
                GameOver("Bateu na borda");
            }
            // if snake head hits its body 
            else if (gameBoardField[snakeXY[0].x, snakeXY[0].y] == GameBoardFields.Snake)
            {

                GameOver("Bateu no corpo");
            }
            // if snake eats the bonus
            else if (gameBoardField[snakeXY[0].x, snakeXY[0].y] == GameBoardFields.Bonus)
            {
                ct.SetSourceSurface(images[4], snakeXY[snakeLength].x * 35, snakeXY[snakeLength].y * 35);
                ct.Paint();

                //draw snake's head
                ct.SetSourceSurface(images[5], snakeXY[0].x * 35, snakeXY[0].y * 35);
                ct.Paint();

                gameBoardField[snakeXY[snakeLength].x, snakeXY[snakeLength].y] = GameBoardFields.Snake;
                snakeLength++;

                if (snakeLength < 96)
                    Bonus();
                timeTick -= 100;
                Console.WriteLine($"Snake score: {snakeLength - 3}");
            }
            else
            {
                //draw snake's head
                ct.SetSourceSurface(images[5], snakeXY[0].x * 35, snakeXY[0].y * 35);
                ct.Paint();
            }
            gameBoardField[snakeXY[0].x, snakeXY[0].y] = GameBoardFields.Snake;
        }
        return true;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    private bool Bonus()
    {
        int x, y;
        Cairo.Context ct = Gdk.CairoHelper.Create(picGameBoard.GdkWindow);
        var imgIndex = rand.Next(0, 4);
        do
        {
            x = rand.Next(1, 11);
            y = rand.Next(1, 11);
        } while (gameBoardField[x, y] != GameBoardFields.Free);


        gameBoardField[x, y] = GameBoardFields.Bonus;

        ct.SetSourceSurface(images[imgIndex], x * 35, y * 35);
        ct.Paint();
        return true;
    }
    [GLib.ConnectBefore]
    protected void OnKeyPressed(object o, KeyPressEventArgs args)
    {
        switch (args.Event.Key)
        {
            case Gdk.Key.Up:
                direction = Directions.Up;
                break;
            case Gdk.Key.Down:
                direction = Directions.Down;
                break;
            case Gdk.Key.Left:
                direction = Directions.Left;
                break;
            case Gdk.Key.Right:
                direction = Directions.Right;
                break;
        }
    }

    void OnExpose(object o, ExposeEventArgs args)
    {
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

        //snake head
        ct.SetSourceSurface(images[5], snakeXY[0].x * 35, snakeXY[0].y * 35);
        ct.Paint();
        //snake body
        ct.SetSourceSurface(images[4], snakeXY[1].x * 35, snakeXY[1].y * 35);
        ct.Paint();
        ct.SetSourceSurface(images[4], snakeXY[2].x * 35, snakeXY[2].y * 35);
        ct.Paint();
        for (int i = 0; i < 4; i++)
        {
            Bonus();
            bonusCount++;
        }
        /*
        if (bonusCount == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                Bonus();
                bonusCount++;
            }
        }*/
        ct.GetTarget().Dispose();
    }

    public void GameOver(string causaDaMorte)
    {

        isGameOver = true;
        Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal | DialogFlags.DestroyWithParent,
                                  MessageType.Info,
                                  ButtonsType.Ok,
                                  $"Perdeu{Environment.NewLine}Causa da morte: {causaDaMorte}");
        dialog.Run();
        dialog.Hide();
    }
}