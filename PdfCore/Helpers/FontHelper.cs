using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace PDFCore.Helpers
{
    public static class FontHelper
    {
        public static string AppPath { get; private set; }
        public static void SetPath(string path) { if (string.IsNullOrEmpty(AppPath) && Directory.Exists(path)){ AppPath = path; } }
        public static byte[] MyriadProRegular
        {
            get { return LoadFontData("MyriadProRegular.ttf"); }
        }
        public static byte[] ComicSansMS
        {
            get { return LoadFontData("ComicSansMS.ttf"); }
        }
        public static byte[] ArialRegular
        {
            get { return LoadFontData("ArialRegular.ttf"); }
        }
        static byte[] LoadFontData(string name)
        {
            using (Stream stream = File.OpenRead(Path.Combine(AppPath,"Fonts",name)))
            {
                if (stream == null)
                    return null;

                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);
                return data;
            }
        }
    }
}
