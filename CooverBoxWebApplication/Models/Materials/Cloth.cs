using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class Cloth
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool Roll { get; set; }
        public double Price { get; set; }
        public double Density { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
