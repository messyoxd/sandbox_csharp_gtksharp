using System;
using Gtk;

namespace Bizingo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            BizingoTabuleiro win = new BizingoTabuleiro();
            win.Show();
            Application.Run();
        }
    }
}
