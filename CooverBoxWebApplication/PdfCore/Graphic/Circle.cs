using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCore.Graphic
{
    public class Circle : Graphics
    {
        public override dynamic Clone() { return new Circle(Center, Diameter) { Stroke = Stroke, Color = Color }; }
        public Circle(Point center = null, double diameter = 1)
        {
            if (center != null) _center = center;
            Diameter = diameter;
        }
        public Circle(double x, double y, double diameter)
        {
            _center = new Point(x, y);
            Diameter = diameter;
        }
        public override Point Center { get { return _center; } }
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
        public override Point Location { get { return new Point(_center.X - Diameter / 2, _center.Y - Diameter / 2); } }
        private Point _center = new Point(0, 0);
        public double Diameter { get; set; } = 1;
        public override double Height { get { return Diameter; } }
        public override double Width { get { return Diameter; } }
        public override void Move(double dx, double dy)
        {
            _center = new Point(_center.X + dx, _center.Y + dy);
        }
        public override void Scale(double x, double y)
        {
            Diameter *= x;
        }
        public override void Rotate(double ang) { }
        public double Stroke { get; set; } = 1;
        public PdfSharpCore.Drawing.XColor Color { get; set; } = PdfSharpCore.Drawing.XColors.Black;
        public override void ToPDFSharp(PdfSharpCore.Drawing.XGraphics contur)
        {
            PdfSharpCore.Drawing.XPen pen = new PdfSharpCore.Drawing.XPen(Color, Stroke)
            {
                DashStyle = PdfSharpCore.Drawing.XDashStyle.Custom
            };
            contur.DrawEllipse(pen, PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.X), PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.Y), PdfSharpCore.Drawing.XUnit.FromMillimeter(Width), PdfSharpCore.Drawing.XUnit.FromMillimeter(Height));
        }

        public override double Left { get { return Location.X; } }
        public override double Right { get { return Location.X + Width; } }
        public override double Top { get { return Location.Y; } }
        public override double Down { get { return Location.Y + Height; } }
        public override double Squeare { get { return Math.Pow(Diameter / 2, 2) * Math.PI; } }
    }

}
