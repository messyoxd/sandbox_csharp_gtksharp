using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    UDPAsynchronousChatServer.UDPAsynchronousChatServer mUDPChatServer;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        mUDPChatServer = new UDPAsynchronousChatServer.UDPAsynchronousChatServer();
        mUDPChatServer.StartReceivingData();
        Build();


    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}
