using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCore.Graphic
{
    public class GraphicGroup: Graphics
    {
        private readonly List<Graphics> objects;
        public GraphicGroup()
        {
            objects = new List<Graphics> { };
        }
        public void Add(Graphics @object)
        {
            objects.Add(@object);
        }
        public Graphics GetObject(int i)
        {
            if (i >= 0 && i < objects.Count) return objects[i];
            else return null;
        }
        public int Count { get { return objects.Count; } }
        public override double Down { get { if (Count == 0) return 0; else return objects.Max(o => o.Down); } }
        public override double Top { get { if (Count == 0) return 0; else return objects.Min(o => o.Top); } }
        public override double Left   { get { if (Count == 0) return 0; else return objects.Min(o => o.Left); } }
        public override double Right { get { if (Count == 0) return 0; else return objects.Max(o => o.Right); } }
        public override Point Location { get {return new Point(Left, Top); } }
        public override Point Center { get {return new Point((Left + Right) / 2, (Top + Down) / 2); } }
        public override double Height { get { return Math.Abs(Down - Top); } }
        public override double Width { get { return Math.Abs(Right - Left); } }
        public override void Move(double dx, double dy)
        {
            for(int i=0;i<Count;i++)
            {
                objects[i].Move(dx, dy);
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
        public override dynamic Clone()
        {
            GraphicGroup group = new GraphicGroup();
            for (int i = 0; i < Count; i++)
            {
                group.Add(objects[i].Clone());
            }
            return group;
        }
        public override void Rotate(double angl)
        {
            Point center = Center;
            for (int i = 0; i < Count; i++)
            {
                //                    Point temppoint = new Point(points[i].X * Math.Round(Math.Cos(angl), 14) - points[i].Y * Math.Round(Math.Sin(angl), 14),points[i].X * Math.Round(Math.Sin(angl), 14) + points[i].Y * Math.Round(Math.Cos(angl), 14));
                Point centerpoint = objects[i].Center;
                objects[i].Rotate(angl);
                objects[i].SetCenter(new Point(centerpoint.X * Math.Round(Math.Cos(angl), 14) - centerpoint.Y * Math.Round(Math.Sin(angl), 14), centerpoint.X * Math.Round(Math.Sin(angl), 14) + centerpoint.Y * Math.Round(Math.Cos(angl), 14)));
            }
            this.SetCenter(center);
        }
        public override void Scale(double x, double y)
        {
            throw new NotImplementedException();
        }
        public override void ToPDFSharp(PdfSharpCore.Drawing.XGraphics contur)
        {
            foreach (var var in objects)
            {
                var?.ToPDFSharp(contur);
            }
        }
        public override double Squeare { get { return objects.Max(o => o.Squeare); } }
    }
}
