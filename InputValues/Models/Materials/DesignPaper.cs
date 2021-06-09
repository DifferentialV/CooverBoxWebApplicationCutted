using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class DesignPaper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Thickness { get; set; }
        public double Density { get; set; }
        public double Price { get; set; }
        public bool VolocnoX { get; set; }
        public bool FullColor { get; set; }
        public bool Sticker { get; set; }
        public bool CoverIsolon { get; set; }
        public bool DisCarton { get; set; }
        public bool CraftCarton { get; set; }
        public bool CatHouse { get; set; }
        public int ViewPriority { get; set; }
        


        public override string ToString()
        {
            string result = $"{Name} {Color} {Density} г/м2";
            //if (Price > 0)
            //    result += $" {Price} грн/лист";
            return result.Trim();
        }

    }
    public static class PaperTypeUsing
    {
        public const string Sticker = "Sticker";
        public const string CoverIsolon = "CoverIsolon";
        public const string DisCarton = "DisCarton";
        public const string CraftCarton = "CraftCarton";
        public const string CatHouse = "CatHouse";
    }
}
