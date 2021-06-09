using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class CoverCarton
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Thickness { get; set; }
        public bool VolocnoX { get; set; }
        public double Density { get; set; }
        public double Price { get; set; }
        public bool Cover { get; set; }
        public bool Circle { get; set; }
        public bool CorrugatedBoard { get; set; }

        public override string ToString()
        {
            return $"{Name} {Thickness}мм";
        }

    }
    public static class CartonTypeUsing
    {
        public const string Cover = "Cover";
        public const string Circle = "Circle";
        public const string CorrugatedBoard = "CorrugatedBoard";
    }
}
