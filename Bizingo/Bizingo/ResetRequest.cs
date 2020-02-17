using System;
namespace Bizingo
{
    public partial class ResetRequest : Gtk.Window
    {
        Action<int> resetJogo;

        public ResetRequest(Action<int> ResetJogo) :
                base(Gtk.WindowType.Toplevel)
        {
            resetJogo = ResetJogo;
            this.Build();
        }

        protected void OnButton2Clicked(object sender, EventArgs e)
        {
            //negar
            resetJogo(0);
            this.Destroy();
        }

        protected void OnButton3Clicked(object sender, EventArgs e)
        {
            //aceitar
            resetJogo(1);
            this.Destroy();
        }

        protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
        {
            this.Destroy();
        }
    }
}
