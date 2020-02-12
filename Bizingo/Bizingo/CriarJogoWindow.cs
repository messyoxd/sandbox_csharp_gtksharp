using System;
namespace Bizingo
{
    public partial class CriarJogoWindow : Gtk.Window
    {
        public CriarJogoWindow(MainWindow mw) :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }
    }
}
