using System;
using System.Collections.Generic;
using System.Linq;
using PDFCore.Helpers;
namespace PDFCore
{
    public abstract class Base
    {
        public Dictionary<string, dynamic> Data = new Dictionary<string, dynamic> { };
        public string Name { get; set; }
        public string Note { get; set; }
        abstract public Point Location { get; }
        public abstract double Height { get; }
        public abstract double Width { get; }
        public abstract void Move(double dx, double dy);
        public abstract void ToPDFSharp(PdfSharpCore.Drawing.XGraphics gfx);
        public abstract double Left { get; }
        public abstract double Right { get; }
        public abstract double Top { get; }
        public abstract double Down { get; }
    }


    public class Page : Group
    {

        public PdfSharpCore.Pdf.PdfPage ToPDFSharp(PdfSharpCore.Pdf.PdfDocument document)
        {
            PdfSharpCore.Pdf.PdfPage page = document.AddPage();
            page.Width = PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.X + Width + 10);
            page.Height = PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.Y + Height + 10);

            PdfSharpCore.Drawing.XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
            base.ToPDFSharp(gfx);
            gfx.Dispose();
            return page;
        }
        public void AddQRCode(string text, double ScaleW = 0.1)
        {
            Image image = Image.CreateQRCode(text);
            image.Scale = (ScaleW * this.Width) / image.Width;
            image.SetLocation(this.Right - image.Width, this.Top);
            this.Add(image);
        }
        private new void ToPDFSharp(PdfSharpCore.Drawing.XGraphics contur) => throw new NotImplementedException("Метод запрещён для класса Page");
    }



    public class Group : Base
    {
        public Group()
        {
            objects = new List<Base> { };
        }
        public void Add(Base @object)
        {
            objects.Add(@object);
        }
        public override Point Location { get { return new Point(Left, Top); } }
        public override double Height { get { return Math.Abs(Down - Top); } }
        public override double Width { get { return Math.Abs(Right - Left); } }
        public Base GetObject(int i)
        {
            if (i >= 0 && i < objects.Count) return objects[i];
            else return null;
        }
        public void SetLocation(double x, double y)
        {
            SetLocation(new Point(x, y));
        }
        public void SetLocation(Point newPoint)
        {
            double dx = newPoint.X - Location.X; double dy = newPoint.Y - Location.Y;
            for (int i = 0; i < objects.Count; i++) { objects[i].Move(dx, dy); }
        }
        public override void Move(double dx, double dy)
        {
            SetLocation(new Point(Location.X + dx, Location.Y + dy));
        }
        private readonly List<Base> objects;
        public override void ToPDFSharp(PdfSharpCore.Drawing.XGraphics contur)
        {
            foreach(var var in objects.OrderBy(o=>o.Top))
            {
                var?.ToPDFSharp(contur);
            }
        }
        public override double Left { get { return objects.Count == 0?0: objects.Min(o=>o.Left); } }
        public override double Right { get { return objects.Count == 0 ? 0 : objects.Max(o => o.Right); } }
        public override double Top { get { return objects.Count == 0 ? 0 : objects.Min(o => o.Top); } }
        public override double Down { get { return objects.Count == 0 ? 0 : objects.Max(o => o.Down); } }
        public int Count { get { return objects.Count; } }
    }


    public class TextFrame : Base
    {
        public TextFrame(Point location = null, double width = 100, double height = 100, double strokeWidth = 14, string styleName = null)
        {
            if (location != null) SetLocation(location);
            SetWidth(width);
            SetHeight(height);
            StrokeWidth = strokeWidth;
            if (styleName != null) StyleName = styleName;
        }
        Point _location = new Point(0, 0);
        double _height = 100;
        double _width = 100;
        public string Content { get; set; } = "";
        public double StrokeWidth { get; set; } = 14;
        public string StyleName { get; set; } = "Myriad Pro";
        public override Point Location { get { return _location; } }
        public void SetLocation(Point location) { _location = location; }
        public void SetLocation(double x, double y) { _location = new Point(x, y); }

        public void SetWidth(double width) { _width = width; }
        public void SetHeight(double height) { _height = height; }
        public override void Move(double dx, double dy)
        {
            SetLocation(new Point(Location.X + dx, Location.Y + dy));
        }
        public override double Height { get { return _height; } }
        public override double Width { get { return _width; } }
        public void MinimizeHeight()
        {
            List<string> temp = Content.Split("\n").ToList();
            List<string> temp2 = new List<string>();
            double h;
            {
                PdfSharpCore.Pdf.PdfDocument pdfDocument = new PdfSharpCore.Pdf.PdfDocument();
                PdfSharpCore.Pdf.PdfPage page = pdfDocument.AddPage();
                PdfSharpCore.Drawing.XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                h = (gfx.MeasureString("Yy", new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Height) / PdfSharpCore.Drawing.XUnit.FromMillimeter(1);
                foreach (var content in temp)
                {
                    temp2.Add("");
                    double length = 0;
                    foreach (var st in content.Split(" "))
                    {
                        length += (gfx.MeasureString(" " + st, new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Width / PdfSharpCore.Drawing.XUnit.FromMillimeter(1));
                        if (length < Width)
                        {
                            temp2[^1] += " " + st;
                        }
                        else
                        {
                            temp2[^1] = temp2[^1].Trim();
                            temp2.Add("");
                            length = (gfx.MeasureString(st, new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Width / PdfSharpCore.Drawing.XUnit.FromMillimeter(1));
                            temp2[^1] += st;
                        }
                    }
                    temp2[^1] = temp2[^1].Trim();
                }
                pdfDocument = null;
            }
            _height = Math.Ceiling(temp2.Count * h);
        }

        public override void ToPDFSharp(PdfSharpCore.Drawing.XGraphics contur)
        {
            List<string> temp = Content.Split("\n").ToList();
            List<string> temp2 = new List<string>();
            double h;
            {
                PdfSharpCore.Pdf.PdfDocument pdfDocument = new PdfSharpCore.Pdf.PdfDocument();
                PdfSharpCore.Pdf.PdfPage page = pdfDocument.AddPage();
                PdfSharpCore.Drawing.XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                h = (gfx.MeasureString("Yy", new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Height) / PdfSharpCore.Drawing.XUnit.FromMillimeter(1);
                foreach (var content in temp)
                {
                    temp2.Add("");
                    double length = 0;
                    foreach (var st in content.Split(" "))
                    {
                        length += (gfx.MeasureString(" " + st, new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Width / PdfSharpCore.Drawing.XUnit.FromMillimeter(1));
                        if (length < Width)
                        {
                            temp2[^1] += " " + st;
                        }
                        else
                        {
                            temp2[^1] = temp2[^1].Trim();
                            temp2.Add("");
                            length = (gfx.MeasureString(st, new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Width / PdfSharpCore.Drawing.XUnit.FromMillimeter(1));
                            temp2[^1] += st;
                        }
                    }
                    temp2[^1] = temp2[^1].Trim();
                }
                pdfDocument = null;
            }

            for (int i = 0; i < temp2.Count; i++)
            {
                TextString textString = new TextString(strokeWidth: StrokeWidth, styleName: StyleName);
                textString.Content = temp2[i];
                textString.SetLocation(Location.X, Location.Y + h * i);
                textString.ToPDFSharp(contur);
            }
        }

        public override double Left { get { return Location.X; } }
        public override double Right { get { return Location.X + Width; } }
        public override double Top { get { return Location.Y; } }
        public override double Down { get { return Location.Y + Height; } }
    }

    public class TextString : Base
    {
        public TextString(Point location = null, double strokeWidth = 14, string styleName = null, char orient = 'L')
        {
            if (location != null) SetLocation(location);

            StrokeWidth = strokeWidth;
            if (styleName != null) StyleName = styleName;
            if (new List<char> { 'L', 'C', 'R' }.Contains(orient)) Orient = orient;
        }
        Point _location = new Point(0, 0);
        public string Content { get; set; } = "";
        public double StrokeWidth { get; set; } = 14;
        public string StyleName { get; set; } = "Myriad Pro";
        public bool Rotate { get; set; } = false;
        public char Orient { get; set; } = 'L';
        public override Point Location { get { return _location; } }
        public void SetLocation(Point location) { _location = location; }
        public void SetLocation(double x, double y) { _location = new Point(x, y); }
        public override double Height
        {
            get
            {
                PdfSharpCore.Pdf.PdfDocument pdfDocument = new PdfSharpCore.Pdf.PdfDocument();
                PdfSharpCore.Pdf.PdfPage page = pdfDocument.AddPage();
                PdfSharpCore.Drawing.XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                if (Rotate) return (gfx.MeasureString(Content, new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Width) / PdfSharpCore.Drawing.XUnit.FromMillimeter(1);
                else return (gfx.MeasureString("Yy", new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Height) / PdfSharpCore.Drawing.XUnit.FromMillimeter(1);
            }
        }
        public override double Width
        {
            get
            {
                PdfSharpCore.Pdf.PdfDocument pdfDocument = new PdfSharpCore.Pdf.PdfDocument();
                PdfSharpCore.Pdf.PdfPage page = pdfDocument.AddPage();
                PdfSharpCore.Drawing.XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                if (Rotate) return (gfx.MeasureString("Yy", new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Height) / PdfSharpCore.Drawing.XUnit.FromMillimeter(1);
                else return (gfx.MeasureString(Content, new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular)).Width) / PdfSharpCore.Drawing.XUnit.FromMillimeter(1);
            }
        }
        public override void Move(double dx, double dy)
        {
            SetLocation(new Point(Location.X + dx, Location.Y + dy));
        }


        public PdfSharpCore.Drawing.XColor Color { get; set; } = PdfSharpCore.Drawing.XColors.Black;

        public override void ToPDFSharp(PdfSharpCore.Drawing.XGraphics gfx)
        {
            PdfSharpCore.Drawing.XFont font = new PdfSharpCore.Drawing.XFont(StyleName, StrokeWidth, PdfSharpCore.Drawing.XFontStyle.Regular);
            PdfSharpCore.Drawing.XSize size = gfx.MeasureString(Content, font);
            PdfSharpCore.Drawing.XPoint xPoint = new PdfSharpCore.Drawing.XPoint(PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.X), PdfSharpCore.Drawing.XUnit.FromMillimeter(Location.Y) + size.Height);


            if (Rotate)
            {
                gfx.RotateTransform(270);

                switch (Orient)
                {
                    case 'C': xPoint = new PdfSharpCore.Drawing.XPoint(-(xPoint.Y + size.Width / 2), xPoint.X); break;
                    case 'R': xPoint = new PdfSharpCore.Drawing.XPoint(-(xPoint.Y + size.Width), xPoint.X); break;
                    default: xPoint = new PdfSharpCore.Drawing.XPoint(-xPoint.Y, xPoint.X); break;
                }
                gfx.DrawString(Content, font, new PdfSharpCore.Drawing.XSolidBrush(Color), xPoint);
                gfx.RotateTransform(90);
            }
            else
            {
                switch (Orient)
                {
                    case 'C': xPoint = new PdfSharpCore.Drawing.XPoint(xPoint.X - size.Width / 2, xPoint.Y); break;
                    case 'R': xPoint = new PdfSharpCore.Drawing.XPoint(xPoint.X - size.Width, xPoint.Y); break;
                }
                gfx.DrawString(Content, font, new PdfSharpCore.Drawing.XSolidBrush(Color), xPoint);
            }
        }

        public override double Left { get { return Location.X - (Rotate ? 0 : 1) * ((Orient == 'C' ? 1 : 0) * Width / 2 + (Orient == 'R' ? 1 : 0) * Width); } }
        public override double Right { get { return Location.X + Width - (Rotate ? 0 : 1) * ((Orient == 'C' ? 1 : 0) * Width / 2 + (Orient == 'R' ? 1 : 0) * Width); } }
        public override double Top { get { return Location.Y - (Rotate ? 1 : 0) * ((Orient == 'C' ? 1 : 0) * Height / 2 + (Orient == 'R' ? 1 : 0) * Height); } }
        public override double Down { get { return Location.Y + Height - (Rotate ? 1 : 0) * ((Orient == 'C' ? 1 : 0) * Height / 2 + (Orient == 'R' ? 1 : 0) * Height); } }
    }





}
