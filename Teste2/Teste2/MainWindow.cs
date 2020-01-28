using System;
using Gtk;
using Teste2;

public partial class MainWindow : Gtk.Window
{
    WatchClock wc;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        drawingArea1.ModifyBg(StateType.Normal, new Gdk.Color(5, 5, 5));
        wc = new WatchClock();
        ClockStart();
    }

    void ClockStart()
    {
        GLib.Timeout.Add(100, new GLib.TimeoutHandler(Update));
    }

    bool Update()
    {
        drawingArea1.GdkWindow.Clear();
        TimeStamp.Text = DateTime.Now.ToString("HH:mm:ss");
        wc.DrawRing(drawingArea1.GdkWindow);
        wc.HourLine(drawingArea1.GdkWindow);
        wc.MinuteLine(drawingArea1.GdkWindow);
        wc.SecondLine(drawingArea1.GdkWindow);

        return true;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}
