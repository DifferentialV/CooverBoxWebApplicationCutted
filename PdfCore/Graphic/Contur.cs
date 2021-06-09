using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCore.Graphic
{
    public class Contur : Graphics
    {
        private readonly List<dynamic> points;
        public Point EndPoint { get { return new Point(points[^1].X, points[^1].Y); } }
        public Contur() { points = new List<dynamic> { }; }
        public void Add(Point point) { points.Add(point); }
        public void Add(Contur contur)
        {
            foreach (dynamic var in contur.points)
            {
                points.Add(var);
            }
        }
        public void Add(Contur contur, char R = 'r')
        {
            if (R == 'r')
            {
                for (int i = contur.Count - 1; i >= 0; i--)
                {
                    if ((i - 1) >= 0 && contur.points[i - 1].GetType() != typeof(Point))
                    {
                        contur.points[i - 1].Reverse();
                        points.Add(contur.points[i - 1]);
                        points.Add(contur.points[i]);
                        i--;
                    }
                    else
                    {
                        points.Add(contur.points[i]);
                    }
                }
            }
            else Add(contur);
        }
        public void AddPoint(double dx, double dy)
        {
            if (points.Count > 0)
                Add(points.Last().X + dx, points.Last().Y + dy);
        }
        public void Add(double x, double y) { points.Add(new Point(x, y)); }
        public void AddArc(Arc arc)
        {
            points.Add(arc);
            points.Add(arc.EndPoint);
        }
        public void AddBezier(Bezier arc)
        {
            points.Add(arc);
            points.Add(arc.EndPoint);
        }
        public void AddArc(double x, double y, double radius, double angl_start, double angl_end)
        {
            Arc arc = new Arc(x, y, radius, angl_start, angl_end);
            points.Add(arc);
            points.Add(arc.EndPoint);
        }
        public void AddArc(Point center, double radius, double angl_start, double angl_end)
        {
            Arc arc = new Arc(center.X, center.Y, radius, angl_start, angl_end);
            points.Add(arc);
            points.Add(arc.EndPoint);
        }
        public Point GetPoint(int point) { if (point >= 0 && point < points.Count) return new Point(points[point].X, points[point].Y); return null; }
        public void Remove(int point) { if (point >= 0 && point < points.Count) points.RemoveAt(point); }
        public override dynamic Clone()
        {
            Contur temp = new Contur { };
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].GetType() != typeof(Point))
                {
                    dynamic clone = points[i].Clone();
                    temp.points.Add(clone);
                    temp.points.Add(clone.EndPoint);
                    i++;
                }
                else
                {
                    temp.points.Add(points[i]);
                }
            }
            temp.Color = this.Color; temp.Stroke = this.Stroke; return temp;
        }

        public override Point Location { get { if (points.Count == 0) return new Point(0, 0); return new Point(Left, Top); } }
        public override Point Center { get { if (points.Count == 0) return new Point(0, 0); return new Point((Left + Right) / 2, (Top + Down) / 2); } }
        public override double Height { get { return Math.Abs(Down - Top); } }
        public override double Width { get { return Math.Abs(Right - Left); } }
        public override double Left { get { return points.Min(p => (p.GetType() == typeof(Point)) ? p.X : p.Left); } }
        public override double Right { get { return points.Max(p => (p.GetType() == typeof(Point)) ? p.X : p.Right); } }
        public override double Top { get { return points.Min(p => (p.GetType() == typeof(Point)) ? p.Y : p.Top); } }
        public override double Down { get { return points.Max(p => (p.GetType() == typeof(Point)) ? p.Y : p.Down); } }
        public int Count { get { return points.Count; } }

        public void Reverse()
        {
            List<dynamic> temp = new List<dynamic>();
            for (int i = points.Count - 1; i >= 0; i--)
            {
                if ((i - 1) >= 0 && points[i - 1].GetType() != typeof(Point))
                {
                    points[i - 1].Reverse();
                    temp.Add(points[i - 1]);
                    temp.Add(points[i]);
                    i--;
                }
                else
                {
                    temp.Add(points[i]);
                }
            }
            for (int i = 0; i < temp.Count; i++)
                points[i] = temp[i];
            //points.Reverse();
        }

        public void MirrorX()
        {
            Point temp = Center;
            SetCenter(0, 0);
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].GetType() != typeof(Point))
                {
                    points[i].MirrorX();
                    i++;
                }
                else
                {
                    points[i] = new Point(points[i].X, -points[i].Y);
                }
            }
            SetCenter(temp);
        }
        public void MirrorY()
        {

            Point temp = Center;
            this.SetCenter(0, 0);
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].GetType() != typeof(Point))
                {
                    points[i].MirrorY();
                    i++;
                }
                else
                {
                    points[i] = new Point(-points[i].X, points[i].Y);
                }
            }
            SetCenter(temp);
        }
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
        public void SetLocation(double x, double y, int point) { SetLocation(new Point(x, y), point); }
        public void SetLocation(Point newPoint, int point)
        {
            if (point >= 0 && point < points.Count)
            {
                double dx = newPoint.X - points[point].X; double dy = newPoint.Y - points[point].Y; this.Move(dx, dy);
            }
        }
        public override void Move(double dx, double dy)
        {
            for (int i = 0; i < points.Count; i++) { if (points[i].GetType() != typeof(Point)) { points[i].Move(dx, dy); i++; points[i] = points[i - 1].EndPoint; } else { points[i] = new Point(points[i].X + dx, points[i].Y + dy); } }
        }
        public override void Scale(double dx, double dy)
        {
            Point temp = Center;
            for (int i = 0; i < points.Count; i++) { if (points[i].GetType() != typeof(Point)) { points[i].Scale(dx, dy); i++; } else { points[i] = new Point(points[i].X * dx, points[i].Y * dy); } }
            SetCenter(temp);
        }
        public override void Rotate(double angl)
        {
            Point temp = Center;
            SetCenter(0, 0);
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].GetType() != typeof(Point))
                {
                    Point temppoint = new Point(points[i].X * Math.Round(Math.Cos(angl), 14) - points[i].Y * Math.Round(Math.Sin(angl), 14), points[i].X * Math.Round(Math.Sin(angl), 14) + points[i].Y * Math.Round(Math.Cos(angl), 14));

                    points[i].Rotate(angl);
                    points[i].SetStartPoint(temppoint);
                    i++;
                }
                else
                {
                    points[i] = new Point(points[i].X * Math.Round(Math.Cos(angl), 14) - points[i].Y * Math.Round(Math.Sin(angl), 14), points[i].X * Math.Round(Math.Sin(angl), 14) + points[i].Y * Math.Round(Math.Cos(angl), 14));
                }
            }
            SetCenter(temp);
        }
        public double Length
        {
            get
            {
                if (Count < 2) return 0;
                double temp = 0;
                for (int i = 0; i < Count - 1; i++)
                {
                    temp += Math.Sqrt(Math.Pow(points[i].X - points[i + 1].X, 2) + Math.Pow(points[i].Y - points[i + 1].Y, 2));
                }
                if (Count >= 3 && Closed)
                    temp += Math.Sqrt(Math.Pow(points[0].X - points[Count - 1].X, 2) + Math.Pow(points[0].Y - points[Count - 1].Y, 2));
                return temp;
            }
        }
        public override double Squeare
        {
            get
            {
                if (Count < 3) return 0;
                double res = 0, s;
                for (int i = 0; i < Count; i++)
                {
                    if (i == 0)
                    {
                        s = points[i].X * (points[Count - 1].Y - points[i + 1].Y); //если i == 0, то points[i-1].Y заменяем на points[Count-1].Y
                        res += s;
                    }
                    else
                      if (i == Count - 1)
                    {
                        s = points[i].X * (points[i - 1].Y - points[0].Y); // если i == n-1, то points[i+1].Y заменяем на y[0]
                        res += s;
                    }
                    else
                    {
                        s = points[i].X * (points[i - 1].Y - points[i + 1].Y);
                        res += s;
                    }
                }
                return Math.Abs(res / 2);

            }
        }
        //___________________________________________

        public bool Closed { get; set; } = true;
        public double Stroke { get; set; } = 1;
        public PdfSharpCore.Drawing.XColor Color { get; set; } = PdfSharpCore.Drawing.XColors.Black;
        public override void ToPDFSharp(PdfSharpCore.Drawing.XGraphics contur)
        {
            PdfSharpCore.Drawing.XPen pen = new PdfSharpCore.Drawing.XPen(Color, Stroke)
            {
                DashStyle = PdfSharpCore.Drawing.XDashStyle.Custom
            };
            PdfSharpCore.Drawing.XGraphicsPath sdsd = new PdfSharpCore.Drawing.XGraphicsPath();
            List<Point> temp = new List<Point> { };
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].GetType() != typeof(Point))
                {
                    if (temp.Count == 1)
                    {
                        temp.Add(new Point(points[i].X, points[i].Y));
                    }
                    if (temp.Count > 1)
                    {
                        List<PdfSharpCore.Drawing.XPoint> xPoints = new List<PdfSharpCore.Drawing.XPoint> { };
                        foreach (Point pointPX in temp)
                        {
                            xPoints.Add(new PdfSharpCore.Drawing.XPoint(PdfSharpCore.Drawing.XUnit.FromMillimeter(pointPX.X), PdfSharpCore.Drawing.XUnit.FromMillimeter(pointPX.Y)));
                        }
                        sdsd.AddLines(xPoints.ToArray());
                    }
                    temp = new List<Point>();
                    points[i].ToPDFSharp(ref sdsd);
                    i++;
                }
                else
                {
                    temp.Add(points[i]);
                }

            }
            if (temp.Count == 1)
            {
                if (points[^3].GetType() != typeof(Point))
                {
                    temp = new List<Point>();
                    temp.Add(points[^2]);
                    temp.Add(points[^1]);
                }
            }
            if (temp.Count > 1)
            {
                List<PdfSharpCore.Drawing.XPoint> xPoints = new List<PdfSharpCore.Drawing.XPoint> { };
                foreach (Point pointPX in temp)
                {
                    xPoints.Add(new PdfSharpCore.Drawing.XPoint(PdfSharpCore.Drawing.XUnit.FromMillimeter(pointPX.X), PdfSharpCore.Drawing.XUnit.FromMillimeter(pointPX.Y)));
                }
                sdsd.AddLines(xPoints.ToArray());
            }
            if (Closed)
                sdsd.CloseFigure();
            contur.DrawPath(pen, sdsd);
#if DEBUG
            //contur.DrawEllipse(pen, PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.X-5), PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.Y-5), PdfSharpCore.Drawing.XUnit.FromMillimeter(10), PdfSharpCore.Drawing.XUnit.FromMillimeter(10));
#endif
        }

        public Point[] GetPoints()
        {
            List<Point> temp = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                temp.Add(new Point(points[i].X, points[i].Y));
            }
            return temp.ToArray();
        }

        static public Contur Rectangle(double x, double y, double width, double height, double stroke = 1)
        {
            return Rectangle(new Point(x, y), new Point(x + width, y + height), stroke);
        }
        static public Contur Rectangle(Point point1, Point point2, double stroke = 1)
        {
            Contur contur = new Contur() { Stroke = stroke };
            contur.Add(point1);
            contur.Add(point2.X, point1.Y);
            contur.Add(point2);
            contur.Add(point1.X, point2.Y);
            return contur;
        }
        static public Contur Line(Point p1, Point p2, double strokeWidth = 1)
        {
            Contur contur = new Contur
            {
                Closed = false,
                Stroke = strokeWidth,
            };
            contur.Add(p1);
            contur.Add(p2);
            return contur;
        }
        static public Contur Line(double x1, double y1, double x2, double y2, double strokeWidth = 1)
        {
            return Line(new Point(x1, y1), new Point(x2, y2), strokeWidth);
        }
        static public Contur Line25D(double x1, double y1, double z1, double x2, double y2, double z2, double strokeWidth = 1)
        {
            double xdx = Math.Round(Math.Cos(200 * Math.PI / 180), 14);
            double xdy = -Math.Round(Math.Sin(200 * Math.PI / 180), 14);
            double ydx = Math.Round(Math.Cos(330 * Math.PI / 180), 14);
            double ydy = -Math.Round(Math.Sin(330 * Math.PI / 180), 14);
            double zdx = Math.Round(Math.Cos(90 * Math.PI / 180), 14);
            double zdy = -Math.Round(Math.Sin(90 * Math.PI / 180), 14);
            return Line(x1 * xdx + y1 * ydx + z1 * zdx, x1 * xdy + y1 * ydy + z1 * zdy, x2 * xdx + y2 * ydx + z2 * zdx, x2 * xdy + y2 * ydy + z2 * zdy, strokeWidth);
        }
    }
}
