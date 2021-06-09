using PdfSharpCore.Drawing;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace PDFCore
{
    class Image : Base
    {
        private readonly XImage image;

        public Image(string filepath)
        {
            image = XImage.FromFile(filepath);
        }
        public Image(Func<System.IO.Stream> stream)
        {
            image = XImage.FromStream(stream);
        }
        public Image(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            this.image = XImage.FromStream(() => stream);
        }
        public static Image CreateQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return new Image(qrCodeImage);
        }
        public double Scale { get; set; } = 1;
        Point _location = new Point(0, 0);
        public override Point Location { get { return _location; } }
        public void SetLocation(Point location) { _location = location; }
        public void SetLocation(double x, double y) { _location = new Point(x, y); }
        public override void Move(double dx, double dy)
        {
            SetLocation(new Point(Location.X + dx, Location.Y + dy));
        }
        public override double Height { get { return image.PixelHeight * Scale / PdfSharpCore.Drawing.XUnit.FromMillimeter(1); } }

        public override double Width { get { return image.PixelWidth * Scale / PdfSharpCore.Drawing.XUnit.FromMillimeter(1); } }

        public override double Left { get { return Location.X; } }

        public override double Right { get { return Location.X + Width; } }

        public override double Top { get { return Location.Y; } }

        public override double Down { get { return Location.Y + Height; } }


        public override void ToPDFSharp(XGraphics gfx)
        {
            gfx.DrawImage(image, XUnit.FromMillimeter(Location.X), XUnit.FromMillimeter(Location.Y), image.PixelWidth * Scale, image.PixelHeight * Scale);
        }
    }
}
