using System;

namespace Chat
{
    public partial class TelaConectar : Gtk.Window
    {
        Action<string, string, string, string> setChat;
        public TelaConectar(string v,Action<string, string, string, string> SetChat) :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            this.Title = v;
            eIpRemoto.Text = string.Empty;
            ePortaLocal.Text = string.Empty;
            ePortaRemota.Text = string.Empty;
            eIpLocal.Text = Chat.ChatSocketHandler.GetLocalIP();
            setChat = SetChat;
        }

        protected void OnOkClicked(object sender, EventArgs e)
        {
            string ipRemoto = eIpRemoto.Text;
            string portaRemota = ePortaRemota.Text;
            string portaLocal = ePortaLocal.Text;
            string ipLocal = eIpLocal.Text;
            setChat(ipLocal, ipRemoto, portaRemota, portaLocal);
            this.Destroy();
        }
    }
}
