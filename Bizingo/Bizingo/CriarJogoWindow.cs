using System;
using Gtk;

namespace Bizingo
{
    public partial class CriarJogoWindow : Gtk.Window
    {
        Action<int> iniciandoJogo;
        public CriarJogoWindow(Action<int> IniciandoJogo) :
                base(Gtk.WindowType.Toplevel)
        {
            iniciandoJogo = IniciandoJogo;
            this.Build();
        }

        protected void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }

        protected void OnBtnCancelarClicked(object sender, EventArgs e)
        {
            this.Destroy();
        }

        protected void OnBtnCriarClicked(object sender, EventArgs e)
        {
            var porta = ePortaLocal.Text;
            iniciandoJogo(int.Parse(porta));
            this.Destroy();
        }
    }
}
