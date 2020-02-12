using System;
using Bizingo;
using Gdk;
using Gtk;

public partial class MainWindow : Gtk.Window
{

    private string localIp;
    private int localPort;

    public void SetLocalIp(string LocalIp)
    {
        localIp = LocalIp;
    }

    public void SetLocalPort(int LocalPort)
    {
        localPort = LocalPort;
    }

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnCriarJogo(object sender, EventArgs e)
    {
        CriarJogoWindow cjw = new CriarJogoWindow(this);
        cjw.Show();
    }

    protected void OnConectar(object sender, EventArgs e)
    {
    }

    protected void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        int x, y;
        ModifierType state;
        args.Event.Window.GetPointer(out x, out y, out state);
        Console.WriteLine($"x: {x} y:{y} state: {state}");
    }
}
