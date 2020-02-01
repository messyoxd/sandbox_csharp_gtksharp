using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    UDPAsynchronousChatServer.UDPAsynchronousChatClient mChatClient;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void btnSendBroadcast_Click(object sender, EventArgs e)
    {
        if(mChatClient == null)
        {
            int.TryParse(tbLocalPort.Text, out var localPort);
            int.TryParse(tbRemotePort.Text, out var remotePort);
            mChatClient = new UDPAsynchronousChatServer.UDPAsynchronousChatClient(localPort, remotePort);
        }
        mChatClient.SendBroadcast(tbBroadcastText.Text);
    }
}
