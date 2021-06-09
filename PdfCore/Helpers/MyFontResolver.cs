using PdfSharpCore.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCore.Helpers
{
    public class MyFontResolver : IFontResolver
    {
        public string DefaultFontName { get; } = "Myriad Pro";

        public FontResolverInfo ResolveTypeface(string familyName,bool isBold, bool isItalic)
        {
            var name = familyName.ToLower();
            switch (name)
            {
                case "myriad pro": return new FontResolverInfo("MyriadProRegular");
                case "comic sans ms": return new FontResolverInfo("ComicSansMS");
                case "arial regular": return new FontResolverInfo("ArialRegular");
                default: return new FontResolverInfo("ArialRegular");
            }
        }
        public byte[] GetFont(string faceName)
        {
            switch (faceName)
            {
                case "MyriadProRegular": return FontHelper.MyriadProRegular;
                case "ComicSansMS": return FontHelper.ComicSansMS;
                case "ArialRegular": return FontHelper.ArialRegular;
                default: return null;
            }
                      
        }

        internal static MyFontResolver OurGlobalFontResolver = null;

        /// <summary>
        /// Ensure the font resolver is only applied once (or an exception is thrown)
        /// </summary>
        internal static void Apply(string apppath)
        {
            FontHelper.SetPath(apppath);
            if (OurGlobalFontResolver == null || GlobalFontSettings.FontResolver == null)
            {
                if (OurGlobalFontResolver == null)
                    OurGlobalFontResolver = new MyFontResolver();

                GlobalFontSettings.FontResolver = OurGlobalFontResolver;
            }
        }

    }
}
