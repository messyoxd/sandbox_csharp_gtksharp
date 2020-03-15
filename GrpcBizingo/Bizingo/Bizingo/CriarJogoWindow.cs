using System;
using Gtk;

namespace Bizingo
{
    public partial class CriarJogoWindow : Gtk.Window
    {
        Action<int, string, string> iniciandoJogo;
        public CriarJogoWindow(Action<int, string, string> IniciandoJogo) :
                base(Gtk.WindowType.Toplevel)
        {
            iniciandoJogo = IniciandoJogo;
            this.Build();
        }

        protected void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            this.Destroy();
        }

        protected void OnBtnCancelarClicked(object sender, EventArgs e)
        {
            this.Destroy();
        }

        protected void OnBtnCriarClicked(object sender, EventArgs e)
        {
            var porta = ePortaLocal.Text;
            var apelido = eApelido.Text;
            var ipLocal = eIpLocal.Text;

            if (porta.Length > 0 && apelido.Length > 0)
            {
                iniciandoJogo(int.Parse(porta), ipLocal, apelido);
                this.Destroy();
            }
            else
            {
                Dialog dialog = new MessageDialog(this,
                                  DialogFlags.Modal,
                                  MessageType.Error,
                                  ButtonsType.Ok,
                                  "Por favor preencha a porta e o apelido");
                dialog.Run();
                dialog.Hide();
            }
        }
    }
}
