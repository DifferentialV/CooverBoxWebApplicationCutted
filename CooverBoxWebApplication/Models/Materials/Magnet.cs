using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class Magnet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Thickness { get; set; }
        public double Diameter { get; set; }
        public double Price { get; set; }

        public override string ToString()
        {
            return $"{Name} {Diameter}x{Thickness}";
        }
    }
}
