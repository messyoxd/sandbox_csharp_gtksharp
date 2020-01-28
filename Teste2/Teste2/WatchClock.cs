using System;
using Cairo;
namespace Teste2
{
    public class WatchClock
    {
        public Antialias universalAntiA = Antialias.None;
        public void DrawRing(Gdk.Window drawingArea)
        {
            int clockAmount = 0;
            double rotateAmount = 0;
            while (clockAmount < 12)
            {
                Cairo.Context ring = Gdk.CairoHelper.Create(drawingArea);

                PointD p1, p2;

                p1 = new PointD(85, 0);
                p2 = new PointD(95, 0);

                ring.Antialias = universalAntiA;
                ring.Translate(100, 100);
                ring.Rotate(rotateAmount);
                ring.LineWidth = 4;
                ring.MoveTo(p1);
                ring.LineTo(p2);
                ring.LineCap = LineCap.Round;

                ring.SetSourceColor(new Cairo.Color(0.95, 0.95, 0.95));
                ring.Stroke();

                ring.GetTarget().Dispose();
                ring.Dispose();

                clockAmount++;
                rotateAmount+=(Math.PI*2)/12;
            }

        }

        public void HourLine(Gdk.Window drawingArea)
        {
            Cairo.Context ring = Gdk.CairoHelper.Create(drawingArea);

            PointD p1, p2;

            p1 = new PointD(0, 0);
            p2 = new PointD(0, -40);

            ring.Antialias = universalAntiA;
            ring.Translate(100, 100);
            ring.Rotate(CalculateHourRadians());
            ring.LineWidth = 8;
            ring.MoveTo(p1);
            ring.LineTo(p2);
            ring.LineCap = LineCap.Round;

            ring.SetSourceColor(new Cairo.Color(0.95, 0.95, 0.95));
            ring.Stroke();

            ring.GetTarget().Dispose();
            ring.Dispose();

        }
        public void MinuteLine(Gdk.Window drawingArea)
        {
            Cairo.Context ring = Gdk.CairoHelper.Create(drawingArea);

            PointD p1, p2;

            p1 = new PointD(0, 0);
            p2 = new PointD(0, -80);

            ring.Antialias = universalAntiA;
            ring.Translate(100, 100);
            ring.Rotate(CalculateMinuteRadians());
            ring.LineWidth = 4;
            ring.MoveTo(p1);
            ring.LineTo(p2);
            ring.LineCap = LineCap.Round;

            ring.SetSourceColor(new Cairo.Color(0.5, 0.1, 0.1));
            ring.Stroke();

            ring.GetTarget().Dispose();
            ring.Dispose();
        }
        public void SecondLine(Gdk.Window drawingArea)
        {
            Cairo.Context ring = Gdk.CairoHelper.Create(drawingArea);

            PointD p1, p2;

            p1 = new PointD(0, 0);
            p2 = new PointD(0, -70);

            ring.Antialias = universalAntiA;
            ring.Translate(100, 100);
            ring.Rotate(CalculateSecondRadians());
            ring.LineWidth = 4;
            ring.MoveTo(p1);
            ring.LineTo(p2);
            ring.LineCap = LineCap.Round;

            ring.SetSourceColor(new Cairo.Color(0.5, 0.5, 0.5));
            ring.Stroke();

            ring.GetTarget().Dispose();
            ring.Dispose();
        }
        public double CalculateHourRadians()
        {
            double radian = Math.PI * 2;
            int minutes;
            if(DateTime.Now.Hour > 12)
            {
                minutes = (DateTime.Now.Hour - 12) * 60+(DateTime.Now.Minute);
            }
            else
            {
                minutes = DateTime.Now.Hour * 60 + (DateTime.Now.Minute); 
            }
            radian = radian * (0.001388889 * (double)minutes);
            return radian;
        }
        public double CalculateMinuteRadians()
        {
            return (Math.PI * 2)*((double)DateTime.Now.Minute * 0.0166666666667);
        }
        public double CalculateSecondRadians()
        {
            return (Math.PI * 2) * ((double)DateTime.Now.Second * 0.0166666666667);
        }

    }
}
