
// This file has been generated by the GUI designer. Do not modify.
namespace Bizingo
{
	public partial class ConectarComPlayer
	{
		private global::Gtk.Fixed fixed1;

		private global::Gtk.Label label1;

		private global::Gtk.Label label2;

		private global::Gtk.Entry eIpRemoto;

		private global::Gtk.Entry ePortaRemota;

		private global::Gtk.Button button2;

		private global::Gtk.Button button3;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget Bizingo.ConectarComPlayer
			this.Name = "Bizingo.ConectarComPlayer";
			this.Title = global::Mono.Unix.Catalog.GetString("Conectar com player");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Bizingo.ConectarComPlayer.Gtk.Container+ContainerChild
			this.fixed1 = new global::Gtk.Fixed();
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label1 = new global::Gtk.Label();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString("IP remoto:");
			this.fixed1.Add(this.label1);
			global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label1]));
			w1.X = 89;
			w1.Y = 66;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label2 = new global::Gtk.Label();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString("Porta Remota:");
			this.fixed1.Add(this.label2);
			global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label2]));
			w2.X = 61;
			w2.Y = 104;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.eIpRemoto = new global::Gtk.Entry();
			this.eIpRemoto.CanFocus = true;
			this.eIpRemoto.Name = "eIpRemoto";
			this.eIpRemoto.IsEditable = true;
			this.eIpRemoto.InvisibleChar = '•';
			this.fixed1.Add(this.eIpRemoto);
			global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.eIpRemoto]));
			w3.X = 165;
			w3.Y = 59;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.ePortaRemota = new global::Gtk.Entry();
			this.ePortaRemota.CanFocus = true;
			this.ePortaRemota.Name = "ePortaRemota";
			this.ePortaRemota.IsEditable = true;
			this.ePortaRemota.InvisibleChar = '•';
			this.fixed1.Add(this.ePortaRemota);
			global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.ePortaRemota]));
			w4.X = 165;
			w4.Y = 95;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.button2 = new global::Gtk.Button();
			this.button2.CanFocus = true;
			this.button2.Name = "button2";
			this.button2.UseUnderline = true;
			this.button2.Label = global::Mono.Unix.Catalog.GetString("Cancelar");
			this.fixed1.Add(this.button2);
			global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.button2]));
			w5.X = 55;
			w5.Y = 150;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.button3 = new global::Gtk.Button();
			this.button3.CanFocus = true;
			this.button3.Name = "button3";
			this.button3.UseUnderline = true;
			this.button3.Label = global::Mono.Unix.Catalog.GetString("Conectar");
			this.fixed1.Add(this.button3);
			global::Gtk.Fixed.FixedChild w6 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.button3]));
			w6.X = 248;
			w6.Y = 150;
			this.Add(this.fixed1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 374;
			this.DefaultHeight = 210;
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
			this.button2.Clicked += new global::System.EventHandler(this.OnButton2Clicked);
			this.button3.Clicked += new global::System.EventHandler(this.OnButton3Clicked);
		}
	}
}