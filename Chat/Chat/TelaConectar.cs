using System;

namespace Chat
{
    public partial class TelaConectar : Gtk.Window
    {
        Chat.ChatSocketHandler chat;
        Action<string, string, string, string> setChat;
        public TelaConectar(string v, Action<string, string, string, string> SetChat, Chat.ChatSocketHandler ct) :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            this.Title = v;
            eIpRemoto.Text = string.Empty;
            ePortaLocal.Text = string.Empty;
            ePortaRemota.Text = string.Empty;
            eIpLocal.Text = Chat.ChatSocketHandler.GetLocalIP();
            setChat = SetChat;
            chat = ct;
            if (chat != null)
            {
                eIpRemoto.Text = chat.remoteEP.Address.ToString();
                ePortaRemota.Text = chat.remoteEP.Port.ToString();
                ePortaLocal.Text = chat.localEP.Port.ToString();
            }
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

        protected void OnCancelar(object sender, EventArgs e)
        {
            this.Destroy();
        }

    }
}
