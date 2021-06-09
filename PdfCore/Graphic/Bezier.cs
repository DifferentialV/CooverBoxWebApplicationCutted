using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PDFCore.Graphic
{
    public class Bezier : Graphics
    {
        private readonly List<Point> points;
        public void SetStartPoint(Point newPoint) {double dx = newPoint.X - Point.X; double dy = newPoint.Y - Point.Y; Move(dx, dy); }
        public Point Point { get {if(points.Count >0) return new Point(points[0].X,points[0].Y); return new Point(0, 0); } }
        public double X { get { return Point.X; } }
        public double Y { get { return Point.Y; } }
        public Point EndPoint { get { if (points.Count > 0) return new Point(points[^1].X, points[^1].Y); return new Point(0, 0); } }
        public Bezier() { points = new List<Point> { }; }
        public void Add(Point point) { points.Add(point); }
        public void AddPoint(double dx, double dy)
        {
            if (points.Count > 0)
                Add(points.Last().X + dx, points.Last().Y + dy);
        }
        public void Add(double x, double y) { points.Add(new Point(x, y)); }

        public Point GetPoint(int point) { if (point >= 0 && point < points.Count) return new Point(points[point].X, points[point].Y); return null; }
        public void Remove(int point) { if (point >= 0 && point < points.Count) points.RemoveAt(point); }
        public override dynamic Clone()
        {
            Bezier temp = new Bezier { };
            for (int i = 0; i < points.Count; i++)
            {
                temp.points.Add(points[i]);
            }
            temp.Color = this.Color; temp.Stroke = this.Stroke; return temp;
        }

        public override Point Location { get { if (points.Count == 0) return new Point(0, 0); return new Point(Left, Top); } }
        public override Point Center { get { if (points.Count == 0) return new Point(0, 0); return new Point((Left + Right) / 2, (Top + Down) / 2); } }
        public override double Height { get { return Math.Abs(Down - Top); } }
        public override double Width { get { return Math.Abs(Right - Left); } }
        public override double Left { get { return points.Min(p => p.X); } }
        public override double Right { get { return points.Max(p =>  p.X); } }
        public override double Top { get { return points.Min(p =>  p.Y); } }
        public override double Down { get { return points.Max(p =>  p.Y ); } }
        public int Count { get { return points.Count; } }

        public void Reverse()
        {
            List<dynamic> temp = new List<dynamic>();
            for (int i = points.Count - 1; i >= 0; i--)
            {
                temp.Add(points[i]);
            }
            for (int i = 0; i < temp.Count; i++)
                points[i] = temp[i];
        }

        public void MirrorX()
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(points[i].X, -points[i].Y);
            }
        }
        public void MirrorY()
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(-points[i].X, points[i].Y);
            }
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
            for (int i = 0; i < points.Count; i++) {   points[i] = new Point(points[i].X + dx, points[i].Y + dy);  }
        }
        public override void Scale(double dx, double dy)
        {
            Point temp = Center;
            for (int i = 0; i < points.Count; i++) {  points[i] = new Point(points[i].X * dx, points[i].Y * dy);  }
            SetCenter(temp);
        }
        public override void Rotate(double angl)
        {
            Point temp = Center;
            SetCenter(0, 0);
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(points[i].X * Math.Round(Math.Cos(angl), 14) - points[i].Y * Math.Round(Math.Sin(angl), 14), points[i].X * Math.Round(Math.Sin(angl), 14) + points[i].Y * Math.Round(Math.Cos(angl), 14));
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
            if (points.Count <= 1) return;
            PdfSharpCore.Drawing.XPen pen = new PdfSharpCore.Drawing.XPen(Color, Stroke)
            {
                DashStyle = PdfSharpCore.Drawing.XDashStyle.Custom
            };
            PdfSharpCore.Drawing.XGraphicsPath path = new PdfSharpCore.Drawing.XGraphicsPath();
            ToPDFSharp(ref path);
            contur.DrawPath(pen, path);
        }
        public void ToPDFSharp(ref XGraphicsPath path)
        {
            int factorial(int a)
            {
                if (a <= 1) return 1;
                return a * factorial(a - 1);
            }
            List<Point> pointsresult = new List<Point>();
            int f1 = factorial(points.Count - 1);
            pointsresult.Add(points[0]);
            for (double t = 0.1; t < 1; t += 0.05)
            {
                double x = 0;
                double y = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    int f2 = factorial(i);
                    int f3 = factorial(points.Count - i - 1);
                    double p1 = Math.Pow(t, i);
                    double p2 = Math.Pow(1 - t, points.Count - i - 1);
                    double c = f1 / (f2 * f3);
                    x += c * p1 * p2 * points[i].X;
                    y += c * p1 * p2 * points[i].Y;
                }
                pointsresult.Add(new Point(x, y));
            }
            pointsresult.Add(points[^1]);
            List<XPoint> xPoints = new List<XPoint>();
            foreach (var point in pointsresult)
                xPoints.Add(new XPoint(XUnit.FromMillimeter(point.X), XUnit.FromMillimeter(point.Y)));
            path.AddCurve(xPoints.ToArray(), 0.5);
        }
    }
}
