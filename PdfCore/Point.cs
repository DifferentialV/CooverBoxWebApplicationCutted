
using System.Linq;
using System;

namespace PDFCore
{

    

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point() { X = 0; Y = 0; }
        public Point(double x, double y) { X = x; Y = y; }
        public Point(double[] point) { X = 0; Y = 0; if (point.Count() == 2) { X = point[0]; Y = point[1]; } }



        public static double Angel(PDFCore.Point p1, PDFCore.Point p2)
        {
            double ang = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            if (ang < 0)
                ang = 2 * Math.PI + ang;
            return ang;
        }
        public static Point PointMedium(Point p1, Point p2)
        {
            return new Point((p2.X + p1.X) / 2, (p2.Y + p1.Y) / 2);

        }
        public static Point PointInRange(Point p1, double ang, double range)
        {
            return new Point(p1.X + range * Math.Round(Math.Cos(ang), 12), p1.Y + range * Math.Round(Math.Sin(ang), 12));

        }

        public static Point IntersectLinLin(Point p1, double ang1, Point p2, double ang2)
        {
            double sin1 = Math.Round(Math.Sin(ang1), 10);
            double cos1 = Math.Round(Math.Cos(ang1), 10);
            double x1;
            double y1;
            if (cos1 != 0)
            {
                x1 = sin1 / cos1;
                y1 = -1;
            }
            else
            {
                x1 = 1;
                y1 = 0;
            }
            double b1 = p1.X * x1 + p1.Y * y1;
            double sin2 = Math.Round(Math.Sin(ang2), 10);
            double cos2 = Math.Round(Math.Cos(ang2), 10);
            double x2;
            double y2;
            if (cos2 != 0)
            {
                x2 = sin2 / cos2;
                y2 = -1;
            }
            else
            {
                x2 = 1;
                y2 = 0;
            }
            double b2 = p2.X * x2 + p2.Y * y2;
            double det1 = x1 * y2 - y1 * x2;
            if (det1 == 0)
                return null;
            double det2 = b1 * y2 - y1 * b2;
            double det3 = x1 * b2 - b1 * x2;
            return new Point(det2 / det1, det3 / det1);
        }

        public static Point IntersectLinLin(Point p1, Point p2, Point p3, Point p4)
        {

            Point temp = IntersectLinLin(p1, Angel(p1, p2), p3, Angel(p3, p4));
            if (temp == null)
                return null;
            bool X1 = temp.X >= Math.Min(p1.X, p2.X) && temp.X <= Math.Max(p1.X, p2.X);
            bool Y1 = temp.Y >= Math.Min(p1.Y, p2.Y) && temp.Y <= Math.Max(p1.Y, p2.Y);
            bool X3 = temp.X >= Math.Min(p3.X, p4.X) && temp.X <= Math.Max(p3.X, p4.X);
            bool Y3 = temp.Y >= Math.Min(p3.Y, p4.Y) && temp.Y <= Math.Max(p3.Y, p4.Y);
            if (X1 && X3 && Y1 && Y3)
                return temp;
            else return null;
        }

        public static Point IntersectLinLin(Point p1, double ang, Point p3, Point p4)
        {

            Point temp = IntersectLinLin(p1, ang, p3, Angel(p3, p4));
            if (temp == null)
                return null;
            bool X3 = temp.X >= Math.Min(p3.X, p4.X) && temp.X <= Math.Max(p3.X, p4.X);
            bool Y3 = temp.Y >= Math.Min(p3.Y, p4.Y) && temp.Y <= Math.Max(p3.Y, p4.Y);
            if (X3 && Y3)
                return temp;
            else return null;
        }

        public static double Range(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }



}
