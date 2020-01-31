using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    UPDAsynchoronousChatServer.UDPAsynchronousChatServer mUDPChatServer;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        mUDPChatServer = new UPDAsynchoronousChatServer.UDPAsynchronousChatServer();
        mUDPChatServer.StartReceivingData();
        Build();


    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}
