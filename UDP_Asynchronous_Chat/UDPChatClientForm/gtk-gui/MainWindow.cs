
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.Fixed fixed1;

	private global::Gtk.Label label3;

	private global::Gtk.Entry tbBroadcastText;

	private global::Gtk.Entry tbRemotePort;

	private global::Gtk.Entry tbLocalPort;

	private global::Gtk.Label label2;

	private global::Gtk.Label label1;

	private global::Gtk.Button btnSendBroadcast;

	private global::Gtk.Label label4;

	private global::Gtk.ScrolledWindow GtkScrolledWindow;

	private global::Gtk.Entry tbSendMessage;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("Client");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.fixed1 = new global::Gtk.Fixed();
		this.fixed1.Name = "fixed1";
		this.fixed1.HasWindow = false;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.label3 = new global::Gtk.Label();
		this.label3.WidthRequest = 107;
		this.label3.Name = "label3";
		this.label3.LabelProp = global::Mono.Unix.Catalog.GetString("Broadcast Text:");
		this.fixed1.Add(this.label3);
		global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label3]));
		w1.X = 5;
		w1.Y = 112;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.tbBroadcastText = new global::Gtk.Entry();
		this.tbBroadcastText.WidthRequest = 209;
		this.tbBroadcastText.HeightRequest = 70;
		this.tbBroadcastText.CanFocus = true;
		this.tbBroadcastText.Name = "tbBroadcastText";
		this.tbBroadcastText.Text = global::Mono.Unix.Catalog.GetString("<DISCOVER>");
		this.tbBroadcastText.IsEditable = true;
		this.tbBroadcastText.InvisibleChar = '•';
		this.fixed1.Add(this.tbBroadcastText);
		global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.tbBroadcastText]));
		w2.X = 114;
		w2.Y = 111;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.tbRemotePort = new global::Gtk.Entry();
		this.tbRemotePort.WidthRequest = 209;
		this.tbRemotePort.CanFocus = true;
		this.tbRemotePort.Name = "tbRemotePort";
		this.tbRemotePort.Text = global::Mono.Unix.Catalog.GetString("23000");
		this.tbRemotePort.IsEditable = true;
		this.tbRemotePort.InvisibleChar = '•';
		this.fixed1.Add(this.tbRemotePort);
		global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.tbRemotePort]));
		w3.X = 114;
		w3.Y = 70;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.tbLocalPort = new global::Gtk.Entry();
		this.tbLocalPort.WidthRequest = 209;
		this.tbLocalPort.CanFocus = true;
		this.tbLocalPort.Name = "tbLocalPort";
		this.tbLocalPort.Text = global::Mono.Unix.Catalog.GetString("23001");
		this.tbLocalPort.IsEditable = true;
		this.tbLocalPort.InvisibleChar = '•';
		this.fixed1.Add(this.tbLocalPort);
		global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.tbLocalPort]));
		w4.X = 114;
		w4.Y = 26;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.label2 = new global::Gtk.Label();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString("Remote Port:");
		this.fixed1.Add(this.label2);
		global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label2]));
		w5.X = 20;
		w5.Y = 75;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.label1 = new global::Gtk.Label();
		this.label1.Name = "label1";
		this.label1.LabelProp = global::Mono.Unix.Catalog.GetString("Local Port:");
		this.fixed1.Add(this.label1);
		global::Gtk.Fixed.FixedChild w6 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label1]));
		w6.X = 36;
		w6.Y = 31;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.btnSendBroadcast = new global::Gtk.Button();
		this.btnSendBroadcast.WidthRequest = 114;
		this.btnSendBroadcast.CanFocus = true;
		this.btnSendBroadcast.Name = "btnSendBroadcast";
		this.btnSendBroadcast.UseUnderline = true;
		this.btnSendBroadcast.Label = global::Mono.Unix.Catalog.GetString("Broadcast");
		this.fixed1.Add(this.btnSendBroadcast);
		global::Gtk.Fixed.FixedChild w7 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.btnSendBroadcast]));
		w7.X = 209;
		w7.Y = 213;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.label4 = new global::Gtk.Label();
		this.label4.WidthRequest = 209;
		this.label4.Name = "label4";
		this.label4.LabelProp = global::Mono.Unix.Catalog.GetString("Message Text:");
		this.fixed1.Add(this.label4);
		global::Gtk.Fixed.FixedChild w8 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label4]));
		w8.X = 428;
		w8.Y = 85;
		// Container child fixed1.Gtk.Fixed+FixedChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
		this.GtkScrolledWindow.WidthRequest = 279;
		this.GtkScrolledWindow.HeightRequest = 154;
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		global::Gtk.Viewport w9 = new global::Gtk.Viewport();
		w9.ShadowType = ((global::Gtk.ShadowType)(0));
		// Container child GtkViewport.Gtk.Container+ContainerChild
		this.tbSendMessage = new global::Gtk.Entry();
		this.tbSendMessage.WidthRequest = 267;
		this.tbSendMessage.HeightRequest = 148;
		this.tbSendMessage.CanFocus = true;
		this.tbSendMessage.Name = "tbSendMessage";
		this.tbSendMessage.IsEditable = true;
		this.tbSendMessage.MaxLength = 1000;
		this.tbSendMessage.InvisibleChar = '•';
		w9.Add(this.tbSendMessage);
		this.GtkScrolledWindow.Add(w9);
		this.fixed1.Add(this.GtkScrolledWindow);
		global::Gtk.Fixed.FixedChild w12 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.GtkScrolledWindow]));
		w12.X = 428;
		w12.Y = 111;
		this.Add(this.fixed1);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 718;
		this.DefaultHeight = 272;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.btnSendBroadcast.Clicked += new global::System.EventHandler(this.btnSendBroadcast_Click);
	}
}
