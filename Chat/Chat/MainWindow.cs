using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    Chat.ChatSocketHandler chat;
    List<string> text;
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        text = new List<string>();
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnConectar(object sender, EventArgs e)
    {
        Chat.TelaConectar telaConectar = new Chat.TelaConectar("Conectar com outra instancia", SetChat, chat);
        telaConectar.Show();
    }

    public void UpdateTextList(string str)
    {
        text.Add(str+"\n");
        PutTextOnTextView();
    }

    public void PutTextOnTextView()
    {
        tvMensagemReceber.Buffer.Text = string.Empty;
        text.ForEach((str) =>
        {
            tvMensagemReceber.Buffer.Text += str;
        });
    }

    public void SetChat(string ipLocal, string ipRemoto, string portaRemota, string portaLocal)
    {
        chat = new Chat.ChatSocketHandler(ipLocal, int.Parse(portaLocal), ipRemoto, int.Parse(portaRemota), UpdateTextList);
        Thread t = new Thread(chat.StartReceiving);
    }

    protected void OnSendMessage(object sender, EventArgs e)
    {
        Gtk.TextBuffer message = tvMensagemEnviar.Buffer;
        if (message.Text.Equals(string.Empty) || chat == null)
        {
            Console.WriteLine("caixa de texto vazio ou instancia de chat ainda não criada!");
            return;
        }
        chat.SendMessage(message.Text);
    }

    protected void OnTestCon(object sender, EventArgs e)
    {
        chat.SendMessage("<DISCOVER>");
        Console.WriteLine("mandando um discovery");
    }
}
