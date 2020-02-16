using System;
using Bizingo;
using Gdk;
using Gtk;

public partial class MainWindow : Gtk.Window
{

    private string localIp;
    private int localPort;
    private string remoteIp;
    private int remotePort;

    private BizingoTabuleiro b;


    public void SetLocalIp(string LocalIp)
    {
        localIp = LocalIp;
    }

    public void SetLocalPort(int LocalPort)
    {
        localPort = LocalPort;
    }

    public void SetRemoteIp(string RemoteIp)
    {
        remoteIp = RemoteIp;
    }

    public void SetRemotelPort(int RemotePort)
    {
        remotePort = RemotePort;
    }

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    public void IniciandoJogo(int portaLocal)
    {
        b = new BizingoTabuleiro(portaLocal,"", 1);
        b.Show();
        this.Destroy();
    }

    public void EntrandoEmJogo(int portaRemota, string endereco)
    {
        b = new BizingoTabuleiro(portaRemota, endereco, 2);
        b.Show();
        this.Destroy();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnCriarJogo(object sender, EventArgs e)
    {
        CriarJogoWindow cjw = new CriarJogoWindow(IniciandoJogo);
        cjw.Show();
    }

    protected void OnConectar(object sender, EventArgs e)
    {
        ConectarComPlayer ccp = new ConectarComPlayer(EntrandoEmJogo);
        ccp.Show();
    }

    protected void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        int x, y;
        ModifierType state;
        args.Event.Window.GetPointer(out x, out y, out state);
        Console.WriteLine($"x: {x} y:{y} state: {state}");
    }
}
