using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnOpen(object sender, EventArgs e)
    {
        int w, h;
        this.GetDefaultSize(out w, out h);
        this.Resize(w, h);

        logTextView.Buffer.Text = "";

        FileChooserDialog chooser = new FileChooserDialog(
        "Escolha o arquivo de log para vê-lo...",
        this,
        FileChooserAction.Open,
        "Cancel", ResponseType.Cancel,
        "Open", ResponseType.Accept
        );
        if( chooser.Run() == (int)ResponseType.Accept)
        {
            System.IO.StreamReader file = System.IO.File.OpenText(chooser.Filename);
            logTextView.Buffer.Text = file.ReadToEnd();
            this.Title = "Messyo's Log Viewer -- "+chooser.Filename.ToString();
            this.Resize(600, 800);
            file.Close();
        }
        chooser.Destroy();
    }

    protected void OnClose(object sender, EventArgs e)
    {
        int w, h;
        this.GetDefaultSize(out w, out h);
        this.Resize(w, h);

        logTextView.Buffer.Text = "";

        this.Title = "Messyo's Log Viewer";
    }

    protected void OnExit(object sender, EventArgs e)
    {
        Application.Quit();
    }

    protected void OnHelp(object sender, EventArgs e)
    {
    }

    protected void OnAbout(object sender, EventArgs e)
    {
        AboutDialog dia = new AboutDialog();

        dia.ProgramName = "Messyo's Log Viewer";
        dia.Version = "3";
        dia.Run();
        dia.Destroy();
    }
}
