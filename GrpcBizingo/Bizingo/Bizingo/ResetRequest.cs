using System;
namespace Bizingo
{
    public partial class ResetRequest : Gtk.Window
    {
        Action<bool> _mandarDecisaoReset;

        public ResetRequest(Action<bool> MandarDecisaoReset, string adversarioNome) :
                base(Gtk.WindowType.Toplevel)
        {
            _mandarDecisaoReset = MandarDecisaoReset;
            this.Build();
            lTexto.Text = $"O adversario {adversarioNome} pediu para reiniciar a partida!";
        }

        protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
        {
            _mandarDecisaoReset(false);
            this.Destroy();
        }

        protected void OnBRecusarClicked(object sender, EventArgs e)
        {
            _mandarDecisaoReset(false);
            this.Destroy();
        }

        protected void OnBAceitarClicked(object sender, EventArgs e)
        {
            _mandarDecisaoReset(true);
            this.Destroy();
        }
    }
}
