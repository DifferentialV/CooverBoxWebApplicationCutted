using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCore.Graphic
{
    public abstract class Graphics : Base
    {
        public abstract void Rotate(double ang);
        public abstract void Scale(double x, double y);
        public abstract Point Center { get; }
        public abstract dynamic Clone();
        public abstract void SetLocation(double x, double y);
        public abstract void SetLocation(Point newPoint);
        public abstract void SetCenter(double x, double y);
        public abstract void SetCenter(Point newPoint);
        public abstract double Squeare { get; }
    }
}
