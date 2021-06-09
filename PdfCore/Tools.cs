using System;
using System.Collections.Generic;
using System.Text;
using PDFCore.Graphic;
namespace PDFCore
{
    static class Tools
    {
        static public Group LineDimension(Point p1, Point p2, bool Vertical = false, double range = 15, double textStroke = 50, string styleName = null, bool offline = false)
        {
            Group group = new Group();
            TextString text = new TextString(strokeWidth: textStroke, orient: 'C');
            if (styleName != null) text.StyleName = styleName;
            text.Content = Math.Round(Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2))).ToString();
            text.Color = PdfSharpCore.Drawing.XColors.DarkBlue;
            if (Vertical)
            {
                dynamic minx = Math.Min(p1.X, p2.X);
                text.Rotate = true;
                if(offline == false)
                {
                    Contur contur = Contur.Line(p1, new Point(minx - text.Width - range, p1.Y));
                    contur.Color = PdfSharpCore.Drawing.XColors.DarkBlue;
                    group.Add(contur);
                    contur = Contur.Line(p2, new Point(minx - text.Width - range, p2.Y));
                    contur.Color = PdfSharpCore.Drawing.XColors.DarkBlue;
                    group.Add(contur);
                    contur = Contur.Line(new Point(minx - range, p1.Y), new Point(minx - range, p2.Y));
                    contur.Color = PdfSharpCore.Drawing.XColors.DarkBlue;
                    group.Add(contur);
                }

                text.SetLocation(Point.PointMedium(new Point(minx - range - 1, p1.Y), new Point(minx - range - 1, p2.Y)));
                group.Add(text);
            }
            else
            {
                dynamic miny = Math.Min(p1.Y, p2.Y);
                if (offline == false)
                {
                    Contur contur = Contur.Line(p1, new Point(p1.X, miny - text.Height - range));
                    contur.Color = PdfSharpCore.Drawing.XColors.DarkBlue;
                    group.Add(contur);
                    contur = Contur.Line(p2, new Point(p2.X, miny - text.Height - range));
                    contur.Color = PdfSharpCore.Drawing.XColors.DarkBlue;
                    group.Add(contur);
                    contur = Contur.Line(new Point(p1.X, miny - range), new Point(p2.X, miny - range));
                    contur.Color = PdfSharpCore.Drawing.XColors.DarkBlue;
                    group.Add(contur);
                }
                text.SetLocation(Point.PointMedium(new Point(p1.X, miny - range - text.Height - 1), new Point(p2.X, miny - range - text.Height - 1)));
                group.Add(text);
            }
            return group;
        }
        static public Group LineDimension(dynamic x1, dynamic y1, dynamic x2, dynamic y2, bool Vertical = false, double range = 15, double textStroke = 50, string styleName = null, bool offline = false)
        {
            return LineDimension(new Point(x1, y1), new Point(x2, y2), Vertical, range, textStroke, styleName, offline);

        }
        static public Group AddLineDimension(Base contur, double range = 15, double textStroke = 50, string styleName = null, bool offline = false)
        {
            double X_min = contur.Location.X;
            double Y_min = contur.Location.Y;
            Group group = new Group();
            group.Add(LineDimension(new Point(X_min, Y_min), new Point(X_min, Y_min + contur.Height), true, range, textStroke, styleName, offline));
            group.Add(LineDimension(new Point(X_min, Y_min), new Point(X_min + contur.Width, Y_min), false, range, textStroke, styleName, offline));
            return group;
        }
        static public TextString AddLabel(Base @object, string str, double strokeWidth = 50, string styleName = "Myriad Pro",double otstup = 10)
        {
            TextString temp = new TextString(strokeWidth: strokeWidth, orient: 'C')
            {
                StyleName = styleName,
                Content = str
            };
            temp.SetLocation(@object.Location.X + @object.Width / 2, @object.Top - otstup - temp.Height);
            return temp;
        }
        static public TextString AddStringCenter(Base @object, string str, double strokeWidth = 50, string styleName = "Myriad Pro")
        {
            TextString temp = new TextString(strokeWidth: strokeWidth, orient: 'C')
            {
                StyleName = styleName,
                Content = str
            };
            temp.SetLocation(@object.Location.X + @object.Width / 2, @object.Location.Y + @object.Height / 2 - temp.Height / 2);
            return temp;
        }
    }
}
