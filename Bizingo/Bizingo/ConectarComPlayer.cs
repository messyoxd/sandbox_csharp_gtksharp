using System;
using Gtk;

namespace Bizingo
{
    public partial class ConectarComPlayer : Gtk.Window
    {
        Action<int, string> entrandoJogo;
        public ConectarComPlayer(Action<int, string> EntrandoJogo) :
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

            var ip = eIpRemoto.Text;
            var porta = ePortaRemota.Text;
            entrandoJogo(int.Parse(porta), ip);
            this.Destroy();
        }
    }
}
