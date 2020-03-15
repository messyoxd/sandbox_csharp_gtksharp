using System;
using Gtk;

namespace Bizingo
{
    public partial class ConectarComPlayer : Gtk.Window
    {
        Action<int, int, string, string, string> entrandoJogo;
        public ConectarComPlayer(Action<int, int, string, string, string> EntrandoJogo) :
                base(Gtk.WindowType.Toplevel)
        {
            entrandoJogo = EntrandoJogo;
            this.Build();
        }

        protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }

        protected void OnButton2Clicked(object sender, EventArgs e)
        {
            this.Destroy();
        }

        protected void OnButton3Clicked(object sender, EventArgs e)
        {

            var ipRemoto = eIpRemoto.Text;
            var iplocal = eIpLocal.Text;
            var portaRemota = ePortaRemota.Text;
            var apelido = eApelido.Text;
            var portaLocal = ePortaLocal.Text;
            entrandoJogo(int.Parse(portaRemota), int.Parse(portaLocal), ipRemoto, iplocal, apelido);
            this.Destroy();
            /*
            if (
            ip.Length > 0 &&
            portaRemota.Length > 0 &&
            apelido.Length > 0 &&
            portaLocal.Length > 0)
            {
                if (portaRemota.Equals(portaLocal))
                {
                    Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal,
                                  MessageType.Error,
                                  ButtonsType.Ok,
                                  "A porta remota não pode ser a mesma porta que a local!");
                    dialog.Run();
                    dialog.Hide();
                }
                else
                {
                    entrandoJogo(int.Parse(portaRemota), int.Parse(portaLocal), ip, apelido);
                    this.Destroy();
                }
            }
            else
            {
                Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal,
                                  MessageType.Error,
                                  ButtonsType.Ok,
                                  "Por favor preencha todos os campos");
                dialog.Run();
                dialog.Hide();
            }*/
        }
    }
}
