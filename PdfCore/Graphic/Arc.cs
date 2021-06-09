using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCore.Graphic
{
    public class Arc : Graphics
    {
        public override void SetLocation(double x, double y) { SetLocation(new Point(x, y)); }
        public override void SetLocation(Point newPoint)
        {
            double dx = newPoint.X - Location.X; double dy = newPoint.Y - Location.Y; this.Move(dx, dy);
        }

        public override void SetCenter(double x, double y) { SetCenter(new Point(x, y)); }
        public override void SetCenter(Point newPoint)
        {
            double dx = newPoint.X - Center.X;
            double dy = newPoint.Y - Center.Y;
            this.Move(dx, dy);
        }
        public void MirrorX()
        {
            CenterCircul = new Point(CenterCircul.X, -CenterCircul.Y);
            AnglStart = Math.PI * 2 - AnglStart;
            AnglEnd = Math.PI * 2 - AnglEnd;
        }
        public void MirrorY()
        {
            CenterCircul = new Point(-CenterCircul.X, CenterCircul.Y);
            AnglStart = Math.PI - AnglStart;
            AnglEnd = Math.PI - AnglEnd;
        }
        public override dynamic Clone() { return new Arc(CenterCircul, Radius, AnglStart, AnglEnd); }
        public void Reverse() { double temp = AnglStart; AnglStart = AnglEnd; AnglEnd = temp; }
        public Arc(double x, double y, double radius, double angl_start, double angl_end)
        {

            CenterCircul = new Point(x, y);
            Radius = radius;
            AnglStart = angl_start;
            AnglEnd = angl_end;
        }
        public Arc(Point center, double radius, double angl_start, double angl_end)
        {
            CenterCircul = new Point(center.X, center.Y);
            Radius = radius;
            AnglStart = angl_start;
            AnglEnd = angl_end;
        }
        public void SetStartPoint(Point newPoint) { double dx = newPoint.X - Point.X; double dy = newPoint.Y - Point.Y; Move(dx, dy); }
        public Point Point { get { return new Point(CenterCircul.X + Radius * Math.Round(Math.Cos(AnglStart), 14), CenterCircul.Y + Radius * Math.Round(Math.Sin(AnglStart), 14)); } }
        public double X { get { return Point.X; } }
        public double Y { get { return Point.Y; } }
        public Point EndPoint { get { return new Point(CenterCircul.X + Radius * Math.Round(Math.Cos(AnglEnd), 14), CenterCircul.Y + Radius * Math.Round(Math.Sin(AnglEnd), 14)); } }

        public double Radius { get; set; }
        public Point CenterCircul { get; set; }
        public double AnglStart { get; set; }
        public double AnglEnd { get; set; }

        public override Point Location
        {
            get
            {
                return new Point(Top, Left);
            }
        }
        public override Point Center { get { return CenterCircul; } }
        public override double Height { get { return Down - Top; } }
        public override double Width { get { return Right - Left; } }
        public override void Move(double dx, double dy)
        {
            CenterCircul = new Point(CenterCircul.X + dx, CenterCircul.Y + dy);
        }
        public override double Top
        {
            get
            {
                for (int i = -10; i <= 10; i++)
                {
                    if (i != 0 && (AnglStart < AnglEnd) ? i * 1.5 * Math.PI >= AnglStart && i * 1.5 * Math.PI <= AnglEnd : i * 1.5 * Math.PI <= AnglStart && i * 1.5 * Math.PI >= AnglEnd)
                    {
                        return CenterCircul.Y - Radius;
                    }
                }
                return Math.Min(CenterCircul.Y + Radius * Math.Round(Math.Sin(AnglStart), 14), CenterCircul.Y + Radius * Math.Round(Math.Sin(AnglEnd), 14));
            }
        }
        public override double Down
        {
            get
            {
                for (int i = -10; i <= 10; i++)
                {
                    if (i != 0 && (AnglStart < AnglEnd) ? i * Math.PI / 2 >= AnglStart && i * Math.PI / 2 <= AnglEnd : i * Math.PI / 2 <= AnglStart && i * Math.PI / 2 >= AnglEnd)
                    {
                        return CenterCircul.Y + Radius;
                    }
                }
                return Math.Max(CenterCircul.Y + Radius * Math.Round(Math.Sin(AnglStart), 14), CenterCircul.Y + Radius * Math.Round(Math.Sin(AnglEnd), 14));

            }
        }
        public override double Left
        {
            get
            {
                for (int i = -10; i <= 10; i++)
                {
                    if (i != 0 && (AnglStart < AnglEnd) ? i * Math.PI >= AnglStart && i * Math.PI <= AnglEnd : i * Math.PI <= AnglStart && i * Math.PI >= AnglEnd)
                    {
                        return CenterCircul.X - Radius;
                    }
                }
                return Math.Min(CenterCircul.X + Radius * Math.Round(Math.Cos(AnglStart), 14), CenterCircul.X + Radius * Math.Round(Math.Cos(AnglEnd), 14));
            }
        }
        public override double Right
        {
            get
            {
                for (int i = -10; i <= 10; i++)
                {
                    if ((AnglStart < AnglEnd) ? i * 2 * Math.PI >= AnglStart && i * 2 * Math.PI <= AnglEnd : i * 2 * Math.PI <= AnglStart && i * 2 * Math.PI >= AnglEnd)
                    {
                        return CenterCircul.X + Radius;
                    }
                }
                return Math.Max(CenterCircul.X + Radius * Math.Round(Math.Cos(AnglStart), 14), CenterCircul.X + Radius * Math.Round(Math.Cos(AnglEnd), 14));
            }
        }

        public override void Rotate(double ang)
        {
            AnglEnd += ang;
            AnglStart += ang;
        }
        public override void Scale(double x, double y)
        {
            Radius *= x;
        }
        public double Stroke { get; set; } = 1;
        public PdfSharpCore.Drawing.XColor Color { get; set; } = PdfSharpCore.Drawing.XColors.Black;
        public override void ToPDFSharp(PdfSharpCore.Drawing.XGraphics contur)
        {
            PdfSharpCore.Drawing.XPen pen = new PdfSharpCore.Drawing.XPen(Color, Stroke)
            {
                DashStyle = PdfSharpCore.Drawing.XDashStyle.Custom
            };
            XGraphicsPath path = new XGraphicsPath();
            ToPDFSharp(ref path);
            contur.DrawPath(pen, path);
        }
        public void ToPDFSharp(ref XGraphicsPath path)
        {
            double count = Math.Max(Math.Abs(Math.Ceiling((AnglEnd - AnglStart) / (Math.PI / 6))), 2);
            double workangl = (AnglEnd - AnglStart) / count;
            double L = XUnit.FromMillimeter(4 * Math.Round(Math.Tan(workangl / 4), 14) * Radius / 3);
            double R = XUnit.FromMillimeter(Radius);
            List<XPoint> startpoints = new List<XPoint>{
                new XPoint(R * Math.Round(Math.Cos(AnglStart), 14) - L* Math.Round(Math.Sin(AnglStart), 14), R * Math.Round(Math.Sin(AnglStart), 14) + L * Math.Round(Math.Cos(AnglStart), 14)),
                new XPoint(R * Math.Round(Math.Cos(AnglStart+workangl), 14) - (-L)* Math.Round(Math.Sin(AnglStart+workangl), 14), R * Math.Round(Math.Sin(AnglStart+workangl), 14) + (-L) * Math.Round(Math.Cos(AnglStart+workangl), 14)),
                new XPoint(R * Math.Round(Math.Cos(AnglStart+workangl), 14), R * Math.Round(Math.Sin(AnglStart+workangl), 14))};

            List<XPoint> points = new List<XPoint>() { new XPoint(R * Math.Round(Math.Cos(AnglStart), 14), R * Math.Round(Math.Sin(AnglStart), 14)) };
            for (int i = 0; i < count; i++)
            {
                foreach (var point in startpoints)
                {
                    points.Add(point);
                }
                for (int j = 0; j < startpoints.Count; j++) { startpoints[j] = new XPoint(startpoints[j].X * Math.Round(Math.Cos(workangl), 14) - startpoints[j].Y * Math.Round(Math.Sin(workangl), 14), startpoints[j].X * Math.Round(Math.Sin(workangl), 14) + startpoints[j].Y * Math.Round(Math.Cos(workangl), 14)); }

            }
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new XPoint(points[i].X + XUnit.FromMillimeter(CenterCircul.X), points[i].Y + XUnit.FromMillimeter(CenterCircul.Y));
            }
            path.AddBeziers(points.ToArray());
        }
        public override double Squeare { get { return 0; } }
    }
}
